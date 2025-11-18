using Oficina.Common.Domain.Entities;
using Oficina.Common.Domain.IRepository;
using Oficina.Common.Infrastructure.Data;

namespace Oficina.Common.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User, CommonDbContext>, IUserRepository
    {
        public UserRepository(CommonDbContext context) : base(context) { }      

    }
}
