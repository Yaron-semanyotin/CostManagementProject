using System;
using CostWise.App_Code.BLL;

namespace CostWise
{
    public partial class DatabaseConnectionTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnectionTestBLL.TestConnection();
                ResultLabel.Text = "החיבור למסד הנתונים הצליח.";
            }
            catch (Exception ex)
            {
                ResultLabel.Text = "החיבור למסד הנתונים נכשל: " + Server.HtmlEncode(ex.Message);
            }
        }
    }
}