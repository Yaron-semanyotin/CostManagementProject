using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CostWise.App_Code.BLL;
using CostWise.Models;

namespace CostWise
{
    public partial class Recipe : System.Web.UI.Page
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
                LoadProducts();
                LoadIngredients();
                int selectedProductId;
                if (int.TryParse(
                    Request.QueryString["productId"], out selectedProductId))
                {
                    ListItem selectedProductItem = ProductDropDownList.Items.FindByValue(selectedProductId.ToString());
                    if (selectedProductItem != null)
                    {
                        ProductDropDownList.SelectedValue = selectedProductId.ToString();
                        RecipeIngredientFormPanel.Enabled = true;
                        LoadRecipe(selectedProductId);
                    }
                }
                if (Session["RecipeMessage"] != null)
                {
                    ResultLabel.Text = Session["RecipeMessage"].ToString();
                    Session.Remove("RecipeMessage");
                }
            }
        }
        protected void ProductDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResultLabel.Text = string.Empty;
            int productId;
            if (!int.TryParse(ProductDropDownList.SelectedValue, out productId))
            {
                RecipeIngredientFormPanel.Enabled = false;
                RecipeIngredientsGrid.DataSource = null;
                RecipeIngredientsGrid.DataBind();
                return;
            }
            RecipeIngredientFormPanel.Enabled = true;
            LoadRecipe(productId);
        }
        protected void IngredientDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResultLabel.Text = string.Empty;
            int ingredientId;
            if (!int.TryParse(IngredientDropDownList.SelectedValue, out ingredientId))
            {
                MeasurementUnitDropDownList.Items.Clear();
                MeasurementUnitDropDownList.Items.Add(new ListItem("בחר יחידת מידה", ""));
                return;
            }
            LoadCompatibleUnits(ingredientId);
        }
        protected void EditIngredientDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResultLabel.Text = string.Empty;
            DropDownList editIngredientDropDownList = sender as DropDownList;
            if (editIngredientDropDownList == null)
            {
                ResultLabel.Text = "לא ניתן לזהות את הרכיב שנבחר לעריכה.";
                return;
            }
            GridViewRow editRow = editIngredientDropDownList.NamingContainer as GridViewRow;
            if (editRow == null)
            {
                ResultLabel.Text = "לא ניתן לזהות את שורת המתכון.";
                return;
            }
            DropDownList editMeasurementUnitDropDownList = editRow.FindControl("EditMeasurementUnitDropDownList") as DropDownList;
            if (editMeasurementUnitDropDownList == null)
            {
                ResultLabel.Text = "לא ניתן לזהות את רשימת יחידות המידה.";
                return;
            }
            int ingredientId;
            if (!int.TryParse(editIngredientDropDownList.SelectedValue, out ingredientId))
            {
                editMeasurementUnitDropDownList.Items.Clear();
                editMeasurementUnitDropDownList.Items.Add(new ListItem("בחר יחידת מידה", ""));
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                List<MeasurementUnit> compatibleUnits = RecipeIngredientBLL.GetCompatibleUnitsForIngredient(userId, ingredientId);
                editMeasurementUnitDropDownList.DataSource = compatibleUnits;
                editMeasurementUnitDropDownList.DataTextField = "UnitName";
                editMeasurementUnitDropDownList.DataValueField = "MeasurementUnitId";
                editMeasurementUnitDropDownList.DataBind();
                editMeasurementUnitDropDownList.Items.Insert(0, new ListItem("בחר יחידת מידה", ""));
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
                ResultLabel.Text =
                    "אירעה שגיאה בעת טעינת יחידות המידה לעריכה.";
            }
        }
        protected void AddRecipeIngredientButton_Click(object sender, EventArgs e)
        {
            ResultLabel.Text = string.Empty;
            int productId;
            if (!int.TryParse(ProductDropDownList.SelectedValue, out productId))
            {
                ResultLabel.Text = "יש לבחור מוצר.";
                return;
            }
            int ingredientId;
            if (!int.TryParse(IngredientDropDownList.SelectedValue, out ingredientId))
            {
                ResultLabel.Text = "יש לבחור רכיב.";
                return;
            }
            decimal quantity;
            if (!decimal.TryParse(QuantityTextBox.Text, out quantity))
            {
                ResultLabel.Text = "יש להזין כמות מספרית.";
                return;
            }
            int measurementUnitId;
            if (!int.TryParse(MeasurementUnitDropDownList.SelectedValue, out measurementUnitId))
            {
                ResultLabel.Text = "יש לבחור יחידת מידה.";
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                RecipeIngredientBLL.CreateRecipeIngredient(userId, productId, ingredientId, quantity, measurementUnitId);
                Session["RecipeMessage"] = "הרכיב נוסף למתכון בהצלחה.";
                Response.Redirect("~/Recipe.aspx?productId=" + productId, false);
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
                ResultLabel.Text = "אירעה שגיאה בעת הוספת הרכיב למתכון.";
            }
        }
        protected void RecipeIngredientsGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            ResultLabel.Text = string.Empty;
            int productId;
            if (!int.TryParse(ProductDropDownList.SelectedValue, out productId))
            {
                e.Cancel = true;
                ResultLabel.Text = "יש לבחור מוצר.";
                return;
            }
            RecipeIngredientsGrid.EditIndex = e.NewEditIndex;
            LoadRecipe(productId);
        }
        protected void RecipeIngredientsGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            ResultLabel.Text = string.Empty;
            int productId;
            if (!int.TryParse(ProductDropDownList.SelectedValue, out productId))
            {
                e.Cancel = true;
                ResultLabel.Text = "יש לבחור מוצר.";
                return;
            }
            RecipeIngredientsGrid.EditIndex = -1;
            LoadRecipe(productId);
        }
        protected void RecipeIngredientsGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            ResultLabel.Text = string.Empty;
            int productId;
            if (!int.TryParse(ProductDropDownList.SelectedValue, out productId))
            {
                e.Cancel = true;
                ResultLabel.Text = "יש לבחור מוצר.";
                return;
            }
            DataKey dataKey = RecipeIngredientsGrid.DataKeys[e.RowIndex];
            if (dataKey == null)
            {
                e.Cancel = true;
                ResultLabel.Text = "לא ניתן לזהות את שורת המתכון.";
                return;
            }
            int recipeIngredientId;
            if (!int.TryParse(dataKey.Values["RecipeIngredientId"]?.ToString(), out recipeIngredientId))
            {
                e.Cancel = true;
                ResultLabel.Text = "שורת המתכון שנבחרה אינה תקינה.";
                return;
            }
            GridViewRow editRow = RecipeIngredientsGrid.Rows[e.RowIndex];
            DropDownList editIngredientDropDownList = editRow.FindControl("EditIngredientDropDownList") as DropDownList;
            DropDownList editMeasurementUnitDropDownList = editRow.FindControl("EditMeasurementUnitDropDownList") as DropDownList;
            if (editIngredientDropDownList == null || editMeasurementUnitDropDownList == null)
            {
                e.Cancel = true;
                ResultLabel.Text = "לא ניתן לקרוא את ערכי שורת המתכון.";
                return;
            }
            int ingredientId;
            if (!int.TryParse(editIngredientDropDownList.SelectedValue, out ingredientId))
            {
                e.Cancel = true;
                ResultLabel.Text = "יש לבחור רכיב.";
                return;
            }
            decimal quantity;
            if (!decimal.TryParse(e.NewValues["Quantity"]?.ToString(), out quantity))
            {
                e.Cancel = true;
                ResultLabel.Text = "יש להזין כמות מספרית.";
                return;
            }
            int measurementUnitId;
            if (!int.TryParse(
                editMeasurementUnitDropDownList.SelectedValue,
                out measurementUnitId))
            {
                e.Cancel = true;
                ResultLabel.Text = "יש לבחור יחידת מידה.";
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                RecipeIngredientBLL.UpdateRecipeIngredient(userId, productId, recipeIngredientId, ingredientId, quantity, measurementUnitId);
                Session["RecipeMessage"] = "שורת המתכון עודכנה בהצלחה.";
                Response.Redirect("~/Recipe.aspx?productId=" + productId, false);
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
                ResultLabel.Text = "אירעה שגיאה בעת עדכון שורת המתכון.";
            }
        }
        protected void RecipeIngredientsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }
            if ((e.Row.RowState & DataControlRowState.Edit) == 0)
            {
                return;
            }
            RecipeIngredientDisplayDto displayRow = e.Row.DataItem as RecipeIngredientDisplayDto;
            if (displayRow == null)
            {
                throw new InvalidOperationException("לא ניתן לטעון את שורת המתכון לעריכה.");
            }
            DropDownList editIngredientDropDownList = e.Row.FindControl("EditIngredientDropDownList") as DropDownList;
            if (editIngredientDropDownList == null)
            {
                throw new InvalidOperationException("לא ניתן לטעון את רשימת הרכיבים לעריכה.");
            }
            int userId = (int)Session["UserId"];
            List<Ingredient> activeIngredients = IngredientBLL.GetActiveIngredientsForUser(userId);
            editIngredientDropDownList.DataSource = activeIngredients;
            editIngredientDropDownList.DataTextField = "IngredientName";
            editIngredientDropDownList.DataValueField = "IngredientId";
            editIngredientDropDownList.DataBind();
            editIngredientDropDownList.Items.Insert(0, new ListItem("בחר רכיב", ""));
            ListItem selectedIngredientItem = editIngredientDropDownList.Items.FindByValue(displayRow.IngredientId.ToString());
            if (selectedIngredientItem != null)
            {
                editIngredientDropDownList.SelectedValue = displayRow.IngredientId.ToString();
            }
            DropDownList editMeasurementUnitDropDownList = e.Row.FindControl("EditMeasurementUnitDropDownList") as DropDownList;
            if (editMeasurementUnitDropDownList == null)
            {
                throw new InvalidOperationException("לא ניתן לטעון את רשימת היחידות לעריכה.");
            }
            if (selectedIngredientItem == null)
            {
                editMeasurementUnitDropDownList.Items.Clear();
                editMeasurementUnitDropDownList.Items.Add(new ListItem("בחר יחידת מידה", ""));
                return;
            }
            List<MeasurementUnit> compatibleUnits = RecipeIngredientBLL.GetCompatibleUnitsForIngredient(userId, displayRow.IngredientId);
            editMeasurementUnitDropDownList.DataSource = compatibleUnits;
            editMeasurementUnitDropDownList.DataTextField = "UnitName";
            editMeasurementUnitDropDownList.DataValueField = "MeasurementUnitId";
            editMeasurementUnitDropDownList.DataBind();
            editMeasurementUnitDropDownList.Items.Insert(0, new ListItem("בחר יחידת מידה", ""));
            ListItem selectedMeasurementUnitItem = editMeasurementUnitDropDownList.Items.FindByValue(displayRow.MeasurementUnitId.ToString());
            if (selectedMeasurementUnitItem != null)
            {
                editMeasurementUnitDropDownList.SelectedValue = displayRow.MeasurementUnitId.ToString();
            }
        }
        protected void RecipeIngredientsGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            ResultLabel.Text = string.Empty;
            int productId;
            if (!int.TryParse(ProductDropDownList.SelectedValue, out productId))
            {
                ResultLabel.Text = "יש לבחור מוצר.";
                return;
            }
            DataKey dataKey = RecipeIngredientsGrid.DataKeys[e.RowIndex];
            if (dataKey == null)
            {
                ResultLabel.Text = "לא ניתן לזהות את שורת המתכון.";
                return;
            }
            int recipeIngredientId;
            if (!int.TryParse(dataKey.Values["RecipeIngredientId"]?.ToString(), out recipeIngredientId))
            {
                ResultLabel.Text = "שורת המתכון שנבחרה אינה תקינה.";
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                RecipeIngredientBLL.DeleteRecipeIngredient(userId, productId, recipeIngredientId);
                Session["RecipeMessage"] = "שורת המתכון נמחקה בהצלחה.";
                Response.Redirect("~/Recipe.aspx?productId=" + productId, false);
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
                ResultLabel.Text = "אירעה שגיאה בעת מחיקת שורת המתכון.";
            }
        }
        private void LoadProducts()
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<Product> products = ProductBLL.GetActiveProductsForUser(userId);
                ProductDropDownList.DataSource = products;
                ProductDropDownList.DataTextField = "ProductName";
                ProductDropDownList.DataValueField = "ProductId";
                ProductDropDownList.DataBind();
                ProductDropDownList.Items.Insert(0, new ListItem("בחר מוצר", ""));
                ResultLabel.Text = string.Empty;
            }
            catch (ArgumentException ex)
            {
                ResultLabel.Text = ex.Message;
            }
            catch (Exception)
            {
                ResultLabel.Text = "אירעה שגיאה בעת טעינת המוצרים.";
            }
        }
        private void LoadIngredients()
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<Ingredient> ingredients = IngredientBLL.GetActiveIngredientsForUser(userId);
                IngredientDropDownList.DataSource = ingredients;
                IngredientDropDownList.DataTextField = "IngredientName";
                IngredientDropDownList.DataValueField = "IngredientId";
                IngredientDropDownList.DataBind();
                IngredientDropDownList.Items.Insert(0, new ListItem("בחר רכיב", ""));
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
        private void LoadCompatibleUnits(int ingredientId)
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<MeasurementUnit> compatibleUnits = RecipeIngredientBLL.GetCompatibleUnitsForIngredient(userId, ingredientId);
                MeasurementUnitDropDownList.DataSource = compatibleUnits;
                MeasurementUnitDropDownList.DataTextField = "UnitName";
                MeasurementUnitDropDownList.DataValueField = "MeasurementUnitId";
                MeasurementUnitDropDownList.DataBind();
                MeasurementUnitDropDownList.Items.Insert(0, new ListItem("בחר יחידת מידה", ""));
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
                ResultLabel.Text = "אירעה שגיאה בעת טעינת יחידות המידה המתאימות.";
            }
        }
        private void LoadRecipe(int productId)
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<RecipeIngredient> recipeIngredients = RecipeIngredientBLL.GetRecipeIngredientsForProduct(userId, productId);
                List<Ingredient> ingredients = IngredientBLL.GetIngredientsForUser(userId);
                List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
                List<RecipeIngredientDisplayDto> displayRows = new List<RecipeIngredientDisplayDto>();
                foreach (RecipeIngredient recipeIngredient in recipeIngredients)
                {
                    Ingredient ingredient = ingredients.Find(item => item.IngredientId == recipeIngredient.IngredientId);
                    MeasurementUnit measurementUnit = availableUnits.Find(unit => unit.MeasurementUnitId == recipeIngredient.MeasurementUnitId);
                    if (ingredient == null || measurementUnit == null)
                    {
                        throw new InvalidOperationException("לא ניתן להציג אחת משורות המתכון.");
                    }
                    RecipeIngredientDisplayDto displayRow = new RecipeIngredientDisplayDto();
                    displayRow.RecipeIngredientId = recipeIngredient.RecipeIngredientId;
                    displayRow.IngredientId = recipeIngredient.IngredientId;
                    displayRow.IngredientName = ingredient.IngredientName;
                    displayRow.Quantity = recipeIngredient.Quantity;
                    displayRow.MeasurementUnitId = recipeIngredient.MeasurementUnitId;
                    displayRow.MeasurementUnitName = measurementUnit.UnitName;
                    displayRows.Add(displayRow);
                }
                RecipeIngredientsGrid.DataSource = displayRows;
                RecipeIngredientsGrid.DataBind();
            }
            catch (ArgumentException ex)
            {
                RecipeIngredientsGrid.DataSource = null;
                RecipeIngredientsGrid.DataBind();
                ResultLabel.Text = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                RecipeIngredientsGrid.DataSource = null;
                RecipeIngredientsGrid.DataBind();
                ResultLabel.Text = ex.Message;
            }
            catch (Exception)
            {
                RecipeIngredientsGrid.DataSource = null;
                RecipeIngredientsGrid.DataBind();
                ResultLabel.Text = "אירעה שגיאה בעת טעינת המתכון.";
            }
        }
    }
}