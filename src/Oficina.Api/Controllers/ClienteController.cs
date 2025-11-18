using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Atendimento.Application.Dto;
using Oficina.Atendimento.Application.IServices;

namespace Oficina.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Atendimento,Admin")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteAppService _appService;
        public ClienteController(IClienteAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(Guid id)
        {
            var cliente = await _appService.ObterAsync(id);
            if (cliente == null) return NotFound();

            return Ok(cliente);
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObterTodos(int page = 1, int pageSize = 10)
        {
            var clientes = await _appService.ObterTodosAsync(page, pageSize);
            return Ok(clientes);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarClienteDto dto)
        {
            var id = await _appService.CriarAsync(dto);
            return Ok(id);
        }

        [HttpPut]
        public async Task<IActionResult> Atualizar([FromBody] ClienteDto dto)
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
