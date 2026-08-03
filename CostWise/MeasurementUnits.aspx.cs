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
                List<MeasurementUnit> customUnits = MeasurementUnitBLL.GetCustomUnits(userId);
                CustomUnitsGrid.DataSource = customUnits;
                CustomUnitsGrid.DataBind();
                CustomUnitNameTextBox.Text = string.Empty;
                UnitFamilyDropDownList.SelectedIndex = 0;
                ConversionFactorTextBox.Text = string.Empty;
                ResultLabel.Text = "היחידה נוספה בהצלחה.";
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
                CustomUnitsGrid.EditIndex = -1;
                List<MeasurementUnit> customUnits = MeasurementUnitBLL.GetCustomUnits(userId);
                CustomUnitsGrid.DataSource = customUnits;
                CustomUnitsGrid.DataBind();
                ResultLabel.Text = "יחידת המידה עודכנה בהצלחה.";
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
    }
}