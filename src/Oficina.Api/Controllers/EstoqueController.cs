using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Estoque.Application.IServices;

namespace Oficina.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Estoque,Admin")]
    public class EstoqueController: ControllerBase
    {
        private readonly IEstoqueAppService _appService;
        public EstoqueController(IEstoqueAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("{pecaId}")]
        public async Task<IActionResult> ConsultarQuantidade(Guid pecaId)
        {
            var quantidade = await _appService.ConsultarQuantidadeEmEstoque(pecaId);
            return Ok(quantidade);
        }

        [HttpPost("adicionar")]
        public async Task<IActionResult> Adicionar(Guid pecaId, int quantidade)
        {
            await _appService.AdicionarPecaAoEstoque(pecaId, quantidade);
            return Ok();
        }

        [HttpPost("remover")]
        public async Task<IActionResult> Remover(Guid pecaId, int quantidade)
        {
            await _appService.RemoverPecaDoEstoque(pecaId, quantidade);
            return Ok();
        }
    }
}
