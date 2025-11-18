using Oficina.Atendimento.Domain.Entities;
using Oficina.Common.Domain.IRepository;

namespace Oficina.Atendimento.Domain.IRepository
{
    public interface IClienteRepository: IGenericRepository<Cliente>
    {
        Task<bool> ExisteEmailAsync(string email, Guid? ignoreId = null);
        Task<bool> ExisteDocumentoAsync(string documento, Guid? ignoreId = null);
    }
}
