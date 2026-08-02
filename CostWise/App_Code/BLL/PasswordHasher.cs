using System;
using System.Security.Cryptography;
using System.Globalization;

namespace CostWise.App_Code.BLL
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int IterationCount = 600000;
        private const string AlgorithmName = "PBKDF2-SHA256";
        public static string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            byte[] salt = new byte[SaltSize];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }
            byte[] hash;
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, IterationCount, HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(HashSize);
            }
            return string.Format(CultureInfo.InvariantCulture, "{0}${1}${2}${3}", AlgorithmName, IterationCount, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }
    }
}