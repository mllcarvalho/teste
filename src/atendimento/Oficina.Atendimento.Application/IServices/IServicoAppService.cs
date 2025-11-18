using Oficina.Atendimento.Application.Dto;

namespace Oficina.Atendimento.Application.IServices
{
    public interface IServicoAppService
    {
        Task<Guid> CriarAsync(ServicoDto dto);
        Task<ServicoDto?> ObterAsync(Guid id);
        Task<IEnumerable<ServicoDto>> ObterTodosAsync(int page = 1, int pageSize = 10);
        Task AtualizarAsync(ServicoDto dto);
        Task DeletarAsync(Guid id);
    }
}
