using Oficina.Estoque.Application.Dto;

namespace Oficina.Estoque.Application.IServices
{
    public interface IPecaAppService
    {
        Task<Guid> CriarAsync(PecaDto dto);
        Task<PecaDto?> ObterAsync(Guid id);
        Task<IEnumerable<PecaDto>> ObterTodosAsync(int page = 1, int pageSize = 10);
        Task AtualizarAsync(PecaDto dto);
        Task DeletarAsync(Guid id);
    }
}
