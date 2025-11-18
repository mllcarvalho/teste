using Oficina.Common.Domain.Entities;

namespace Oficina.Common.Domain.ISecurity
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
