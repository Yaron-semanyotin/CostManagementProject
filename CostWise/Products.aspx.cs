using CostWise.App_Code.BLL;
using CostWise.App_Code.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace CostWise
{
    public partial class Products : System.Web.UI.Page
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
                LoadAvailableUnits();
                if (Session["ProductsMessage"] != null)
                {
                    ResultLabel.Text = Session["ProductsMessage"].ToString();
                    Session.Remove("ProductsMessage");
                }
            }
        }
        private void LoadProducts()
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<Product> products = ProductBLL.GetProductsForUser(userId);
                ProductsGrid.DataSource = products;
                ProductsGrid.DataBind();
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
        protected void AddProductButton_Click(object sender, EventArgs e)
        {
            ResultLabel.Text = string.Empty;
            string productName = ProductNameTextBox.Text;
            decimal yieldQuantity;
            if (!decimal.TryParse(YieldQuantityTextBox.Text, out yieldQuantity))
            {
                ResultLabel.Text = "יש להזין כמות תוצר מספרית.";
                return;
            }
            int yieldUnitId;
            if (!int.TryParse(YieldUnitDropDownList.SelectedValue, out yieldUnitId))
            {
                ResultLabel.Text = "יש לבחור יחידת תוצר.";
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                int createdProductId = ProductBLL.CreateProduct(userId, productName, yieldQuantity, yieldUnitId);
                Session["RecipeMessage"] = "המוצר נוסף בהצלחה. כעת ניתן להוסיף רכיבים למתכון.";
                Response.Redirect("~/Recipe.aspx?productId=" + createdProductId, false);
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
                ResultLabel.Text = "אירעה שגיאה בעת הוספת המוצר.";
            }
        }
        protected void ProductsGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            ResultLabel.Text = string.Empty;
            ProductsGrid.EditIndex = e.NewEditIndex;
            LoadProducts();
        }
        protected void ProductsGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            ResultLabel.Text = string.Empty;
            ProductsGrid.EditIndex = -1;
            LoadProducts();
        }
        protected void ProductsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }
            bool isActive = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "IsActive"));
            bool isEditRow = (e.Row.RowState & DataControlRowState.Edit) != 0;
            if (!isActive && !isEditRow)
            {
                TableCell commandCell = e.Row.Cells[e.Row.Cells.Count - 1];
                foreach (Control control in commandCell.Controls)
                {
                    LinkButton actionButton = control as LinkButton;
                    if (actionButton == null)
                    {
                        continue;
                    }
                    if (string.Equals(actionButton.CommandName, "Edit", StringComparison.OrdinalIgnoreCase))
                    {
                        actionButton.Text = "הפעל מחדש";
                    }
                    else if (string.Equals(actionButton.CommandName, "Delete", StringComparison.OrdinalIgnoreCase))
                    {
                        actionButton.Visible = false;
                    }
                }
                return;
            }
            if (!isEditRow)
            {
                return;
            }
            DropDownList editYieldUnitDropDownList = (DropDownList)e.Row.FindControl("EditYieldUnitDropDownList");
            if (editYieldUnitDropDownList == null)
            {
                return;
            }
            int userId = (int)Session["UserId"];
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            editYieldUnitDropDownList.DataSource = availableUnits;
            editYieldUnitDropDownList.DataTextField = "UnitName";
            editYieldUnitDropDownList.DataValueField = "MeasurementUnitId";
            editYieldUnitDropDownList.DataBind();
            string currentYieldUnitLabel = DataBinder.Eval(e.Row.DataItem, "YieldUnitLabel")?.ToString();
            ListItem currentUnitItem = editYieldUnitDropDownList.Items.FindByText(currentYieldUnitLabel);
            if (currentUnitItem != null)
            {
                currentUnitItem.Selected = true;
            }
        }
        protected void ProductsGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            ResultLabel.Text = string.Empty;
            int productId = Convert.ToInt32(ProductsGrid.DataKeys[e.RowIndex].Values["ProductId"]);
            bool isActive = Convert.ToBoolean(ProductsGrid.DataKeys[e.RowIndex].Values["IsActive"]);
            string productName = e.NewValues["ProductName"]?.ToString();
            decimal yieldQuantity;
            if (!decimal.TryParse(e.NewValues["YieldQuantity"]?.ToString(), out yieldQuantity))
            {
                e.Cancel = true;
                ResultLabel.Text = "יש להזין כמות תוצר מספרית.";
                return;
            }
            GridViewRow row = ProductsGrid.Rows[e.RowIndex];
            DropDownList editYieldUnitDropDownList = (DropDownList)row.FindControl("EditYieldUnitDropDownList");
            if (editYieldUnitDropDownList == null)
            {
                e.Cancel = true;
                ResultLabel.Text = "לא ניתן לקרוא את יחידת התוצר שנבחרה.";
                return;
            }
            int yieldUnitId;
            if (!int.TryParse(editYieldUnitDropDownList.SelectedValue, out yieldUnitId))
            {
                e.Cancel = true;
                ResultLabel.Text = "יש לבחור יחידת תוצר תקינה.";
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                if (isActive)
                {
                    ProductBLL.UpdateProduct(userId, productId, productName, yieldQuantity, yieldUnitId);
                    Session["ProductsMessage"] = "המוצר עודכן בהצלחה.";
                }
                else
                {
                    ProductBLL.ReactivateProduct(userId, productId, productName, yieldQuantity, yieldUnitId);
                    Session["ProductsMessage"] = "המוצר הופעל מחדש בהצלחה.";
                }
                Response.Redirect("~/Products.aspx", false);
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
                ResultLabel.Text = isActive ? "אירעה שגיאה בעת עדכון המוצר." : "אירעה שגיאה בעת הפעלה מחדש של המוצר.";
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