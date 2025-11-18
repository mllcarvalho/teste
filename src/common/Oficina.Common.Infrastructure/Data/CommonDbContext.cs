using Microsoft.EntityFrameworkCore;
using Oficina.Common.Domain.Entities;

namespace Oficina.Common.Infrastructure.Data
{
    public class CommonDbContext: DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public CommonDbContext(DbContextOptions<CommonDbContext> options)
            : base(options)
        {

        }
      

    }
}
