using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Atendimento.Application.Dto;
using Oficina.Atendimento.Application.IServices;
using Oficina.Estoque.Application.Dto;

namespace Oficina.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Atendimento,Admin")]
    public class VeiculoController : ControllerBase
    {
        private readonly IVeiculoAppService _appService;
        public VeiculoController(IVeiculoAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(Guid id)
        {
            var veiculo = await _appService.ObterAsync(id);
            if (veiculo == null) return NotFound();

            return Ok(veiculo);
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObterTodos(int page = 1, int pageSize = 10)
        {   
            var veiculos = await _appService.ObterTodosAsync(page, pageSize);
            return Ok(veiculos);
        }

        [HttpGet("todos/{clientId}")]
        public async Task<IActionResult> ObterTodosPorCliente(Guid clientId, int page = 1, int pageSize = 10)
        {
            var veiculos = await _appService.ObterTodosPorClienteAsync(clientId, page, pageSize);
            return Ok(veiculos);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] VeiculoDto veiculoDto)
        {
            var id = await _appService.CriarAsync(veiculoDto);
            return Ok(id);
        }

        [HttpPut]
        public async Task<IActionResult> Atualizar([FromBody] VeiculoDto dto)
        {
            await _appService.AtualizarAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await _appService.DeletarAsync(id);
            return Ok();
        }
    }
}
