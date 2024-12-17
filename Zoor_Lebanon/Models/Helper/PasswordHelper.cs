using System.Security.Cryptography;
using System.Text;
namespace Zoor_Lebanon.Models.Helper
{


    public static class PasswordHelper
    {
        public static (string hash, string salt) HashPassword(string password)
        {
            var saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            var salt = Convert.ToBase64String(saltBytes);

            var combinedPasswordSalt = password + salt;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPasswordSalt));
                string hash = Convert.ToBase64String(hashBytes);
                return (hash, salt);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            var combinedPasswordSalt = enteredPassword + storedSalt;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPasswordSalt));
                string hash = Convert.ToBase64String(hashBytes);
                return hash == storedHash;
            }
        }
    }

}
