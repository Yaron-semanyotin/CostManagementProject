using CostWise.App_Code.BLL;
using System;
using System.IO;
namespace CostWise
{
    public partial class Site : System.Web.UI.MasterPage
    {
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
        private void RedirectToLogin()
        {
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