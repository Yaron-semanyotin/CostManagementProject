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
        public static bool VerifyPassword(string password, string storedPasswordHash) // function to verify the password by the password and passwordHash
        {
            if (password == null) // checks if the password is null
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (storedPasswordHash == null) // check if thr storedPasswordHash is null
            {
                throw new ArgumentNullException(nameof(storedPasswordHash));
            }
            string[] parts = storedPasswordHash.Split('$'); // splits the string at each $ char
            if (parts.Length != 4)
                return false;
            if (!string.Equals(parts[0], AlgorithmName, StringComparison.Ordinal))
                return false;
            int iterations;
            if (!int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out iterations) || iterations <= 0)
                return false;
            byte[] salt;
            byte[] expectedHash;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expectedHash = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }
            if (salt.Length != SaltSize || expectedHash.Length != HashSize)
                return false;
            byte[] actualHash;
            using(Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                actualHash = pbkdf2.GetBytes(HashSize);
            }
            return FixedTimeEquals(actualHash, expectedHash);
        }
        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length)
                return false;
            int difference = 0;
            for (int i = 0; i < left.Length; i++)
            {
                difference |= left[i] ^ right[i];
            }
            return difference == 0;
        }
    }
}