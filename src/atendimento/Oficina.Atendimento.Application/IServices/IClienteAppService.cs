using Oficina.Atendimento.Application.Dto;

namespace Oficina.Atendimento.Application.IServices
{
    public interface IClienteAppService
    {
        Task<Guid> CriarAsync(CriarClienteDto dto);
        Task<ClienteDto?> ObterAsync(Guid id);
        Task<IEnumerable<ClienteDto>> ObterTodosAsync(int page = 1, int pageSize = 10);
        Task AtualizarAsync(ClienteDto dto);
        Task DeletarAsync(Guid id);
    }
}
