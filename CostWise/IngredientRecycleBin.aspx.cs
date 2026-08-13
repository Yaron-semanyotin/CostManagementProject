using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CostWise.App_Code.BLL;
namespace CostWise
{
    public partial class IngredientRecycleBin : System.Web.UI.Page
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
                LoadRecycleBinIngredients();
                if (Session["IngredientRecycleBinMessage"] != null)
                {
                    ResultLabel.Text = Session["IngredientRecycleBinMessage"].ToString();
                    Session.Remove("IngredientRecycleBinMessage");
                }
            }
        }
        private void LoadRecycleBinIngredients()
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<Ingredient> inactiveIngredients = IngredientBLL.GetInactiveIngredientsForUser(userId);
                List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId
                    );
                var recycleBinRows = inactiveIngredients.Select(ingredient =>
                    {
                        MeasurementUnit packageUnit = availableUnits.Find(unit => unit.MeasurementUnitId == ingredient.PackageUnitId);
                        return new
                        {
                            ingredient.IngredientId,
                            ingredient.IngredientName,
                            ingredient.PackagePrice,
                            ingredient.PackageQuantity,
                            PackageUnitName = packageUnit == null ? "יחידה לא זמינה" : packageUnit.UnitName
                        };
                    }).ToList();
                RecycleBinGrid.DataSource = recycleBinRows;
                RecycleBinGrid.DataBind();
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
        protected void RecycleBinGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "RestoreIngredient", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            int rowIndex;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out rowIndex) || rowIndex < 0 || rowIndex >= RecycleBinGrid.DataKeys.Count)
            {
                ResultLabel.Text = "לא ניתן לזהות את הרכיב לשחזור.";
                return;
            }
            int ingredientId = Convert.ToInt32(RecycleBinGrid.DataKeys[rowIndex].Value);
            int userId = (int)Session["UserId"];
            try
            {
                IngredientBLL.RestoreIngredient(userId, ingredientId);
                Session["InvalidateProductBuilderDataCache"] = true;
                Session["IngredientRecycleBinMessage"] = "הרכיב הוחזר לרשימת הרכיבים.";
            }
            catch (ArgumentException ex)
            {
                Session["IngredientRecycleBinMessage"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                Session["IngredientRecycleBinMessage"] = ex.Message;
            }
            catch (Exception)
            {
                Session["IngredientRecycleBinMessage"] = "אירעה שגיאה בעת שחזור הרכיב.";
            }
            Response.Redirect("~/IngredientRecycleBin.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
    }
}