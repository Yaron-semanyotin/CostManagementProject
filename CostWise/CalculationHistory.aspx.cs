using System;
using CostWise.App_Code.BLL;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Web.Script.Serialization;

namespace CostWise
{
    public partial class CalculationHistory : System.Web.UI.Page
    {
        private int SelectedHistoryProductId
        {
            get
            {
                object storedProductId = ViewState["SelectedHistoryProductId"];
                if (storedProductId == null)
                {
                    return 0;
                }
                return Convert.ToInt32(storedProductId);
            }
            set
            {
                ViewState["SelectedHistoryProductId"] = value;
            }
        }
        private int SelectedCostCalculationId
        {
            get
            {
                object storedCalculationId = ViewState["SelectedCostCalculationId"];
                if (storedCalculationId == null)
                {
                    return 0;
                }
                return Convert.ToInt32(storedCalculationId);
            }
            set
            {
                ViewState["SelectedCostCalculationId"] = value;
            }
        }
        private CostCalculationResult SelectedCalculationDetails { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["BusinessId"] == null || Session["UserName"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                LoadCalculations();
            }
        }
        private bool TryGetDateFilter(out DateTime? dateFromUtc, out DateTime? dateToUtcExclusive)
        {
            dateFromUtc = null;
            dateToUtcExclusive = null;
            DateTime parsedDate;
            if (!string.IsNullOrWhiteSpace(DateFromTextBox.Text))
            {
                if (!DateTime.TryParseExact(DateFromTextBox.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    ResultLabel.Text = "תאריך ההתחלה אינו תקין.";
                    return false;
                }
                dateFromUtc = DateTime.SpecifyKind(parsedDate.Date, DateTimeKind.Utc);
            }
            if (!string.IsNullOrWhiteSpace(DateToTextBox.Text))
            {
                if (!DateTime.TryParseExact(DateToTextBox.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    ResultLabel.Text = "תאריך הסיום אינו תקין.";
                    return false;
                }
                if (parsedDate.Date == DateTime.MaxValue.Date)
                {
                    ResultLabel.Text = "תאריך הסיום גדול מדי.";
                    return false;
                }
                dateToUtcExclusive = DateTime.SpecifyKind(parsedDate.Date.AddDays(1), DateTimeKind.Utc);
            }
            if (dateFromUtc.HasValue && dateToUtcExclusive.HasValue && dateFromUtc.Value >= dateToUtcExclusive.Value)
            {
                ResultLabel.Text = "תאריך ההתחלה חייב להיות מוקדם או שווה לתאריך הסיום.";
                return false;
            }
            return true;
        }
        private void LoadCalculations()
        {
            DateTime? dateFromUtc;
            DateTime? dateToUtcExclusive;
            if (!TryGetDateFilter(
                out dateFromUtc,
                out dateToUtcExclusive))
            {
                ProductHistoryRepeater.DataSource = null;
                ProductHistoryRepeater.DataBind();
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                List<ProductCalculationHistoryGroup> groups = CostCalculationBLL.GetCalculationHistoryByProduct(userId, dateFromUtc, dateToUtcExclusive);
                ProductHistoryRepeater.DataSource = groups;
                ProductHistoryRepeater.DataBind();
                ResultLabel.Text = groups.Count == 0 ? "לא נמצאו חישובים היסטוריים בטווח שנבחר." : string.Empty;
            }
            catch (ArgumentException ex)
            {
                ResultLabel.Text = ex.Message;
            }
            catch (Exception)
            {
                ResultLabel.Text = "אירעה שגיאה בעת טעינת היסטוריית החישובים.";
            }
        }
        protected void ApplyDateFilterButton_Click(object sender, EventArgs e)
        {
            SelectedCalculationDetails = null;
            SelectedHistoryProductId = 0;
            SelectedCostCalculationId = 0;
            LoadCalculations();
        }
        protected void ClearDateFilterButton_Click(object sender, EventArgs e)
        {
            DateFromTextBox.Text = string.Empty;
            DateToTextBox.Text = string.Empty;
            SelectedCalculationDetails = null;
            SelectedHistoryProductId = 0;
            SelectedCostCalculationId = 0;
            LoadCalculations();
        }
        protected void QuickDateRangeButton_Command(object sender, CommandEventArgs e)
        {
            string selectedRange = Convert.ToString(e.CommandArgument);
            int selectedProductId;
            if (!int.TryParse(e.CommandName, out selectedProductId) || selectedProductId <= 0)
            {
                ResultLabel.Text = "לא ניתן לזהות את המוצר המבוקש.";
                return;
            }
            DateTime todayUtc = DateTime.UtcNow.Date;
            DateTime dateFromUtc;
            switch (selectedRange)
            {
                case "7d":
                    dateFromUtc = todayUtc.AddDays(-6);
                    break;
                case "30d":
                    dateFromUtc = todayUtc.AddDays(-29);
                    break;
                case "3m":
                    dateFromUtc = todayUtc.AddMonths(-3);
                    break;
                case "6m":
                    dateFromUtc = todayUtc.AddMonths(-6);
                    break;
                case "1y":
                    dateFromUtc = todayUtc.AddYears(-1);
                    break;
                case "all":
                    DateFromTextBox.Text = string.Empty;
                    DateToTextBox.Text = string.Empty;
                    ResetSelectedCalculation(selectedProductId);
                    LoadCalculations();
                    return;
                default:
                    ResultLabel.Text = "טווח הזמן שנבחר אינו תקין.";
                    return;
            }
            DateFromTextBox.Text = dateFromUtc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateToTextBox.Text = todayUtc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            ResetSelectedCalculation(selectedProductId);
            LoadCalculations();
        }
        private void ResetSelectedCalculation(int productIdToKeepOpen)
        {
            SelectedCalculationDetails = null;
            SelectedHistoryProductId = productIdToKeepOpen;
            SelectedCostCalculationId = 0;
        }
        private void LoadHistoricalCalculationDetails(int userId, int costCalculationId)
        {
            SelectedCalculationDetails = CostCalculationBLL.GetCalculationDetails(userId, costCalculationId);
            SelectedHistoryProductId = SelectedCalculationDetails.Calculation.ProductId;
        }
        protected void OpenCalculationDetailsButton_Command(object sender, CommandEventArgs e)
        {
            int costCalculationId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out costCalculationId) || costCalculationId <= 0)
            {
                SelectedCalculationDetails = null;
                SelectedHistoryProductId = 0;
                SelectedCostCalculationId = 0;
                LoadCalculations();
                ResultLabel.Text = "לא ניתן לזהות את החישוב המבוקש.";
                return;
            }
            if (SelectedCostCalculationId == costCalculationId)
            {
                SelectedCalculationDetails = null;
                SelectedCostCalculationId = 0;
                LoadCalculations();
                ResultLabel.Text = string.Empty;
                return;
            }
            SelectedCalculationDetails = null;
            SelectedHistoryProductId = 0;
            SelectedCostCalculationId = 0;
            try
            {
                int userId = (int)Session["UserId"];
                LoadHistoricalCalculationDetails(userId, costCalculationId);
                SelectedCostCalculationId = costCalculationId;
                LoadCalculations();
                ResultLabel.Text = string.Empty;
            }
            catch (ArgumentException ex)
            {
                SelectedCalculationDetails = null;
                SelectedHistoryProductId = 0;
                SelectedCostCalculationId = 0;
                LoadCalculations();
                ResultLabel.Text = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                SelectedCalculationDetails = null;
                SelectedHistoryProductId = 0;
                SelectedCostCalculationId = 0;
                LoadCalculations();
                ResultLabel.Text = ex.Message;
            }
            catch (Exception)
            {
                SelectedCalculationDetails = null;
                SelectedHistoryProductId = 0;
                SelectedCostCalculationId = 0;
                LoadCalculations();
                ResultLabel.Text = "אירעה שגיאה בעת טעינת פרטי החישוב.";
            }
        }
        protected string FormatCost(object costValue)
        {
            decimal cost = Convert.ToDecimal(costValue);
            return cost.ToString("N2") + " ₪";
        }
        protected string FormatTrendChange(object changePercentageValue)
        {
            if (changePercentageValue == null)
            {
                return "אין מספיק נתונים להשוואה";
            }
            decimal changePercentage = Convert.ToDecimal(changePercentageValue);
            if (changePercentage > 0m)
            {
                return "↑ " + Math.Abs(changePercentage).ToString("N1") + "%";
            }
            if (changePercentage < 0m)
            {
                return "↓ " + Math.Abs(changePercentage).ToString("N1") + "%";
            }
            return "ללא שינוי 0.0%";
        }
        protected string GetTrendCssClass(object changePercentageValue)
        {
            if (changePercentageValue == null)
            {
                return "text-secondary";
            }
            decimal changePercentage = Convert.ToDecimal(changePercentageValue);
            if (changePercentage > 0m)
            {
                return "text-danger";
            }
            if (changePercentage < 0m)
            {
                return "text-success";
            }
            return "text-secondary";
        }
        protected string FormatCalculationChangeReason(object reasonValue)
        {
            CostCalculationChangeReason reason = reasonValue as CostCalculationChangeReason;
            if (reason == null)
            {
                return string.Empty;
            }
            if (reason.WasAdded)
            {
                return "הרכיב נוסף למתכון.";
            }
            if (reason.WasRemoved)
            {
                return "הרכיב הוסר מהמתכון.";
            }
            List<string> descriptions = new List<string>();
            if (reason.PackagePriceChanged && reason.PreviousPackagePrice.HasValue && reason.CurrentPackagePrice.HasValue && reason.PackagePriceChange.HasValue)
            {
                decimal priceChange = reason.PackagePriceChange.Value;
                string direction = priceChange > 0m ? "עלה" : "ירד";
                string changeDescription = priceChange > 0m ? "עלייה" : "ירידה";
                descriptions.Add("מחיר האריזה " + direction + " מ-" + FormatCost(reason.PreviousPackagePrice.Value)
                    + " ל-" + FormatCost(reason.CurrentPackagePrice.Value) + " (" + changeDescription
                    + " של " + FormatCost(Math.Abs(priceChange)) + ").");
            }
            if (reason.PackageQuantityChanged
                && reason.PreviousPackageQuantityInBaseUnit.HasValue
                && reason.CurrentPackageQuantityInBaseUnit.HasValue)
            {
                descriptions.Add("כמות האריזה השתנתה מ־" + reason.PreviousPackageQuantityInBaseUnit.Value.ToString("0.######")
                    + " ל-" + reason.CurrentPackageQuantityInBaseUnit.Value.ToString("0.######")
                    + " " + reason.BaseUnitName + ".");
            }
            if (reason.RecipeQuantityChanged
                && reason.PreviousRecipeQuantityInBaseUnit.HasValue
                && reason.CurrentRecipeQuantityInBaseUnit.HasValue)
            {
                descriptions.Add("הכמות במתכון השתנתה מ-" + reason.PreviousRecipeQuantityInBaseUnit.Value.ToString("0.######")
                    + " ל-" + reason.CurrentRecipeQuantityInBaseUnit.Value.ToString("0.######") + " " + reason.BaseUnitName + ".");
            }
            if (reason.ManualOverrideChanged)
            {
                descriptions.Add("העלות הידנית של הרכיב השתנתה.");
            }
            if (descriptions.Count == 0)
            {
                descriptions.Add("עלות הרכיב במוצר השתנתה.");
            }
            return string.Join(" ", descriptions);
        }
        protected string FormatSignedCostChange(object costChangeValue)
        {
            decimal costChange = Convert.ToDecimal(costChangeValue);
            if (costChange > 0m)
            {
                return "+" + FormatCost(costChange);
            }
            if (costChange < 0m)
            {
                return "-" + FormatCost(Math.Abs(costChange));
            }
            return FormatCost(0m);
        }

        protected string GetCostChangeCssClass(object costChangeValue)
        {
            decimal costChange = Convert.ToDecimal(costChangeValue);
            if (costChange > 0m)
            {
                return "text-danger";
            }
            if (costChange < 0m)
            {
                return "text-success";
            }
            return "text-secondary";
        }
        protected bool HasCalculationChangeReasons(object reasonsValue)
        {
            List<CostCalculationChangeReason> reasons = reasonsValue as List<CostCalculationChangeReason>;
            return reasons != null && reasons.Count > 0;
        }
        protected string FormatHistoryPeriod(object calculatedAtValue, object validUntilValue)
        {
            DateTime calculatedAtUtc = Convert.ToDateTime(calculatedAtValue);
            string startDate = calculatedAtUtc.ToString("dd/MM/yyyy");
            if (validUntilValue == null)
            {
                return "מתאריך " + startDate + " עד היום";
            }
            DateTime validUntilUtc = Convert.ToDateTime(validUntilValue);
            string endDate = validUntilUtc.ToString("dd/MM/yyyy");
            return "מתאריך " + startDate + " עד " + endDate;
        }
        protected bool IsCalculationDetailsOpen(object costCalculationIdValue)
        {
            int costCalculationId;

            return int.TryParse(Convert.ToString(costCalculationIdValue), out costCalculationId)
                && SelectedCostCalculationId == costCalculationId;
        }

        protected string GetCalculationDetailsButtonText(object costCalculationIdValue)
        {
            return IsCalculationDetailsOpen(costCalculationIdValue)
                ? "סגור פרטים"
                : "הצג פרטים";
        }
        protected List<CostCalculationResult> GetInlineCalculationDetails(object costCalculationIdValue)
        {
            int costCalculationId;
            if (!int.TryParse(Convert.ToString(costCalculationIdValue), out costCalculationId))
            {
                return new List<CostCalculationResult>();
            }
            if (SelectedCalculationDetails == null || SelectedCostCalculationId != costCalculationId)
            {
                return new List<CostCalculationResult>();
            }
            return new List<CostCalculationResult> { SelectedCalculationDetails };
        }
        protected string GetProductCostChartDataJson(object calculationsValue)
        {
            List<CostCalculation> calculations = calculationsValue as List<CostCalculation>;
            if (calculations == null || calculations.Count == 0)
            {
                return "[]";
            }
            List<object> chartPoints = new List<object>();
            for (int index = calculations.Count - 1; index >= 0; index--)
            {
                CostCalculation calculation = calculations[index];
                chartPoints.Add(new
                {
                    label = calculation.CalculatedAtUtc.ToString("dd/MM/yyyy HH:mm"),
                    value = calculation.TotalIngredientCostSnapshot
                });
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(chartPoints);
        }
        protected string GetProductHistoryCollapseCssClass(object productIdValue)
        {
            int productId = Convert.ToInt32(productIdValue);
            return productId == SelectedHistoryProductId ? "collapse show" : "collapse";
        }
        protected string GetProductHistoryAriaExpanded(object productIdValue)
        {
            int productId = Convert.ToInt32(productIdValue);
            return productId == SelectedHistoryProductId ? "true" : "false";
        }
    }
}