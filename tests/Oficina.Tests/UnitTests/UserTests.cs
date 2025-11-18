using Oficina.Common.Domain.Entities;
using Oficina.Common.Domain.Enum;
using Oficina.Common.Infrastructure.Security;

namespace Oficina.Tests.UnitTests
{
    public class UserTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            var hasher = new PasswordHasher();
            var password = "pass";
            var hash = hasher.Hash(password);

            var user = new User("user", "user@email.com", hash, UserRole.Atendimento);

            Assert.Equal("user", user.Username);
            Assert.Equal("user@email.com", user.Email);
            Assert.Equal(hash, user.PasswordHash);
            Assert.Equal(UserRole.Atendimento, user.Role);
            Assert.NotEqual(default, user.DataCriacao);
        }

        [Fact]
        public void VerifyPassword_ReturnsTrue_ForCorrectPassword()
        {
            var hasher = new PasswordHasher();
            var password = "pass";
            var hash = hasher.Hash(password);
            var user = new User("user", "user@email.com", hash, UserRole.Admin);

            Assert.True(user.VerifyPassword(password, hasher));
        }

        [Fact]
        public void VerifyPassword_ReturnsFalse_ForIncorrectPassword()
        {
            var hasher = new PasswordHasher();
            var hash = hasher.Hash("pass");
            var user = new User("user", "user@email.com", hash, UserRole.Admin);

            Assert.False(user.VerifyPassword("wrong", hasher));
        }
    }
}
