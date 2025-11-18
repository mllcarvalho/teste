using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Atendimento.Application.Dto;
using Oficina.Atendimento.Application.IServices;

namespace Oficina.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Atendimento,Admin")]
    public class ServicoController : ControllerBase
    {
        private readonly IServicoAppService _appService;
        public ServicoController(IServicoAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(Guid id)
        {
            var servico = await _appService.ObterAsync(id);
            if (servico == null) return NotFound();

            return Ok(servico);
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObterTodos(int page = 1, int pageSize = 10)
        { 
            var servicos = await _appService.ObterTodosAsync(page, pageSize);
            return Ok(servicos);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] ServicoDto dto)
        {
            var id = await _appService.CriarAsync(dto);
            return Ok(id);
        }

        [HttpPut]
        public async Task<IActionResult> Atualizar([FromBody] ServicoDto dto)
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
