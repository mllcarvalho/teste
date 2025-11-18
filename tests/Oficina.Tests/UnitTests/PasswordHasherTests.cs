using Oficina.Common.Infrastructure.Security;

namespace Oficina.Tests.UnitTests
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _hasher = new();

        [Fact]
        public void Hash_And_Verify_ReturnsTrue_ForValidPassword()
        {
            var password = "securePassword123!";
            var hash = _hasher.Hash(password);

            Assert.True(_hasher.Verify(password, hash));
        }

        [Fact]
        public void Verify_ReturnsFalse_ForInvalidPassword()
        {
            var password = "securePassword123!";
            var hash = _hasher.Hash(password);

            Assert.False(_hasher.Verify("wrongPassword", hash));
        }

        [Fact]
        public void Verify_ReturnsFalse_ForMalformedHash()
        {
            var password = "securePassword123!";
            var malformedHash = "not.a.valid.hash";

            Assert.False(_hasher.Verify(password, malformedHash));
        }
    }
}
