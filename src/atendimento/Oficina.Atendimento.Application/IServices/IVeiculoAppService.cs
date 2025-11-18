using Oficina.Atendimento.Application.Dto;

namespace Oficina.Atendimento.Application.IServices
{
    public interface IVeiculoAppService
    {
        Task<Guid> CriarAsync(VeiculoDto dto);
        Task<VeiculoDto?> ObterAsync(Guid id);
        Task<IEnumerable<VeiculoDto>> ObterTodosPorClienteAsync(Guid clientId, int page = 1, int pageSize = 10);
        Task<IEnumerable<VeiculoDto>> ObterTodosAsync(int page = 1, int pageSize = 10);
        Task AtualizarAsync(VeiculoDto dto);
        Task DeletarAsync(Guid id);
    }
}
