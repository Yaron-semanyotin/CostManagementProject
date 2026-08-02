using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CostWise.App_Code.BLL;

namespace CostWise
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void LoginButton_Click(object sender, EventArgs e)
        {
            LoginResultLabel.Text = string.Empty;
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;
            try
            {
                User user = AuthenticationBLL.Authenticate(username, password);
                if (user == null)
                {
                    LoginResultLabel.Text = "שם המשתמש או הסיסמה שגויים.";
                    return;
                }
                Session["UserId"] = user.UserId;
                Session["BusinessId"] = user.BusinessId;
                Session["UserName"] = user.Username;
                Response.Redirect("~/Dashboard.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (ArgumentException ex)
            {
                LoginResultLabel.Text = ex.Message;
            }
            catch (Exception)
            {
                LoginResultLabel.Text = "אירעה שגיאה בעת ההתחברות. נסה שוב.";
            }
        }
    }
}