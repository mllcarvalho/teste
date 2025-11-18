using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Infrastructure.Data;
using Oficina.Common.Infrastructure.Repositories;

namespace Oficina.Atendimento.Infrastructure.Repositories
{
    public class OrcamentoRepository : GenericRepository<Orcamento, AtendimentoDbContext>, IOrcamentoRepository
    {
        public OrcamentoRepository(AtendimentoDbContext context) : base(context) { }

    }
}
