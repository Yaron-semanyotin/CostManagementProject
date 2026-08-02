using System;
using CostWise.App_Code.DAL;

namespace CostWise.App_Code.BLL
{
    public static class RegistrationBLL
    {
        public static void Register(string businessName, string username, string password, string confirmPassword)
        {
            businessName = businessName?.Trim();
            if (string.IsNullOrWhiteSpace(businessName))
            {
                throw new ArgumentException("שם העסק הוא שדה חובה.");
            }
            if (businessName.Length > 150)
            {
                throw new ArgumentException("שם העסק יכול להכיל עד 150 תווים.");
            }

            username = username?.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("שם המשתמש הוא שדה חובה.");
            }
            if (username.Length > 50)
            {
                throw new ArgumentException("שם המשתמש יכול להכיל עד 50 תווים.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("הסיסמה היא שדה חובה.");
            }
            if (password.Length < 8)
            {
                throw new ArgumentException("הסיסמה חייבת להכיל לפחות 8 תווים.");
            }
            if (password.Length > 128)
            {
                throw new ArgumentException("הסיסמה יכולה להכיל עד 128 תווים.");
            }
            if (string.IsNullOrEmpty(confirmPassword))
            {
                throw new ArgumentException("יש להזין את הסיסמה פעם נוספת.");
            }
            if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                throw new ArgumentException("הסיסמאות אינן תואמות.");
            }
            if (RegistrationDAL.UsernameExists(username))
            {
                throw new InvalidOperationException("שם המשתמש כבר קיים במערכת.");
            }
            string passwordHash = PasswordHasher.HashPassword(password);
            RegistrationDAL.CreateBusinessAndUser(businessName, username, passwordHash);
        }
    }
}