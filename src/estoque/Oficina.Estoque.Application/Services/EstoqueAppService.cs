using Oficina.Estoque.Application.Dto;
using Oficina.Estoque.Application.IServices;
using Oficina.Estoque.Domain.IRepositories;
using Oficina.Estoque.Domain.Services;

namespace Oficina.Estoque.Application.Services
{
    public class EstoqueAppService: IEstoqueAppService
    {
        private readonly EstoqueDomainService _estoqueDomainService;

        public EstoqueAppService(EstoqueDomainService estoqueDomainService, IPecaRepository pecaRepository)
        {
            _estoqueDomainService = estoqueDomainService;
        }

        public async Task AdicionarPecaAoEstoque(Guid id, int quantidade)
        {
            await _estoqueDomainService.AdicionarPeca(id, quantidade);
        }

        public async Task RemoverPecaDoEstoque(Guid id, int quantidade)
        {
           await _estoqueDomainService.RemoverPeca(id, quantidade);
        }

        public async Task<int> ConsultarQuantidadeEmEstoque(Guid pecaId)
        {
            return await _estoqueDomainService.ConsultarQuantidade(pecaId);
        }
    }
}
