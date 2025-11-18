using Oficina.Atendimento.Domain.Entities;
using Oficina.Common.Domain.IRepository;

namespace Oficina.Atendimento.Domain.IRepository
{
    public interface IVeiculoRepository : IGenericRepository<Veiculo>
    {
        Task<int> DeletarPorClienteIdAsync(Guid clienteId);
    }
}
