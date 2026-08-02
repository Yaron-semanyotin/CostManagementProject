using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CostWise.App_Code.BLL;

namespace CostWise
{
    public partial class MeasurementUnitTest : System.Web.UI.Page
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
                try
                {
                    int userId = (int)Session["UserId"];
                    List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
                    ResultLabel.Text = "מספר היחידות הזמינות: " + availableUnits.Count;
                    AvailableUnitsGrid.DataSource = availableUnits;
                    AvailableUnitsGrid.DataBind();
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
        }
    }
}