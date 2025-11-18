using Oficina.Atendimento.Domain.Entities;
using Oficina.Common.Domain.IRepository;

namespace Oficina.Atendimento.Domain.IRepository
{
    public interface IOrdemDeServicoRepository : IGenericRepository<OrdemDeServico>
    {
        Task<IEnumerable<OrdemDeServico>> ObterFinalizadasAsync();
    }
}
