using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CostWise.App_Code.BLL;
using System.Web.Security;

namespace CostWise
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSavedUsername();
            }
        }
        private void LoadSavedUsername()
        {
            HttpCookie savedUsernameCookie = Request.Cookies["CostWiseSavedUsername"];

            if (savedUsernameCookie == null)
            {
                return;
            }

            string savedUsername = HttpUtility.UrlDecode(savedUsernameCookie.Value);

            if (string.IsNullOrWhiteSpace(savedUsername) || savedUsername.Length > 50)
            {
                return;
            }

            UsernameTextBox.Text = savedUsername;
            RememberDetailsCheckBox.Checked = true;
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
                UpdateSavedUsernameCookie(user.Username);
                UpdatePersistentAuthenticationCookie(user.Username);
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
        private void UpdateSavedUsernameCookie(string username)
        {
            HttpCookie savedUsernameCookie = new HttpCookie("CostWiseSavedUsername");

            savedUsernameCookie.HttpOnly = true;
            savedUsernameCookie.SameSite = SameSiteMode.Lax;
            savedUsernameCookie.Secure = Request.IsSecureConnection;
            savedUsernameCookie.Path = string.IsNullOrEmpty(Request.ApplicationPath) ? "/" : Request.ApplicationPath;

            if (RememberDetailsCheckBox.Checked)
            {
                savedUsernameCookie.Value = HttpUtility.UrlEncode(username);
                savedUsernameCookie.Expires = DateTime.UtcNow.AddDays(30);
            }
            else
            {
                savedUsernameCookie.Value = string.Empty;
                savedUsernameCookie.Expires = DateTime.UtcNow.AddDays(-1);
            }

            Response.Cookies.Add(savedUsernameCookie);
        }
        private void UpdatePersistentAuthenticationCookie(string username)
        {
            FormsAuthentication.SignOut();

            if (!KeepSignedInCheckBox.Checked)
            {
                return;
            }

            FormsAuthentication.SetAuthCookie(username, true);
        }
    }
}