using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CostWise.App_Code.BLL;

namespace CostWise
{
    public partial class ProductRecycleBin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["BusinessId"] == null || Session["UserName"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            if (!IsPostBack)
            {
                LoadInactiveProducts();
                if (Session["ProductRecycleBinMessage"] != null)
                {
                    ResultLabel.Text = Session["ProductRecycleBinMessage"].ToString();
                    Session.Remove("ProductRecycleBinMessage");
                }
            }
        }
        private void LoadInactiveProducts()
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<Product> inactiveProducts = ProductBLL.GetInactiveProductsForUser(userId);
                ProductRecycleBinGrid.DataSource = inactiveProducts;
                ProductRecycleBinGrid.DataBind();
                ResultLabel.Text = string.Empty;
            }
            catch (ArgumentException ex)
            {
                ResultLabel.Text = ex.Message;
            }
            catch (Exception)
            {
                ResultLabel.Text = "אירעה שגיאה בעת טעינת סל המחזור.";
            }
        }
        protected void ProductRecycleBinGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "RestoreProduct", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            int rowIndex;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out rowIndex) || rowIndex < 0 || rowIndex >= ProductRecycleBinGrid.DataKeys.Count)
            {
                ResultLabel.Text = "לא ניתן לזהות את המוצר לשחזור.";
                return;
            }
            int productId = Convert.ToInt32(ProductRecycleBinGrid.DataKeys[rowIndex].Value);
            int userId = (int)Session["UserId"];
            try
            {
                ProductBLL.RestoreProduct(userId, productId);
                Session["ProductRecycleBinMessage"] = "המוצר הוחזר לרשימת המוצרים.";
            }
            catch (ArgumentException ex)
            {
                Session["ProductRecycleBinMessage"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                Session["ProductRecycleBinMessage"] = ex.Message;
            }
            catch (Exception)
            {
                Session["ProductRecycleBinMessage"] = "אירעה שגיאה בעת שחזור המוצר.";
            }
            Response.Redirect("~/ProductRecycleBin.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
    }
}