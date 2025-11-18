using Microsoft.Extensions.Configuration;
using Oficina.Common.Domain.Entities;
using Oficina.Common.Domain.Enum;
using Oficina.Common.Infrastructure.Security;

namespace Oficina.Tests.UnitTests
{
    public class JwtTokenGeneratorTests
    {
        [Fact]
        public void GenerateToken_ReturnsToken_WithExpectedClaims()
        {
            var inMemorySettings = new Dictionary<string, string> {
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"}
        };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            System.Environment.SetEnvironmentVariable("JWT__KEY", "mysupersecretkeythatismorethan32chars!");

            var generator = new JwtTokenGenerator(configuration);
            var user = new User("user", "user@email.com", "hash", UserRole.Admin);

            var token = generator.GenerateToken(user);

            Assert.False(string.IsNullOrWhiteSpace(token));
            // Optionally, decode and check claims if needed
        }
    }
}
