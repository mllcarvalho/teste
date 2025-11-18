using Oficina.Common.Infrastructure.Repositories;
using Oficina.Estoque.Domain.IRepositories;
using Oficina.Estoque.Infrastructure.Data;

namespace Oficina.Estoque.Infrastructure.Repositories
{
    public class EstoqueRepository : GenericRepository<Domain.Entities.Estoque, EstoqueDbContext>, IEstoqueRepository
    {
        public EstoqueRepository(EstoqueDbContext context) : base(context) { }

    }
}
