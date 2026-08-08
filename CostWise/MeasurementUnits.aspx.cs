using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CostWise.App_Code.BLL;

namespace CostWise
{
    public partial class MeasurementUnits : System.Web.UI.Page
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
                LoadUnits();
                if (Session["MeasurementUnitsMessage"] != null)
                {
                    ResultLabel.Text = Session["MeasurementUnitsMessage"].ToString();
                    Session.Remove("MeasurementUnitsMessage");
                }
            }
        }
        private void LoadUnits()
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<MeasurementUnit> systemUnits = MeasurementUnitBLL.GetSystemUnits();
                List<MeasurementUnit> customUnits = MeasurementUnitBLL.GetCustomUnits(userId);
                SystemUnitsGrid.DataSource = systemUnits;
                SystemUnitsGrid.DataBind();
                CustomUnitsGrid.DataSource = customUnits;
                CustomUnitsGrid.DataBind();
                ResultLabel.Text = string.Empty;
            }
            catch (ArgumentException ex)
            {
                ResultLabel.Text = ex.Message;
            }
            catch (Exception)
            {
                ResultLabel.Text = "אירעה שגיאה בעת טעינת יחידות המידה";
            }
        }
        protected void AddCustomUnitButton_Click(object sender, EventArgs e)
        {
            ResultLabel.Text = string.Empty;
            string unitName = CustomUnitNameTextBox.Text;
            string unitFamily = UnitFamilyDropDownList.SelectedValue;
            string conversionFactorText = ConversionFactorTextBox.Text;
            decimal conversionFactorToBase;
            if (!decimal.TryParse(conversionFactorText, out conversionFactorToBase))
            {
                ResultLabel.Text = "יש להזין מקדם המרה מספרי.";
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                MeasurementUnitBLL.CreateCustomUnit(userId, unitName, unitFamily, conversionFactorToBase);
                Session["MeasurementUnitsMessage"] = "היחידה נוספה בהצלחה.";
                Response.Redirect("~/MeasurementUnits.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
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
                ResultLabel.Text = "אירעה שגיאה בעת הוספת יחידת המידה.";
            }
        }
        protected void CustomUnitsGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            CustomUnitsGrid.EditIndex = e.NewEditIndex;
            LoadUnits();
        }
        protected void CustomUnitsGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            CustomUnitsGrid.EditIndex = -1;
            LoadUnits();
        }
        protected void CustomUnitsGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int measurementUnitId = (int)CustomUnitsGrid.DataKeys[e.RowIndex].Value;
            string unitName = e.NewValues["UnitName"]?.ToString();
            string unitFamily = e.NewValues["UnitFamily"]?.ToString();
            string conversionFactorText = e.NewValues["ConversionFactorToBase"]?.ToString();
            decimal conversionFactorToBase;
            if (!decimal.TryParse(conversionFactorText, out conversionFactorToBase))
            {
                e.Cancel = true;
                ResultLabel.Text = "יש להזין מקדם המרה מספרי.";
                return;
            }
            int userId = (int)Session["UserId"];
            try
            {
                MeasurementUnitBLL.UpdateCustomUnit(userId, measurementUnitId, unitName, unitFamily, conversionFactorToBase);
                Session["MeasurementUnitsMessage"] = "יחידת המידה עודכנה בהצלחה.";
                Response.Redirect("~/MeasurementUnits.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (ArgumentException ex)
            {
                e.Cancel = true;
                ResultLabel.Text = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                e.Cancel = true;
                ResultLabel.Text = ex.Message;
            }
            catch (Exception)
            {
                e.Cancel = true;
                ResultLabel.Text = "אירעה שגיאה בעת עדכון יחידת המידה.";
            }
        }
        protected void CustomUnitsGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int measurementUnitId = (int)CustomUnitsGrid.DataKeys[e.RowIndex].Value;
            int userId = (int)Session["UserId"];
            try
            {
                MeasurementUnitBLL.DeleteCustomUnit(userId, measurementUnitId);
                Session["MeasurementUnitsMessage"] = "יחידת המידה נמחקה בהצלחה.";
                Response.Redirect("~/MeasurementUnits.aspx", false);

                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (ArgumentException ex)
            {
                Session["MeasurementUnitsMessage"] = ex.Message;
                Response.Redirect("~/MeasurementUnits.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (InvalidOperationException ex)
            {
                Session["MeasurementUnitsMessage"] = ex.Message;
                Response.Redirect("~/MeasurementUnits.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (Exception)
            {
                Session["MeasurementUnitsMessage"] = "אירעה שגיאה בעת מחיקת יחידת המידה.";
                Response.Redirect("~/MeasurementUnits.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }
        protected string GetUnitFamilyDisplayName(object unitFamilyValue)
        {
            string unitFamily = unitFamilyValue?.ToString();
            switch (unitFamily)
            {
                case "Weight":
                    return "משקל";

                case "Volume":
                    return "נפח";

                case "Quantity":
                    return "כמות";

                default:
                    return "לא ידוע";
            }
        }
        protected string GetSystemUnitDisplayQuantity(object unitNameValue)
        {
            string unitName = unitNameValue?.ToString()?.Trim();
            switch (unitName)
            {
                case "גרם":
                case "Gram":
                    return "1.00";

                case "מיליליטר":
                case "Milliliter":
                    return "1.00";

                case "קילוגרם":
                case "Kilogram":
                    return "1";

                case "ליטר":
                case "Liter":
                    return "1";

                case "יחידה":
                case "Unit":
                    return "1";

                default:
                    return "—";
            }
        }
    }
}