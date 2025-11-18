using Oficina.Estoque.Domain.Enum;
using Oficina.Estoque.Domain.IRepositories;

namespace Oficina.Estoque.Domain.Services
{
    public class EstoqueDomainService
    {
        private readonly IPecaRepository _pecaRepository;
        private readonly IEstoqueRepository _movimentoRepository;

        public EstoqueDomainService(IPecaRepository pecaRepository, IEstoqueRepository movimentoEstoqueRepository)
            {
            _pecaRepository = pecaRepository;
            _movimentoRepository = movimentoEstoqueRepository;
        }

        public async Task AdicionarPeca(Guid pecaId, int quantidade)
        {
            var peca = await _pecaRepository.GetByIdAsync(pecaId) ?? throw new ArgumentException("Peça não encontrada.");
            peca.AdicionarQuantidade(quantidade);

            var movimento = new Entities.Estoque(peca.Id, TipoMovimentoEstoque.Entrada, quantidade);
            await _movimentoRepository.AddAsync(movimento);

            _pecaRepository.Update(peca);
            await _pecaRepository.SaveChangesAsync();
        }

        public async Task RemoverPeca(Guid pecaId, int quantidade)
        {
            var peca = await _pecaRepository.GetByIdAsync(pecaId);
            if (peca == null)
                throw new InvalidOperationException($"Peça {pecaId} não encontrada.");

            if (peca.Quantidade < quantidade)
                throw new InvalidOperationException(
                    $"Estoque insuficiente para remover {quantidade} unidade(s). Disponível: {peca.Quantidade}."
                );

            peca.RemoverQuantidade(quantidade);
            _pecaRepository.Update(peca);
            await _pecaRepository.SaveChangesAsync();

            var movimento = new Entities.Estoque(pecaId, TipoMovimentoEstoque.Saida, quantidade);
            await _movimentoRepository.AddAsync(movimento);
            await _movimentoRepository.SaveChangesAsync();
        }

        public async Task<int> ConsultarQuantidade(Guid pecaId)
        {
            var peca = await _pecaRepository.GetByIdAsync(pecaId);
            if (peca == null)
                throw new InvalidOperationException($"Peça {pecaId} não encontrada.");

            return peca.Quantidade;
        }
    }
}
