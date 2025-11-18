using Oficina.Common.Domain.ISecurity;
using System.Security.Cryptography;

namespace Oficina.Common.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32;  // 256 bit
        private const int Iterations = 100000;

        public string Hash(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            using var algorithm = new Rfc2898DeriveBytes(
                password,
                SaltSize,
                Iterations,
                HashAlgorithmName.SHA256);

            var key = algorithm.GetBytes(KeySize);
            var salt = algorithm.Salt;

            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public bool Verify(string password, string passwordHash)
        {
            var parts = passwordHash.Split('.', 2);
            if (parts.Length != 2)
                return false;

            try
            {
                var salt = Convert.FromBase64String(parts[0]);
                var key = Convert.FromBase64String(parts[1]);

                using var algorithm = new Rfc2898DeriveBytes(
                    password,
                    salt,
                    Iterations,
                    HashAlgorithmName.SHA256);

                var keyToCheck = algorithm.GetBytes(KeySize);

                return keyToCheck.SequenceEqual(key);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
