using Moq;
using Oficina.Estoque.Application.Dto;
using Oficina.Estoque.Application.Services;
using Oficina.Estoque.Domain.Entities;
using Oficina.Estoque.Domain.Enum;
using Oficina.Estoque.Domain.IRepositories;
using Oficina.Estoque.Domain.Services;

namespace Oficina.Tests.UnitTests
{
    public class EstoqueAppServiceTests
    {
        private readonly Mock<IPecaRepository> _pecaRepoMock;
        private readonly Mock<IEstoqueRepository> _estoqueRepoMock;
        private readonly EstoqueDomainService _domainService;
        private readonly EstoqueAppService _appService;

        public EstoqueAppServiceTests()
        {
            _pecaRepoMock = new Mock<IPecaRepository>();
            _estoqueRepoMock = new Mock<IEstoqueRepository>();
            _domainService = new EstoqueDomainService(_pecaRepoMock.Object, _estoqueRepoMock.Object);
            _appService = new EstoqueAppService(_domainService, _pecaRepoMock.Object);
        }

        [Fact]
        public async Task AdicionarPecaAoEstoque_Sucesso()
        {
            var pecaId = Guid.NewGuid();
            var dto = new PecaDto
            {
                PecaId = pecaId,
                Quantidade = 5,
                Nome = "Filtro",
                Tipo = "Peca",
                Codigo = "COD",
                Descricao = "desc",
                Fabricante = "fab",
                Preco = 10m,
                QuantidadeMinima = 1
            };

            var peca = new Peca(dto.Nome, dto.Preco, TipoPeca.Peca, dto.Codigo, dto.Descricao, dto.Fabricante, dto.QuantidadeMinima);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);

            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<System.Linq.Expressions.Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);
            _pecaRepoMock.Setup(r => r.Update(peca));
            _pecaRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _estoqueRepoMock.Setup(r => r.AddAsync(It.IsAny<Estoque.Domain.Entities.Estoque>())).Returns(Task.CompletedTask);

            await _appService.AdicionarPecaAoEstoque(dto.PecaId, dto.Quantidade);

            Assert.True(peca.Quantidade >= 5);
        }

        [Fact]
        public async Task AdicionarPecaAoEstoque_PecaNaoExiste()
        {
            var pecaId = Guid.NewGuid();
            var dto = new PecaDto { PecaId = pecaId, Quantidade = 5 };

            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<System.Linq.Expressions.Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync((Peca)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _appService.AdicionarPecaAoEstoque(dto.PecaId, dto.Quantidade));
        }

        [Fact]
        public async Task AdicionarPecaAoEstoque_QuantidadeInvalida()
        {
            var pecaId = Guid.NewGuid();
            var dtoZero = new PecaDto { PecaId = pecaId, Quantidade = 0 };
            var dtoNeg = new PecaDto { PecaId = pecaId, Quantidade = -1 };

            var peca = new Peca("Filtro", 10m, TipoPeca.Peca, "COD", "desc", "fab", 1);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);

            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<System.Linq.Expressions.Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);

            await Assert.ThrowsAsync<ArgumentException>(() => _appService.AdicionarPecaAoEstoque(dtoZero.PecaId, dtoZero.Quantidade));
            await Assert.ThrowsAsync<ArgumentException>(() => _appService.AdicionarPecaAoEstoque(dtoNeg.PecaId, dtoNeg.Quantidade));
        }

        [Fact]
        public async Task RemoverPecaDoEstoque_Sucesso()
        {
            var pecaId = Guid.NewGuid();
            var quantidade = 5;
            var peca = new Peca("Filtro", 10m, TipoPeca.Peca, "COD", "desc", "fab", 1);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            peca.AdicionarQuantidade(10);

            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<System.Linq.Expressions.Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);
            _pecaRepoMock.Setup(r => r.Update(peca));
            _pecaRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _estoqueRepoMock.Setup(r => r.AddAsync(It.IsAny<Estoque.Domain.Entities.Estoque>())).Returns(Task.CompletedTask);
            _estoqueRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _appService.RemoverPecaDoEstoque(pecaId, quantidade);

            Assert.Equal(5, peca.Quantidade);
        }

        [Fact]
        public async Task RemoverPecaDoEstoque_PecaNaoExiste()
        {
            var pecaId = Guid.NewGuid();
            var quantidade = 5;

            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<System.Linq.Expressions.Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync((Peca)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _appService.RemoverPecaDoEstoque(pecaId, quantidade));
        }

        [Fact]
        public async Task RemoverPecaDoEstoque_EstoqueInsuficiente()
        {
            var pecaId = Guid.NewGuid();
            var quantidade = 5;
            var peca = new Peca("Filtro", 10m, TipoPeca.Peca, "COD", "desc", "fab", 1);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            peca.AdicionarQuantidade(2);

            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<System.Linq.Expressions.Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _appService.RemoverPecaDoEstoque(pecaId, quantidade));
        }

        [Fact]
        public async Task RemoverPecaDoEstoque_QuantidadeInvalida()
        {
            var pecaId = Guid.NewGuid();
            var peca = new Peca("Filtro", 10m, TipoPeca.Peca, "COD", "desc", "fab", 1);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            peca.AdicionarQuantidade(10);

            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<System.Linq.Expressions.Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);

            await Assert.ThrowsAsync<ArgumentException>(() => _appService.RemoverPecaDoEstoque(pecaId, 0));
            await Assert.ThrowsAsync<ArgumentException>(() => _appService.RemoverPecaDoEstoque(pecaId, -1));
        }

        [Fact]
        public void RemoverQuantidade_DeveLancarArgumentException_QuandoEstoqueInsuficiente()
        {
            var peca = new Peca("Filtro", 10m, TipoPeca.Peca, "COD", "desc", "fab", 1);
            peca.GetType().GetProperty("Id")?.SetValue(peca, Guid.NewGuid());
            peca.AdicionarQuantidade(2); 

            var ex = Assert.Throws<ArgumentException>(() => peca.RemoverQuantidade(5));
            Assert.Equal("Estoque insuficiente.", ex.Message);
        }

        [Fact]
        public async Task ConsultarQuantidadeEmEstoque_Sucesso()
        {
            var pecaId = Guid.NewGuid();
            var peca = new Peca("Filtro", 10m, TipoPeca.Peca, "COD", "desc", "fab", 1);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            peca.AdicionarQuantidade(7);

            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<System.Linq.Expressions.Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);

            var quantidade = await _appService.ConsultarQuantidadeEmEstoque(pecaId);

            Assert.Equal(7, quantidade);
        }

        [Fact]
        public async Task ConsultarQuantidadeEmEstoque_PecaNaoExiste()
        {
            var pecaId = Guid.NewGuid();

            _pecaRepoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<System.Linq.Expressions.Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync((Peca)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _appService.ConsultarQuantidadeEmEstoque(pecaId));
        }
    }
}