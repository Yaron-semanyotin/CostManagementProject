using System;
using CostWise.App_Code.BLL;
using System.Collections.Generic;

namespace CostWise
{
    public partial class CalculationHistory : System.Web.UI.Page
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
                LoadCalculations();
            }
        }
        private void LoadCalculations()
        {
            try
            {
                int userId = (int)Session["UserId"];
                List<CostCalculation> calculations = CostCalculationBLL.GetCalculationHistory(userId);
                CalculationsGrid.DataSource = calculations;
                CalculationsGrid.DataBind();
                ResultLabel.Text = string.Empty;
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
    }
}