using System;
using CostWise.App_Code.DAL;

namespace CostWise.App_Code.BLL
{
    public static class AuthenticationBLL
    {
        public static User Authenticate(string username, string password)
        {
            username = username?.Trim();
            if (string.IsNullOrWhiteSpace(username)) // if the username is empty return a massage
            {
                throw new ArgumentException("שם המשתמש הוא שדה חובה.");
            }
            if (username.Length > 50)
            {
                throw new ArgumentException("שם המשתמש יכול להכיל עד 50 תווים."); // if the username is to long return a massage
            }
            if (string.IsNullOrWhiteSpace(password)) // if the password is empty return a massage
            {
                throw new ArgumentException("הסיסמה היא שדה חובה.");
            }
            if (password.Length < 8) // if the password is to short return a massage
            {
                throw new ArgumentException("הסיסמה חייבת להכיל לפחות 8 תווים.");
            }
            if (password.Length > 128) // if the password is to long return a massage
            {
                throw new ArgumentException("הסיסמה יכולה להכיל עד 128 תווים.");
            }
            User user = AuthenticationDAL.GetUserByUsername(username); // creating user object
            if (user == null) // if its null return null
                return null;
            if (!PasswordHasher.VerifyPassword(password, user.PasswordHash)) // if the user password not equals to passwordhash retun null
                return null;
            user.PasswordHash = null;
            return user;
        }
    }
}