using Oficina.Atendimento.Application.Dto;

namespace Oficina.Atendimento.Application.IServices
{
    public interface IOrdemServicoAppService
    {
        Task<Guid> CriarAsync(string clienteDoc, string veiculoPlaca);
        Task<ListarOrdemDeServicoDto?> ObterPorIdAsync(Guid id);
        Task<List<ListarOrdemDeServicoDto>> ObterTodosAsync(int page, int pageSize);
        Task IniciarDiagnostico(Guid ordemId);
        Task ConcluirDiagnostico(OrdemDeServicoDto dto);
        Task AprovarOrcamento(Guid ordemId);
        Task FinalizarExecucao(Guid ordemId);
        Task Entregar(Guid ordemId);
        Task<double> CalcularTempoMedioExecucao();
    }
}
