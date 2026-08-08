using CostWise.App_Code.BLL;
using System;
using System.IO;
namespace CostWise
{
    public partial class BusinessProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["UserId"] is int) || !(Session["BusinessId"] is int) || !(Session["UserName"] is string))
            {
                return;
            }
            if (!IsPostBack)
            {
                ShowTemporaryMessage();
                ShowTemporaryLogoMessage();
                LoadBusiness();
            }
        }
        private void ShowTemporaryMessage()
        {
            if (Session["BusinessProfileMessage"] == null)
            {
                return;
            }
            ResultLabel.Text = Server.HtmlEncode(Session["BusinessProfileMessage"].ToString());
            ResultLabel.CssClass = "d-block mt-3 text-success";
            Session.Remove("BusinessProfileMessage");
        }
        private void ShowTemporaryLogoMessage()
        {
            if (Session["BusinessProfileLogoMessage"] == null)
            {
                return;
            }
            LogoResultLabel.Text = Server.HtmlEncode(Session["BusinessProfileLogoMessage"].ToString());
            LogoResultLabel.CssClass = "d-block mt-3 text-success";
            Session.Remove("BusinessProfileLogoMessage");
        }
        private void LoadBusiness()
        {
            try
            {
                int userId = (int)Session["UserId"];
                Business business = BusinessBLL.GetBusinessForUser(userId);
                BusinessNameTextBox.Text = business.BusinessName;
                ShowBusinessLogo(business.LogoPath);
            }
            catch (Exception)
            {
                ResultLabel.Text = "לא ניתן לטעון את פרטי העסק.";
                ResultLabel.CssClass = "d-block mt-3 text-danger";
                BusinessNameTextBox.Enabled = false;
                SaveBusinessButton.Enabled = false;
            }
        }
        protected void SaveBusinessButton_Click(object sender, EventArgs e)
        {
            ResultLabel.Text = string.Empty;
            Page.Validate("BusinessProfile");
            if (!Page.IsValid)
            {
                return;
            }
            if (!(Session["UserId"] is int) || !(Session["BusinessId"] is int) || !(Session["UserName"] is string))
            {
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                BusinessBLL.UpdateBusinessName(userId, BusinessNameTextBox.Text);
                Session["BusinessProfileMessage"] = "שם העסק עודכן בהצלחה.";
                Response.Redirect("~/BusinessProfile.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (ArgumentException ex)
            {
                ResultLabel.Text = Server.HtmlEncode(ex.Message);
                ResultLabel.CssClass = "d-block mt-3 text-danger";
            }
            catch (InvalidOperationException)
            {
                ResultLabel.Text = "לא ניתן לעדכן את העסק.";
                ResultLabel.CssClass = "d-block mt-3 text-danger";
            }
            catch (Exception)
            {
                ResultLabel.Text = "אירעה שגיאה בעת עדכון העסק.";
                ResultLabel.CssClass = "d-block mt-3 text-danger";
            }
        }
        protected void UploadLogoButton_Click(object sender, EventArgs e)
        {
            LogoResultLabel.Text = string.Empty;
            if (!(Session["UserId"] is int) || !(Session["BusinessId"] is int) || !(Session["UserName"] is string))
            {
                return;
            }
            if (!BusinessLogoFileUpload.HasFile)
            {
                ShowLogoError("יש לבחור קובץ לוגו.");
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                Business currentBusiness = BusinessBLL.GetBusinessForUser(userId);
                string originalExtension = Path.GetExtension(BusinessLogoFileUpload.FileName);
                byte[] fileHeader = new byte[8];
                Stream inputStream = BusinessLogoFileUpload.PostedFile.InputStream;
                int headerBytesRead = inputStream.Read(fileHeader, 0, fileHeader.Length);
                if (headerBytesRead < fileHeader.Length)
                {
                    Array.Resize(ref fileHeader, headerBytesRead);
                }
                if (inputStream.CanSeek)
                {
                    inputStream.Position = 0;
                }
                string safeExtension = BusinessBLL.ValidateBusinessLogoUpload(userId, originalExtension, BusinessLogoFileUpload.PostedFile.ContentLength, fileHeader);
                string safeFileName = Guid.NewGuid().ToString("N") + safeExtension;
                string uploadDirectoryPath = Path.GetFullPath(Server.MapPath("~/Uploads/BusinessLogos"));
                Directory.CreateDirectory(uploadDirectoryPath);
                string allowedDirectoryPrefix = uploadDirectoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
                string physicalFilePath = Path.GetFullPath(Path.Combine(uploadDirectoryPath, safeFileName));
                if (!physicalFilePath.StartsWith(allowedDirectoryPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("מיקום שמירת הלוגו אינו תקין.");
                }
                BusinessLogoFileUpload.SaveAs(physicalFilePath);
                string relativeLogoPath = "Uploads/BusinessLogos/" + safeFileName;
                try
                {
                    BusinessBLL.UpdateBusinessLogoPath(userId, relativeLogoPath);
                }
                catch
                {
                    TryDeleteFile(physicalFilePath);
                    throw;
                }
                TryDeleteOldBusinessLogo(currentBusiness.LogoPath, physicalFilePath);
                Session["BusinessProfileLogoMessage"] = "לוגו העסק הועלה בהצלחה.";
                Response.Redirect("~/BusinessProfile.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (ArgumentException ex)
            {
                ShowLogoError(ex.Message);
            }
            catch (InvalidOperationException)
            {
                ShowLogoError("לא ניתן לעדכן את לוגו העסק.");
            }
            catch (IOException)
            {
                ShowLogoError("אירעה שגיאה בעת שמירת קובץ הלוגו.");
            }
            catch (UnauthorizedAccessException)
            {
                ShowLogoError("אין הרשאה לשמור את קובץ הלוגו.");
            }
            catch (Exception)
            {
                ShowLogoError("אירעה שגיאה בעת העלאת לוגו העסק.");
            }
        }
        private void ShowLogoError(string message)
        {
            LogoResultLabel.Text = Server.HtmlEncode(message);
            LogoResultLabel.CssClass = "d-block mt-3 text-danger";
        }
        private static void TryDeleteFile(string physicalFilePath)
        {
            try
            {
                if (File.Exists(physicalFilePath))
                {
                    File.Delete(physicalFilePath);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
        private void TryDeleteOldBusinessLogo(string oldLogoPath, string newPhysicalFilePath)
        {
            if (string.IsNullOrWhiteSpace(oldLogoPath))
            {
                return;
            }
            string normalizedOldLogoPath = oldLogoPath.Trim().Replace('\\', '/');
            if (!normalizedOldLogoPath.StartsWith("Uploads/BusinessLogos/", StringComparison.Ordinal) || normalizedOldLogoPath.Contains("..") || normalizedOldLogoPath.Contains(":"))
            {
                return;
            }
            try
            {
                string uploadDirectoryPath = Path.GetFullPath(Server.MapPath("~/Uploads/BusinessLogos"));
                string oldPhysicalFilePath = Path.GetFullPath(Server.MapPath("~/" + normalizedOldLogoPath));
                string allowedDirectoryPrefix = uploadDirectoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
                if (!oldPhysicalFilePath.StartsWith(allowedDirectoryPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                if (string.Equals(oldPhysicalFilePath, newPhysicalFilePath, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                TryDeleteFile(oldPhysicalFilePath);
            }
            catch (ArgumentException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
        private void ShowBusinessLogo(string logoPath)
        {
            BusinessLogoImage.Visible = false;
            BusinessLogoFallbackPanel.Visible = true;
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
                BusinessLogoImage.ImageUrl = ResolveUrl("~/" + normalizedLogoPath);
                BusinessLogoImage.Visible = true;
                BusinessLogoFallbackPanel.Visible = false;
            }
            catch (Exception)
            {
                BusinessLogoImage.Visible = false;
                BusinessLogoFallbackPanel.Visible = true;
            }
        }
    }
}