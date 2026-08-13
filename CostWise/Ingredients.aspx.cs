using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CostWise.App_Code.BLL;

namespace CostWise
{
    public partial class Ingredients : System.Web.UI.Page
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
                LoadIngredients();
                LoadAvailableUnits();
                if (Session["IngredientsMessage"] != null)
                {
                    ResultLabel.Text = Session["IngredientsMessage"].ToString();
                    Session.Remove("IngredientsMessage");
                }
            }
        }
        private void LoadIngredients()
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<Ingredient> ingredients = IngredientBLL.GetActiveIngredientsForUser(userId);
                List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
                var ingredientRows = ingredients.Select(ingredient =>
                    {
                        MeasurementUnit packageUnit = availableUnits.Find(unit => unit.MeasurementUnitId == ingredient.PackageUnitId);
                        return new
                        {
                            ingredient.IngredientId,
                            ingredient.IngredientName,
                            ingredient.PackagePrice,
                            ingredient.PackageQuantity,
                            ingredient.PackageUnitId,
                            PackageUnitName = packageUnit == null ? "יחידה לא זמינה" : packageUnit.UnitName
                        };
                    }).ToList();
                IngredientsGrid.DataSource = ingredientRows;
                IngredientsGrid.DataBind();
                ResultLabel.Text = string.Empty;
            }
            catch (ArgumentException ex)
            {
                ResultLabel.Text = ex.Message;
            }
            catch (Exception)
            {
                ResultLabel.Text = "אירעה שגיאה בעת טעינת הרכיבים.";
            }
        }
        private void LoadAvailableUnits()
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
                PackageUnitDropDownList.DataSource = availableUnits;
                PackageUnitDropDownList.DataTextField = "UnitName";
                PackageUnitDropDownList.DataValueField = "MeasurementUnitId";
                PackageUnitDropDownList.DataBind();
                PackageUnitDropDownList.Items.Insert(0, new ListItem("בחר יחידת מידה", ""));
            }
            catch (ArgumentException ex)
            {
                ResultLabel.Text = ex.Message;
            }
            catch (Exception)
            {
                ResultLabel.Text = "אירעה שגיאה בעת טעינת יחידות המידה.";
            }
        }
        protected void AddIngredientButton_Click(object sender, EventArgs e)
        {
            ResultLabel.Text = string.Empty;
            string ingredientName = IngredientNameTextBox.Text;
            decimal packagePrice;
            if (!decimal.TryParse(PackagePriceTextBox.Text, out packagePrice))
            {
                ResultLabel.Text = "יש להזין מחיר אריזה מספרי.";
                return;
            }
            decimal packageQuantity;
            if (!decimal.TryParse(PackageQuantityTextBox.Text, out packageQuantity))
            {
                ResultLabel.Text = "יש להזין כמות אריזה מספרית.";
                return;
            }
            int packageUnitId;
            if (!int.TryParse(PackageUnitDropDownList.SelectedValue, out packageUnitId))
            {
                ResultLabel.Text = "יש לבחור יחידת מידה לאריזה.";
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                IngredientBLL.CreateIngredient(userId, ingredientName, packagePrice, packageQuantity, packageUnitId);
                Session["InvalidateProductBuilderDataCache"] = true;
                Session["IngredientsMessage"] = "הרכיב נוסף בהצלחה.";
                Response.Redirect("~/Ingredients.aspx", false);
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
                ResultLabel.Text = "אירעה שגיאה בעת הוספת הרכיב.";
            }
        }
        protected void IngredientsGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            ResultLabel.Text = string.Empty;
            IngredientsGrid.EditIndex = e.NewEditIndex;
            LoadIngredients();
        }
        protected void IngredientsGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            ResultLabel.Text = string.Empty;
            IngredientsGrid.EditIndex = -1;
            LoadIngredients();
        }
        protected void IngredientsGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            ResultLabel.Text = string.Empty;
            int ingredientId = Convert.ToInt32(IngredientsGrid.DataKeys[e.RowIndex].Values["IngredientId"]);
            string ingredientName = e.NewValues["IngredientName"]?.ToString();
            decimal packagePrice;
            if (!decimal.TryParse(e.NewValues["PackagePrice"]?.ToString(), out packagePrice))
            {
                e.Cancel = true;
                ResultLabel.Text = "יש להזין מחיר אריזה מספרי.";
                return;
            }
            decimal packageQuantity;
            if (!decimal.TryParse(e.NewValues["PackageQuantity"]?.ToString(), out packageQuantity))
            {
                e.Cancel = true;
                ResultLabel.Text = "יש להזין כמות אריזה מספרית.";
                return;
            }
            GridViewRow row = IngredientsGrid.Rows[e.RowIndex];
            DropDownList editPackageUnitDropDownList = (DropDownList)row.FindControl("EditPackageUnitDropDownList");
            if (editPackageUnitDropDownList == null)
            {
                e.Cancel = true;
                ResultLabel.Text = "לא ניתן לקרוא את יחידת המידה שנבחרה.";
                return;
            }
            int packageUnitId;
            if (!int.TryParse(editPackageUnitDropDownList.SelectedValue, out packageUnitId))
            {
                e.Cancel = true;
                ResultLabel.Text = "יש לבחור יחידת מידה תקינה.";
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                IngredientBLL.UpdateIngredient(userId, ingredientId, ingredientName, packagePrice, packageQuantity, packageUnitId);
                Session["InvalidateProductBuilderDataCache"] = true;
                Session["IngredientsMessage"] = "הרכיב עודכן בהצלחה.";
                Response.Redirect("~/Ingredients.aspx", false);
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
                ResultLabel.Text = "אירעה שגיאה בעת עדכון הרכיב.";
            }
        }
        protected void IngredientsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }
            bool isEditRow = (e.Row.RowState & DataControlRowState.Edit) != 0;
            if (!isEditRow)
            {
                return;
            }
            DropDownList editPackageUnitDropDownList = (DropDownList)e.Row.FindControl("EditPackageUnitDropDownList");
            if (editPackageUnitDropDownList == null)
            {
                return;
            }
            int userId = (int)Session["UserId"];
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            editPackageUnitDropDownList.DataSource = availableUnits;
            editPackageUnitDropDownList.DataTextField = "UnitName";
            editPackageUnitDropDownList.DataValueField = "MeasurementUnitId";
            editPackageUnitDropDownList.DataBind();
            int currentPackageUnitId = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "PackageUnitId"));
            ListItem currentUnitItem = editPackageUnitDropDownList.Items.FindByValue(currentPackageUnitId.ToString());
            if (currentUnitItem != null)
            {
                currentUnitItem.Selected = true;
            }
        }
        protected void IngredientsGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int ingredientId = Convert.ToInt32(IngredientsGrid.DataKeys[e.RowIndex].Values["IngredientId"]);
            int userId = (int)Session["UserId"];
            try
            {
                string deactivatedIngredientName = IngredientBLL.DeactivateIngredient(userId, ingredientId);
                Session["InvalidateProductBuilderDataCache"] = true;
                string safeIngredientName = Server.HtmlEncode(deactivatedIngredientName);
                Session["IngredientsMessage"] = "\"" + safeIngredientName + "\" הועבר לסל המחזור וניתן להחזירו בכל עת.";
                Response.Redirect("~/Ingredients.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (ArgumentException ex)
            {
                Session["IngredientsMessage"] = ex.Message;
                Response.Redirect("~/Ingredients.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (InvalidOperationException ex)
            {
                Session["IngredientsMessage"] = ex.Message;
                Response.Redirect("~/Ingredients.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (Exception)
            {
                Session["IngredientsMessage"] = "אירעה שגיאה בעת העברת הרכיב לסל המחזור.";
                Response.Redirect("~/Ingredients.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }
    }
}