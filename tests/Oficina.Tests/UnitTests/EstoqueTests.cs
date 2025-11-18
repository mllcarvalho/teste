using Oficina.Estoque.Domain.Entities;
using Oficina.Estoque.Domain.Enum;
using Oficina.Estoque.Domain.IRepositories;
using Oficina.Estoque.Domain.Services;
using Moq;
using System.Linq.Expressions;

namespace Oficina.Tests.UnitTests
{

    public class EstoqueTests
    {
        private readonly Mock<IPecaRepository> _pecaRepoMock;
        private readonly Mock<IEstoqueRepository> _estoqueRepoMock;
        private readonly EstoqueDomainService _service;

        public EstoqueTests()
        {
            _pecaRepoMock = new Mock<IPecaRepository>();
            _estoqueRepoMock = new Mock<IEstoqueRepository>();
            _service = new EstoqueDomainService(_pecaRepoMock.Object, _estoqueRepoMock.Object);
        }

        [Fact]
        public async Task AdicionarPeca_Sucesso()
        {
            var pecaId = Guid.NewGuid();
            var peca = new Peca("Filtro", 10m, TipoPeca.Peca, "COD", "desc", "fab", 1);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);
            _estoqueRepoMock.Setup(r => r.AddAsync(It.IsAny<Estoque.Domain.Entities.Estoque>())).Returns(Task.CompletedTask);
            _pecaRepoMock.Setup(r => r.Update(peca));
            _pecaRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.AdicionarPeca(pecaId, 5);

            Assert.True(peca.Quantidade >= 5);
        }

        [Fact]
        public async Task AdicionarPeca_PecaNaoExiste()
        {
            var pecaId = Guid.NewGuid();
            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync((Peca)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarPeca(pecaId, 5));
        }

        [Fact]
        public async Task AdicionarPeca_QuantidadeInvalida()
        {
            var pecaId = Guid.NewGuid();
            var peca = new Peca("Filtro", 10m, TipoPeca.Peca, "COD", "desc", "fab", 1);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarPeca(pecaId, 0));
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarPeca(pecaId, -1));
        }

        [Fact]
        public async Task RemoverPeca_Sucesso()
        {
            var pecaId = Guid.NewGuid();
            var peca = new Peca("Filtro", 10m, TipoPeca.Peca, "COD", "desc", "fab", 1);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            peca.AdicionarQuantidade(10);
            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);
            _pecaRepoMock.Setup(r => r.Update(peca));
            _pecaRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _estoqueRepoMock.Setup(r => r.AddAsync(It.IsAny<Estoque.Domain.Entities.Estoque>())).Returns(Task.CompletedTask);
            _estoqueRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.RemoverPeca(pecaId, 5);

            Assert.Equal(5, peca.Quantidade);
        }

        [Fact]
        public async Task RemoverPeca_PecaNaoExiste()
        {
            var pecaId = Guid.NewGuid();
            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync((Peca)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverPeca(pecaId, 5));
        }

        [Fact]
        public async Task RemoverPeca_EstoqueInsuficiente()
        {
            var pecaId = Guid.NewGuid();
            var peca = new Peca("Filtro", 10m, TipoPeca.Peca, "COD", "desc", "fab", 1);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            peca.AdicionarQuantidade(2);
            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverPeca(pecaId, 5));
        }

        [Fact]
        public async Task RemoverPeca_QuantidadeInvalida()
        {
            var pecaId = Guid.NewGuid();
            var peca = new Peca("Filtro", 10m, TipoPeca.Peca, "COD", "desc", "fab", 1);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            peca.AdicionarQuantidade(10);
            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);

            Assert.Throws<ArgumentException>(() => peca.RemoverQuantidade(0));
            Assert.Throws<ArgumentException>(() => peca.RemoverQuantidade(-1));
        }

        [Fact]
        public async Task ConsultarQuantidade_Sucesso()
        {
            var pecaId = Guid.NewGuid();
            var peca = new Peca("Filtro", 10m, TipoPeca.Peca, "COD", "desc", "fab", 1);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            peca.AdicionarQuantidade(7);
            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);

            var quantidade = await _service.ConsultarQuantidade(pecaId);

            Assert.Equal(7, quantidade);
        }

        [Fact]
        public async Task ConsultarQuantidade_PecaNaoExiste()
        {
            var pecaId = Guid.NewGuid();
            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync((Peca)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ConsultarQuantidade(pecaId));
        }
    }
}
