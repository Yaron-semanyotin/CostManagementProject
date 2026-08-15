using CostWise.App_Code.BLL;
using System;
using System.IO;
using System.Web.UI.WebControls;

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
        private void LoadBusiness()
        {
            try
            {
                int userId = (int)Session["UserId"];
                Business business = BusinessBLL.GetBusinessForUser(userId);
                BusinessNameTextBox.Text = business.BusinessName;
                ShowBusinessLogo(business.LogoPath);
                LoadBusinessSettings(userId, business);
            }
            catch (Exception)
            {
                ResultLabel.Text = "לא ניתן לטעון את פרטי העסק.";
                ResultLabel.CssClass = "d-block mt-3 text-danger";
                BusinessNameTextBox.Enabled = false;
                SaveBusinessButton.Enabled = false;
                BusinessSettingsPanel.Enabled = false;
            }
        }
        private void LoadBusinessSettings(int userId, Business business)
        {
            if (business == null)
            {
                throw new ArgumentNullException(nameof(business));
            }
            var availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            DefaultRecipeMeasurementUnitDropDownList.Items.Clear();
            DefaultRecipeMeasurementUnitDropDownList.DataSource = availableUnits;
            DefaultRecipeMeasurementUnitDropDownList.DataTextField = "UnitName";
            DefaultRecipeMeasurementUnitDropDownList.DataValueField = "MeasurementUnitId";
            DefaultRecipeMeasurementUnitDropDownList.DataBind();
            DefaultRecipeMeasurementUnitDropDownList.Items.Insert(0, new ListItem("ללא יחידת ברירת מחדל", string.Empty));
            ShowYieldUnitSelectionSwitch.Checked = business.ShowYieldUnitSelection;
            VatRatePercentTextBox.Text = business.VatRatePercent.ToString("0.##");
            if (!business.DefaultRecipeMeasurementUnitId.HasValue)
            {
                return;
            }
            ListItem selectedUnitItem = DefaultRecipeMeasurementUnitDropDownList.Items.FindByValue(business.DefaultRecipeMeasurementUnitId.Value.ToString());
            if (selectedUnitItem == null)
            {
                throw new InvalidOperationException("יחידת ברירת המחדל השמורה אינה זמינה לעסק.");
            }
            DefaultRecipeMeasurementUnitDropDownList.ClearSelection();
            selectedUnitItem.Selected = true;
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
            int? defaultRecipeMeasurementUnitId = null;
            if (!string.IsNullOrWhiteSpace(DefaultRecipeMeasurementUnitDropDownList.SelectedValue))
            {
                int parsedMeasurementUnitId;
                if (!int.TryParse(DefaultRecipeMeasurementUnitDropDownList.SelectedValue, out parsedMeasurementUnitId))
                {
                    ResultLabel.Text = "יחידת ברירת המחדל שנבחרה אינה תקינה.";
                    ResultLabel.CssClass = "d-block mt-3 text-danger";
                    return;
                }
                defaultRecipeMeasurementUnitId = parsedMeasurementUnitId;
            }
            decimal vatRatePercent;
            if (!decimal.TryParse(VatRatePercentTextBox.Text, out vatRatePercent))
            {
                ResultLabel.Text = "יש להזין שיעור מע״מ מספרי.";
                ResultLabel.CssClass = "d-block mt-3 text-danger";
                return;
            }
            try
            {
                int userId = (int)Session["UserId"];
                BusinessBLL.UpdateBusinessName(userId, BusinessNameTextBox.Text);
                BusinessBLL.UpdateBusinessSettings(userId, ShowYieldUnitSelectionSwitch.Checked, defaultRecipeMeasurementUnitId, vatRatePercent);
                SaveBusinessLogoIfSelected(userId);
                Session["InvalidateProductBuilderDataCache"] = true;
                Session["BusinessProfileMessage"] = "השינויים נשמרו בהצלחה.";
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
                ResultLabel.Text = "לא ניתן לשמור את השינויים.";
                ResultLabel.CssClass = "d-block mt-3 text-danger";
            }
            catch (Exception)
            {
                ResultLabel.Text = "אירעה שגיאה בעת שמירת השינויים.";
                ResultLabel.CssClass = "d-block mt-3 text-danger";
            }
        }
        private void SaveBusinessLogoIfSelected(int userId)
        {
            if (!BusinessLogoFileUpload.HasFile)
            {
                return;
            }
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