using Oficina.Common.Domain.Enum;
using Oficina.Common.Domain.ISecurity;
using System.ComponentModel.DataAnnotations;

namespace Oficina.Common.Domain.Entities
{
    public  class User: Base
    {
        private User() {}

        public User(string userName, string email, string passwordHash, UserRole role)
        {
            Username = userName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            DataCriacao = DateTime.UtcNow;
        }

        [Required]
        public string Username { get; private set; }
        [Required]
        public string Email { get; private set; }
        [Required]
        public string PasswordHash { get; private set; }
        public UserRole Role { get; private set; }

        public bool VerifyPassword(string password, IPasswordHasher passwordHasher)
        {
            return passwordHasher.Verify(password, PasswordHash);
        }
    }
}
