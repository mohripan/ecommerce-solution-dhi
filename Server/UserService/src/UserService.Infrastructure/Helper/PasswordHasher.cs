using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserService.Contracts.Interfaces;

namespace UserService.Infrastructure.Helper
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);

                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = deriveBytes.GetBytes(32);

                    return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
                }
            }
        }

        public bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            var parts = hashedPassword.Split('.');
            if (parts.Length != 2)
                throw new FormatException("Invalid hash format.");

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHash = Convert.FromBase64String(parts[1]);

            using (var deriveBytes = new Rfc2898DeriveBytes(hashedPassword, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] computedHash = deriveBytes.GetBytes(32);

                return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
            }
        }
    }
}
