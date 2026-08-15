using CostWise.App_Code.BLL;
using System;
using System.IO;
using System.Web.Security;

namespace CostWise
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            RestoreSessionFromAuthenticationCookie();
        }
        private void RestoreSessionFromAuthenticationCookie()
        {
            bool hasCompleteSession =
                Session["UserId"] is int
                &&
                Session["BusinessId"] is int
                &&
                Session["UserName"] is string;
            if (hasCompleteSession)
            {
                return;
            }
            FormsIdentity formsIdentity = Context.User?.Identity as FormsIdentity;
            if (formsIdentity == null || !formsIdentity.IsAuthenticated)
            {
                return;
            }
            try
            {
                User user = AuthenticationBLL.GetUserForAuthenticatedIdentity(formsIdentity.Name);

                if (user == null)
                {
                    Session.Clear();
                    FormsAuthentication.SignOut();
                    return;
                }
                Session["UserId"] = user.UserId;
                Session["BusinessId"] = user.BusinessId;
                Session["UserName"] = user.Username;
            }
            catch (Exception)
            {
                Session.Clear();
                FormsAuthentication.SignOut();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["BusinessId"] == null || Session["UserName"] == null)
            {
                RedirectToLogin();
                return;
            }
            if (!IsPostBack)
            {
                int userId = (int)Session["UserId"];
                LoadBusinessName(userId);
            }
        }
        private void LoadBusinessName(int userId)
        {
            try
            {
                Business business = BusinessBLL.GetBusinessForUser(userId);
                BusinessNameLabel.Text = Server.HtmlEncode(business.BusinessName);
                ShowBusinessLogo(business.LogoPath);
            }
            catch (ArgumentException)
            {
                RedirectToLogin();
            }
            catch (InvalidOperationException)
            {
                RedirectToLogin();
            }
            catch (Exception)
            {
                BusinessNameLabel.Text = "לא ניתן לטעון את שם העסק";
            }
        }
        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            RedirectToLogin();
        }
        protected string GetNavigationLinkCssClass(string pageVirtualPath)
        {
            const string baseCssClass = "nav-link rounded px-3 py-2";
            bool isCurrentPage = string.Equals(Request.AppRelativeCurrentExecutionFilePath, pageVirtualPath, StringComparison.OrdinalIgnoreCase);
            if (isCurrentPage)
            {
                return baseCssClass + " active fw-semibold";
            }
            return baseCssClass + " text-dark";
        }
        protected bool IsIngredientsSectionCurrent()
        {
            string currentPagePath = Request.AppRelativeCurrentExecutionFilePath;
            return
                string.Equals(currentPagePath, "~/Ingredients.aspx", StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(currentPagePath, "~/IngredientRecycleBin.aspx", StringComparison.OrdinalIgnoreCase);
        }
        protected bool IsProductsSectionCurrent()
        {
            string currentPagePath = Request.AppRelativeCurrentExecutionFilePath;
            return
                string.Equals(currentPagePath, "~/Products.aspx", StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(currentPagePath, "~/ProductRecycleBin.aspx", StringComparison.OrdinalIgnoreCase);
        }
        private void RedirectToLogin()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
        private void ShowBusinessLogo(string logoPath)
        {
            TopBusinessLogoImage.Visible = false;
            TopBusinessLogoFallbackPanel.Visible = true;
            if (string.IsNullOrWhiteSpace(logoPath))
            {
                return;
            }
            string normalizedLogoPath = logoPath.Trim().Replace('\\', '/');
            if (!normalizedLogoPath.StartsWith("Uploads/BusinessLogos/", StringComparison.Ordinal) || normalizedLogoPath.Contains("..") || normalizedLogoPath.Contains(":"))
            {
                return;
            }
            try
            {
                string uploadDirectoryPath = Path.GetFullPath(Server.MapPath("~/Uploads/BusinessLogos"));
                string physicalLogoPath = Path.GetFullPath(Server.MapPath("~/" + normalizedLogoPath));
                string allowedDirectoryPrefix = uploadDirectoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
                if (!physicalLogoPath.StartsWith(allowedDirectoryPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                if (!File.Exists(physicalLogoPath))
                {
                    return;
                }
                TopBusinessLogoImage.ImageUrl = ResolveUrl("~/" + normalizedLogoPath);
                TopBusinessLogoImage.Visible = true;
                TopBusinessLogoFallbackPanel.Visible = false;
            }
            catch (Exception)
            {
                TopBusinessLogoImage.Visible = false;
                TopBusinessLogoFallbackPanel.Visible = true;
            }
        }
    }
}