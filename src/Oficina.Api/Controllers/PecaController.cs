using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Estoque.Application.Dto;
using Oficina.Estoque.Application.IServices;

namespace Oficina.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Estoque,Admin")]
    public class PecaController: ControllerBase
    {
        private readonly IPecaAppService _appService;

        public PecaController(IPecaAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(Guid id)
        {
            var peca = await _appService.ObterAsync(id);
            if (peca == null) return NotFound();

            return Ok(peca);
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObterTodos(int page = 1, int pageSize = 10)
        {
            var pecas = await _appService.ObterTodosAsync(page, pageSize);
            return Ok(pecas);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] PecaDto dto)
        {
            var id = await _appService.CriarAsync(dto);
            return Ok(id);
        }

        [HttpPut]
        public async Task<IActionResult> Atualizar([FromBody] PecaDto dto)
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
