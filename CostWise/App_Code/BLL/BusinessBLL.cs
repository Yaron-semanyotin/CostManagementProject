using CostWise.App_Code.DAL;
using System;

namespace CostWise.App_Code.BLL
{
    public static class BusinessBLL
    {
        public const int MaxBusinessLogoFileSizeBytes = 2 * 1024 * 1024;
        public static string ValidateBusinessLogoUpload(int userId, string fileExtension, int contentLength, byte[] fileHeader)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            fileExtension = fileExtension?.Trim().ToLowerInvariant();
            bool hasAllowedExtension = fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png";
            if (!hasAllowedExtension)
            {
                throw new ArgumentException("ניתן להעלות רק קובצי JPG או PNG.");
            }
            if (contentLength <= 0)
            {
                throw new ArgumentException("קובץ הלוגו ריק.");
            }
            if (contentLength > MaxBusinessLogoFileSizeBytes)
            {
                throw new ArgumentException("גודל קובץ הלוגו יכול להיות עד 2MB.");
            }
            if (fileHeader == null || fileHeader.Length < 8)
            {
                throw new ArgumentException("תוכן קובץ הלוגו אינו תקין.");
            }
            bool isJpeg = fileHeader[0] == 0xFF && fileHeader[1] == 0xD8 && fileHeader[2] == 0xFF;
            bool isPng = fileHeader[0] == 0x89 && fileHeader[1] == 0x50 && fileHeader[2] == 0x4E && fileHeader[3] == 0x47 && fileHeader[4] == 0x0D && fileHeader[5] == 0x0A && fileHeader[6] == 0x1A && fileHeader[7] == 0x0A;
            bool extensionMatchesContent = (fileExtension == ".png" && isPng) || ((fileExtension == ".jpg" || fileExtension == ".jpeg") && isJpeg);
            if (!extensionMatchesContent)
            {
                throw new ArgumentException("סיומת הקובץ אינה תואמת לתוכן התמונה.");
            }
            if (fileExtension == ".jpeg")
            {
                return ".jpg";
            }
            return fileExtension;
        }
        public static Business GetBusinessForUser(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            Business business = BusinessDAL.GetBusinessByUserId(userId);
            if (business == null)
            {
                throw new InvalidOperationException("לא נמצא עסק עבור המשתמש הנוכחי.");
            }
            return business;
        }
        public static void UpdateBusinessName(int userId, string businessName)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            businessName = businessName?.Trim();
            if (string.IsNullOrWhiteSpace(businessName))
            {
                throw new ArgumentException("שם העסק הוא שדה חובה.");
            }
            if (businessName.Length > 150)
            {
                throw new ArgumentException("שם העסק יכול להכיל עד 150 תווים.");
            }
            bool wasUpdated = BusinessDAL.UpdateBusinessName(userId, businessName);
            if (!wasUpdated)
            {
                throw new InvalidOperationException("לא ניתן לעדכן את העסק עבור המשתמש הנוכחי.");
            }
        }
        public static void UpdateBusinessSettings(int userId, bool showYieldUnitSelection, int? defaultRecipeMeasurementUnitId, decimal vatRatePercent)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (vatRatePercent < 0 || vatRatePercent > 100)
            {
                throw new ArgumentException("שיעור המע״מ חייב להיות בין 0 ל־100.");
            }
            if (decimal.Round(vatRatePercent, 2) != vatRatePercent)
            {
                throw new ArgumentException("שיעור המע״מ יכול להכיל עד שתי ספרות אחרי הנקודה.");
            }
            if (defaultRecipeMeasurementUnitId.HasValue)
            {
                int measurementUnitId = defaultRecipeMeasurementUnitId.Value;
                if (measurementUnitId <= 0)
                {
                    throw new ArgumentException("יחידת ברירת המחדל למתכון אינה תקינה.");
                }
                bool unitIsAvailable = MeasurementUnitBLL.GetAvailableUnits(userId).Exists(unit => unit.MeasurementUnitId == measurementUnitId);
                if (!unitIsAvailable)
                {
                    throw new ArgumentException("יחידת ברירת המחדל אינה זמינה לעסק.");
                }
            }
            Business currentBusiness = GetBusinessForUser(userId);
            bool vatRateChanged = currentBusiness.VatRatePercent != vatRatePercent;
            bool wasUpdated = BusinessDAL.UpdateBusinessSettings(userId, showYieldUnitSelection, defaultRecipeMeasurementUnitId, vatRatePercent);
            if (!wasUpdated)
            {
                throw new InvalidOperationException("לא ניתן לעדכן את הגדרות העסק.");
            }
            if (vatRateChanged)
            {
                CostCalculationBLL.RecalculateAndSaveActiveProductCosts(userId);
            }
        }
        public static void UpdateBusinessLogoPath(int userId, string logoPath)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            logoPath = logoPath?.Trim().Replace('\\', '/');
            if (string.IsNullOrWhiteSpace(logoPath))
            {
                throw new ArgumentException("נתיב הלוגו הוא שדה חובה.");
            }
            if (logoPath.Length > 260)
            {
                throw new ArgumentException("נתיב הלוגו ארוך מדי.");
            }
            const string allowedDirectory = "Uploads/BusinessLogos/";
            if (!logoPath.StartsWith(allowedDirectory, StringComparison.Ordinal))
            {
                throw new ArgumentException("נתיב הלוגו אינו תקין.");
            }
            if (logoPath.Contains("..") || logoPath.Contains(":"))
            {
                throw new ArgumentException("נתיב הלוגו אינו תקין.");
            }
            bool hasAllowedExtension = logoPath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                logoPath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                logoPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
            if (!hasAllowedExtension)
            {
                throw new ArgumentException("סוג קובץ הלוגו אינו נתמך.");
            }
            bool wasUpdated = BusinessDAL.UpdateBusinessLogoPath(userId, logoPath);
            if (!wasUpdated)
            {
                throw new InvalidOperationException("לא ניתן לעדכן את לוגו העסק.");
            }
        }
    }
}