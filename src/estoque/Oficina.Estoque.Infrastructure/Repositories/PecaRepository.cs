using Oficina.Common.Infrastructure.Repositories;
using Oficina.Estoque.Domain.Entities;
using Oficina.Estoque.Domain.IRepositories;
using Oficina.Estoque.Infrastructure.Data;

namespace Oficina.Estoque.Infrastructure.Repositories
{
    public class PecaRepository: GenericRepository<Peca, EstoqueDbContext>, IPecaRepository
    {
        public PecaRepository(EstoqueDbContext context) : base(context) { }

    }
}
