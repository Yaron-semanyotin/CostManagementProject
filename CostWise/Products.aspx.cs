using CostWise.App_Code.BLL;
using CostWise.App_Code.DAL;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace CostWise
{
    public partial class Products : System.Web.UI.Page
    {
        protected bool ShouldClearProductBuilderDataCache { get; private set; }
        private int SelectedRecipeProductId
        {
            get
            {
                object storedProductId = ViewState["SelectedRecipeProductId"];
                if (storedProductId == null)
                {
                    return 0;
                }
                return Convert.ToInt32(storedProductId);
            }
            set
            {
                ViewState["SelectedRecipeProductId"] = value;
            }
        }
        protected int EditingProductId
        {
            get
            {
                object storedProductId = ViewState["EditingProductId"];
                if (storedProductId == null)
                {
                    return 0;
                }
                return Convert.ToInt32(storedProductId);
            }
            set
            {
                ViewState["EditingProductId"] = value;
            }
        }
        private void UpdateProductFormMode()
        {
            bool isEditingProduct = EditingProductId > 0;
            ProductFormTitleLiteral.Text = isEditingProduct ? "עריכת מוצר" : "הוספת מוצר";
            AddProductButton.Text = isEditingProduct ? "שמור שינויים" : "צור מוצר";
            CancelProductEditButton.Visible = isEditingProduct;
        }
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
                LoadAvailableUnits();
                ShouldClearProductBuilderDataCache = string.Equals(Convert.ToString(Session["InvalidateProductBuilderDataCache"]), bool.TrueString, StringComparison.OrdinalIgnoreCase);
                Session.Remove("InvalidateProductBuilderDataCache");
                if (Session["ProductsMessage"] != null)
                {
                    ResultLabel.Text = Session["ProductsMessage"].ToString();
                    Session.Remove("ProductsMessage");
                }
            }
            LoadBusinessProductSettings();
            UpdateProductFormMode();
        }
        private void LoadBusinessProductSettings()
        {
            try
            {
                int userId = (int)Session["UserId"];
                Business business = BusinessBLL.GetBusinessForUser(userId);
                YieldUnitSelectionPanel.Visible = business.ShowYieldUnitSelection;
            }
            catch (ArgumentException)
            {
                YieldUnitSelectionPanel.Visible = false;
                AddProductButton.Enabled = false;
                ResultLabel.Text = "לא ניתן לטעון את הגדרות המוצרים.";
            }
            catch (InvalidOperationException)
            {
                YieldUnitSelectionPanel.Visible = false;
                AddProductButton.Enabled = false;
                ResultLabel.Text = "לא ניתן לטעון את הגדרות המוצרים.";
            }
            catch (Exception)
            {
                YieldUnitSelectionPanel.Visible = false;
                AddProductButton.Enabled = false;
                ResultLabel.Text = "אירעה שגיאה בעת טעינת הגדרות המוצרים.";
            }
        }
        private void LoadProducts()
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<Product> products = CostCalculationBLL.GetActiveProductsWithCurrentCosts(userId);
                ProductsGrid.DataSource = products;
                ProductsGrid.DataBind();
                UpdateRecipeSelectionState();
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
        private void UpdateRecipeSelectionState()
        {
            foreach (GridViewRow row in ProductsGrid.Rows)
            {
                LinkButton openRecipeButton = row.FindControl("OpenRecipeButton") as LinkButton;
                if (openRecipeButton == null)
                {
                    continue;
                }
                int rowProductId;
                bool isSelected = int.TryParse(openRecipeButton.CommandArgument, out rowProductId) && rowProductId == SelectedRecipeProductId;
                openRecipeButton.Attributes["aria-expanded"] = isSelected ? "true" : "false";
                row.Attributes["data-recipe-selected"] = isSelected ? "true" : "false";
            }
        }
        private void LoadProductRecipeDetails(int userId, int productId)
        {
            CostCalculationResult result = CostCalculationBLL.CalculateProductCost(userId, productId);
            if (result == null || result.Calculation == null || result.Items == null)
            {
                throw new InvalidOperationException("לא ניתן להציג את פרטי המתכון.");
            }
            CostCalculation calculation = result.Calculation;
            RecipeProductNameLabel.Text = Server.HtmlEncode(calculation.ProductNameSnapshot);
            RecipeYieldQuantityLabel.Text = calculation.YieldQuantitySnapshot.ToString("0.######");
            RecipeYieldUnitLabel.Text = Server.HtmlEncode(calculation.YieldUnitLabelSnapshot);
            RecipeTotalCostLabel.Text = calculation.TotalIngredientCostSnapshot.ToString("N2") + " ₪";
            RecipeTotalCostIncludingVatLabel.Text = calculation.TotalCostIncludingVat.ToString("N2") + " ₪";
            RecipeCostPerYieldUnitLabel.Text = calculation.CostPerYieldUnitSnapshot.ToString("N2") + " ₪";
            RecipeCostPerYieldUnitIncludingVatLabel.Text = calculation.CostPerYieldUnitIncludingVat.ToString("N2") + " ₪";
            ProductRecipeItemsGrid.DataSource = result.Items;
            ProductRecipeItemsGrid.DataBind();
            ProductRecipeDetailsPanel.Visible = true;
        }
        private void LoadAvailableUnits()
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
                YieldUnitDropDownList.DataSource = availableUnits;
                YieldUnitDropDownList.DataTextField = "UnitName";
                YieldUnitDropDownList.DataValueField = "MeasurementUnitId";
                YieldUnitDropDownList.DataBind();
                YieldUnitDropDownList.Items.Insert(0, new ListItem("בחר יחידת מידה", ""));
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
        private void LoadProductForEditing(int userId, int productId)
        {
            Product product = ProductBLL.GetActiveProductForUser(userId, productId);
            ProductNameTextBox.Text = product.ProductName;
            InstructionsHtmlTextBox.Text = product.InstructionsHtml ?? string.Empty;
            YieldQuantityTextBox.Text = product.YieldQuantity.ToString("0.######");
            ListItem yieldUnitItem = YieldUnitDropDownList.Items.FindByText(product.YieldUnitLabel);
            if (yieldUnitItem == null)
            {
                throw new InvalidOperationException("יחידת התוצר של המוצר אינה זמינה לעריכה.");
            }
            YieldUnitDropDownList.ClearSelection();
            yieldUnitItem.Selected = true;
        }
        private void LoadRecipeForEditing(int userId, int productId)
        {
            List<RecipeIngredient> recipeIngredients = RecipeIngredientBLL.GetRecipeIngredientsForProduct(userId, productId);
            if (recipeIngredients == null || recipeIngredients.Count == 0)
            {
                throw new InvalidOperationException("לא ניתן לערוך מוצר שאין לו מתכון.");
            }
            List<RecipeIngredientInput> recipeInputs = new List<RecipeIngredientInput>();
            foreach (RecipeIngredient recipeIngredient in recipeIngredients)
            {
                RecipeIngredientInput recipeInput = new RecipeIngredientInput
                {
                    IngredientId = recipeIngredient.IngredientId,
                    Quantity = recipeIngredient.Quantity,
                    MeasurementUnitId = recipeIngredient.MeasurementUnitId,
                    ManualIngredientCostOverride = recipeIngredient.ManualIngredientCostOverride
                };
                recipeInputs.Add(recipeInput);
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            RecipeIngredientsJsonHiddenField.Value = serializer.Serialize(recipeInputs);
        }
        protected void AddProductButton_Click(object sender, EventArgs e)
        {
            ResultLabel.Text = string.Empty;
            string productName = ProductNameTextBox.Text;
            string instructionsHtml = InstructionsHtmlTextBox.Text;
            decimal yieldQuantity;
            if (!decimal.TryParse(YieldQuantityTextBox.Text, out yieldQuantity))
            {
                ResultLabel.Text = "יש להזין כמות תוצר מספרית.";
                return;
            }
            int yieldUnitId = 0;
            if (YieldUnitSelectionPanel.Visible && !int.TryParse(YieldUnitDropDownList.SelectedValue, out yieldUnitId))
            {
                ResultLabel.Text = "יש לבחור יחידת תוצר.";
                return;
            }
            string recipeIngredientsJson = RecipeIngredientsJsonHiddenField.Value;
            if (string.IsNullOrWhiteSpace(recipeIngredientsJson))
            {
                ResultLabel.Text = "יש להוסיף לפחות רכיב אחד למתכון.";
                return;
            }
            List<RecipeIngredientInput> recipeIngredients;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                recipeIngredients = serializer.Deserialize<List<RecipeIngredientInput>>(recipeIngredientsJson);
            }
            catch (Exception)
            {
                ResultLabel.Text = "פרטי המתכון שהתקבלו אינם תקינים.";
                return;
            }
            if (recipeIngredients == null || recipeIngredients.Count == 0)
            {
                ResultLabel.Text = "יש להוסיף לפחות רכיב אחד למתכון.";
                return;
            }
            bool isEditingProduct = EditingProductId > 0;
            try
            {
                int userId = (int)Session["UserId"];
                if (isEditingProduct)
                {
                    ProductBLL.UpdateProductWithRecipe(userId, EditingProductId, productName, yieldQuantity, yieldUnitId, instructionsHtml, recipeIngredients);
                    Session["ProductsMessage"] = "המוצר והמתכון עודכנו בהצלחה.";
                }
                else
                {
                    ProductBLL.CreateProductWithRecipe(userId, productName, yieldQuantity, yieldUnitId, instructionsHtml, recipeIngredients);
                    Session["ProductsMessage"] = "המוצר והמתכון נוצרו בהצלחה.";
                }
                Response.Redirect("~/Products.aspx", false);
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
                ResultLabel.Text = isEditingProduct ? "אירעה שגיאה בעת עדכון המוצר." : "אירעה שגיאה בעת הוספת המוצר.";
            }
        }
        protected void CancelProductEditButton_Click(object sender, EventArgs e)
        {
            EditingProductId = 0;
            SelectedRecipeProductId = 0;
            ProductsGrid.EditIndex = -1;
            RecipeIngredientsJsonHiddenField.Value = string.Empty;
            Response.Redirect("~/Products.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
        protected void ProductsGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "OpenRecipe", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            int productId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out productId) || productId <= 0)
            {
                ResultLabel.Text = "לא ניתן לזהות את המוצר המבוקש.";
                return;
            }
            if (SelectedRecipeProductId == productId)
            {
                SelectedRecipeProductId = 0;
                ProductRecipeDetailsPanel.Visible = false;
                UpdateRecipeSelectionState();
                ResultLabel.Text = string.Empty;
                return;
            }
            SelectedRecipeProductId = 0;
            ProductRecipeDetailsPanel.Visible = false;
            try
            {
                int userId = (int)Session["UserId"];
                LoadProductRecipeDetails(userId, productId);
                SelectedRecipeProductId = productId;
                UpdateRecipeSelectionState();
                ResultLabel.Text = string.Empty;
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
                ResultLabel.Text = "אירעה שגיאה בעת טעינת פרטי המתכון.";
            }
        }
        protected void ProductsGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            ResultLabel.Text = string.Empty;
            DataKey productDataKey = ProductsGrid.DataKeys[e.NewEditIndex];
            if (productDataKey == null)
            {
                e.Cancel = true;
                ResultLabel.Text = "לא ניתן לזהות את המוצר לעריכה.";
                return;
            }
            int productId;
            if (!int.TryParse(Convert.ToString(productDataKey.Values["ProductId"]), out productId))
            {
                e.Cancel = true;
                ResultLabel.Text = "מזהה המוצר לעריכה אינו תקין.";
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                LoadProductForEditing(userId, productId);
                LoadRecipeForEditing(userId, productId);
                EditingProductId = productId;
                UpdateProductFormMode();
                SelectedRecipeProductId = 0;
                ProductRecipeDetailsPanel.Visible = false;
                e.Cancel = true;
                ProductsGrid.EditIndex = -1;
                UpdateRecipeSelectionState();
            }
            catch (ArgumentException ex)
            {
                e.Cancel = true;
                EditingProductId = 0;
                ProductsGrid.EditIndex = -1;
                RecipeIngredientsJsonHiddenField.Value = string.Empty;
                ResultLabel.Text = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                e.Cancel = true;
                EditingProductId = 0;
                ProductsGrid.EditIndex = -1;
                RecipeIngredientsJsonHiddenField.Value = string.Empty;
                ResultLabel.Text = ex.Message;
            }
            catch (Exception)
            {
                e.Cancel = true;
                EditingProductId = 0;
                ProductsGrid.EditIndex = -1;
                RecipeIngredientsJsonHiddenField.Value = string.Empty;
                ResultLabel.Text = "אירעה שגיאה בעת טעינת המוצר לעריכה.";
            }
        }
        protected void ProductsGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int productId = Convert.ToInt32(ProductsGrid.DataKeys[e.RowIndex].Values["ProductId"]);
            int userId = (int)Session["UserId"];
            try
            {
                ProductBLL.DeactivateProduct(userId, productId);
                Session["ProductsMessage"] = "המוצר הושבת בהצלחה.";
            }
            catch (ArgumentException ex)
            {
                Session["ProductsMessage"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                Session["ProductsMessage"] = ex.Message;
            }
            catch (Exception)
            {
                Session["ProductsMessage"] = "אירעה שגיאה בעת השבתת המוצר.";
            }
            Response.Redirect("~/Products.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}