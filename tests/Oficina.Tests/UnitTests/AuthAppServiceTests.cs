using Moq;
using Oficina.Common.Application.Services;
using Oficina.Common.Domain.Entities;
using Oficina.Common.Domain.Enum;
using Oficina.Common.Domain.IRepository;
using Oficina.Common.Domain.ISecurity;

namespace Oficina.Tests.UnitTests
{
    public class AuthAppServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<IPasswordHasher> _hasherMock = new();
        private readonly Mock<IJwtTokenGenerator> _tokenGenMock = new();

        private AuthAppService CreateService() =>
            new AuthAppService(_userRepoMock.Object, _hasherMock.Object, _tokenGenMock.Object);

        [Fact]
        public async Task Authenticate_ReturnsToken_WhenCredentialsAreValid()
        {
            var user = new User("admin", "admin@oficina.com", "hashed", UserRole.Admin);
            _userRepoMock.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync(user);
            _hasherMock.Setup(h => h.Verify("password", "hashed")).Returns(true);
            _tokenGenMock.Setup(t => t.GenerateToken(user)).Returns("token");

            var service = CreateService();
            var result = await service.Authenticate("admin", "password");

            Assert.Equal("token", result);
        }

        [Fact]
        public async Task Authenticate_ReturnsNull_WhenUserNotFound()
        {
            _userRepoMock.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync((User)null);

            var service = CreateService();
            var result = await service.Authenticate("notfound", "password");

            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_ReturnsNull_WhenPasswordInvalid()
        {
            var user = new User("admin", "admin@oficina.com", "hashed", UserRole.Admin);
            _userRepoMock.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync(user);
            _hasherMock.Setup(h => h.Verify("wrong", "hashed")).Returns(false);

            var service = CreateService();
            var result = await service.Authenticate("admin", "wrong");

            Assert.Null(result);
        }
    }
}
