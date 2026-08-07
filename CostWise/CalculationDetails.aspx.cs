using System;
using CostWise.App_Code.BLL;
namespace CostWise
{
    public partial class CalculationDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["BusinessId"] == null || Session["UserName"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                LoadCalculationDetails();
            }
        }
        private bool TryGetCalculationId(out int costCalculationId)
        {
            costCalculationId = 0;
            string calculationIdValue = Request.QueryString["calculationId"];
            return int.TryParse(calculationIdValue, out costCalculationId) && costCalculationId > 0;
        }
        private void LoadCalculationDetails()
        {
            CalculationPanel.Visible = false;
            int costCalculationId;
            if (!TryGetCalculationId(out costCalculationId))
            {
                ResultLabel.Text = "מזהה החישוב חסר או אינו תקין.";
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                CostCalculationResult result = CostCalculationBLL.GetCalculationDetails(userId, costCalculationId);
                ProductNameLabel.Text = Server.HtmlEncode(result.Calculation.ProductNameSnapshot);
                YieldLabel.Text = result.Calculation.YieldQuantitySnapshot.ToString() + " " + Server.HtmlEncode(result.Calculation.YieldUnitLabelSnapshot);
                TotalCostLabel.Text = result.Calculation.TotalIngredientCostSnapshot.ToString();
                CostPerYieldUnitLabel.Text = result.Calculation.CostPerYieldUnitSnapshot.ToString();
                CalculatedAtLabel.Text = result.Calculation.CalculatedAtUtc.ToString("dd/MM/yyyy HH:mm:ss") + " UTC";
                CalculationItemsGrid.DataSource = result.Items;
                CalculationItemsGrid.DataBind();
                ResultLabel.Text = string.Empty;
                CalculationPanel.Visible = true;
            }
            catch (ArgumentException ex)
            {
                ResultLabel.Text = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                ResultLabel.Text = ex.Message;
            }
            catch (Exception)
            {
                ResultLabel.Text = "אירעה שגיאה בעת טעינת פרטי החישוב.";
            }
        }
    }
}