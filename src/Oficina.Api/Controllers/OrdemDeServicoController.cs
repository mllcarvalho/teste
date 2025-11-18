using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Atendimento.Application.Dto;
using Oficina.Atendimento.Application.IServices;


namespace Oficina.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Atendimento,Admin")]
    public class OrdemDeServicoController : ControllerBase
    {
        private readonly IOrdemServicoAppService _appService;

        public OrdemDeServicoController(IOrdemServicoAppService appService)
        {
            _appService = appService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarOrdem(string clienteDoc, string veiculoPlaca)
        {
            var result = await _appService.CriarAsync(clienteDoc, veiculoPlaca);
        
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(Guid id)
        {
            var ordem = await _appService.ObterPorIdAsync(id);
            if (ordem == null)
                return NotFound();
            return Ok(ordem);
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObterTodos(int page = 1, int pageSize = 10)
        {
            var ordens = await _appService.ObterTodosAsync(page, pageSize);
            return Ok(ordens);
        }

        [HttpPost("iniciar-diagnostico")]
        public async Task<IActionResult> IniciarDiagnostico(Guid ordemId)
        {
            await _appService.IniciarDiagnostico(ordemId);
            return Ok();
        }

        [HttpPost("concluir-diagnostico")]
        public async Task<IActionResult> ConcluirDiagnostico([FromBody] OrdemDeServicoDto request)
        {
            await _appService.ConcluirDiagnostico(request);
            return Ok();
        }

        [HttpPost("aprovar-orcamento")]
        public async Task<IActionResult> AprovarOrcamento(Guid ordemId)
        {
            await _appService.AprovarOrcamento(ordemId);
            return Ok();
        }

        [HttpPost("finalizar-execucao")]
        public async Task<IActionResult> FinalizarExecucao(Guid ordemId)
        {
            await _appService.FinalizarExecucao(ordemId);
            return Ok();
        }

        [HttpPost("entregar")]
        public async Task<IActionResult> Entregar(Guid ordemId)
        {
            await _appService.Entregar(ordemId);
            return Ok();
        }

        [HttpGet("tempo-medio-execucao")]
        public async Task<IActionResult> ObterTempoMedioExecucao()
        {
            var mediaHoras = await _appService.CalcularTempoMedioExecucao();

            return Ok(new
            {
                tempoMedioExecucaoHoras = mediaHoras,
                mensagem = $"Tempo médio de execução das ordens finalizadas: {mediaHoras} horas"
            });
        }

        [HttpGet("os-status/{ordemId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Status(Guid ordemId)
        {
            var ordem = await _appService.ObterPorIdAsync(ordemId);
            if (ordem == null) return NotFound();
            return Ok(new { ordem.Status });
        }
    }
}
