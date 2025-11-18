using Oficina.Common.Application.IServices;
using Oficina.Common.Domain.IRepository;
using Oficina.Common.Domain.ISecurity;

namespace Oficina.Common.Application.Services
{
    public class AuthAppService: IAuthAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public AuthAppService(IUserRepository userRepository,
                           IPasswordHasher passwordHasher,
                           IJwtTokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<string> Authenticate(string username, string password)
        {
            var user = await _userRepository.GetAsync(x => x.Username == username);
            if (user == null) return null;

            if (!user.VerifyPassword(password, _passwordHasher)) return null;

            return _tokenGenerator.GenerateToken(user);
        }
    }
}
