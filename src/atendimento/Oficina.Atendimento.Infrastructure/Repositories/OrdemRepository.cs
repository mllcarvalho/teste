using Microsoft.EntityFrameworkCore;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Infrastructure.Data;
using Oficina.Common.Infrastructure.Repositories;

namespace Oficina.Atendimento.Infrastructure.Repositories
{
    public class OrdemRepository: GenericRepository<OrdemDeServico, AtendimentoDbContext>, IOrdemDeServicoRepository
    {
        public OrdemRepository(AtendimentoDbContext context) : base(context) { }
        public async Task<IEnumerable<OrdemDeServico>> ObterFinalizadasAsync()
        {
            var query = _dbSet.AsQueryable();
            return await query
                .Where(o => o.DataInicioExecucao.HasValue && o.DataFinalizacao.HasValue)
                .ToListAsync();
        }
    }
}
