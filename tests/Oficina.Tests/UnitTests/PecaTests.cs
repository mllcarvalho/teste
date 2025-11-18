using Oficina.Estoque.Application.Dto;
using Oficina.Estoque.Application.Services;
using Oficina.Estoque.Domain.Entities;
using Oficina.Estoque.Domain.Enum;
using Oficina.Estoque.Domain.IRepositories;
using Moq;
using System.Linq.Expressions;

namespace Oficina.Tests.UnitTests
{
    public class PecaTests
    {
        private readonly Mock<IPecaRepository> _repoMock;
        private readonly PecaAppService _service;

        public PecaTests()
        {
            _repoMock = new Mock<IPecaRepository>();
            _service = new PecaAppService(_repoMock.Object);
        }

        [Fact]
        public async Task CriarAsync_DeveRetornarId()
        {
            var dto = new PecaDto
            {
                Nome = "Filtro de óleo",
                Preco = 50m,
                Tipo = "Peca",
                Codigo = "FILTRO123",
                Descricao = "Filtro de óleo sintético",
                Fabricante = "Bosch",
                QuantidadeMinima = 2,
                Quantidade = 10
            };
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Peca>()))
                .Callback<Peca>(p => p.GetType().GetProperty("Id")?.SetValue(p, Guid.NewGuid()))
                .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var id = await _service.CriarAsync(dto);

            Assert.NotEqual(Guid.Empty, id);
        }
        
        [Fact]
        public async Task CriarAsync_DeveLancarArgumentException_QuandoTipoInvalido()
        {
            var dto = new PecaDto
            {
                Nome = "Filtro inválido",
                Preco = 10m,
                Tipo = "TipoInvalido",
                Codigo = "COD",
                Descricao = "desc",
                Fabricante = "fab",
                QuantidadeMinima = 1,
                Quantidade = 0
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
            Assert.Equal($"Valor inválido para enum {typeof(TipoPeca).Name}: {dto.Tipo}", ex.Message);
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarPecaDto_QuandoExiste()
        {
            var pecaId = Guid.NewGuid();
            var peca = new Peca("Filtro de óleo", 50m, TipoPeca.Peca, "FILTRO123", "Filtro de óleo sintético", "Bosch", 2);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            _repoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);

            var result = await _service.ObterAsync(pecaId);

            Assert.NotNull(result);
            Assert.Equal("Filtro de óleo", result.Nome);
            Assert.Equal(50m, result.Preco);
            Assert.Equal(pecaId, result.PecaId);
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarNull_QuandoNaoExiste()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Peca)null);

            var result = await _service.ObterAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarListaDePecas()
        {
            var pecas = new List<Peca>
            {
                new Peca("Filtro de óleo", 50m, TipoPeca.Peca, "FILTRO123", "Filtro de óleo sintético", "Bosch", 2),
                new Peca("Filtro de ar", 60m, TipoPeca.Peca, "FILTRO456", "Filtro de ar esportivo", "Mann", 3)
            };
            _repoMock.Setup(r => r.GetAllAsync(1, 10, null, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(pecas);

            var result = await _service.ObterTodosAsync(1, 10);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Nome == "Filtro de óleo");
            Assert.Contains(result, p => p.Nome == "Filtro de ar");
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarPeca()
        {
            var pecaId = Guid.NewGuid();
            var peca = new Peca("Filtro de óleo", 50m, TipoPeca.Peca, "FILTRO123", "Filtro de óleo sintético", "Bosch", 2);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            var dto = new PecaDto
            {
                PecaId = pecaId,
                Nome = "Filtro de ar",
                Preco = 60m,
                Tipo = "Peca",
                Codigo = "FILTRO456",
                Descricao = "Filtro de ar esportivo",
                Fabricante = "Mann",
                QuantidadeMinima = 3,
                Quantidade = 5
            };
            _repoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);

            _repoMock
               .Setup(r => r.SaveChangesAsync())
               .Callback(() => peca.DataAtualizacao = DateTime.UtcNow)
               .Returns(Task.CompletedTask);

            await _service.AtualizarAsync(dto);

            Assert.Equal("Filtro de ar", peca.Nome);
            Assert.Equal(60m, peca.Preco);
            Assert.Equal("FILTRO456", peca.Codigo);
            Assert.Equal("Filtro de ar esportivo", peca.Descricao);
            Assert.Equal("Mann", peca.Fabricante);
            Assert.Equal(3, peca.QuantidadeMinima);
        }

        [Fact]
        public async Task DeletarAsync_DeveRemoverPeca()
        {
            var pecaId = Guid.NewGuid();
            var peca = new Peca("Filtro de óleo", 50m, TipoPeca.Peca, "FILTRO123", "Filtro de óleo sintético", "Bosch", 2);
            peca.GetType().GetProperty("Id")?.SetValue(peca, pecaId);
            _repoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                .ReturnsAsync(peca);

            await _service.DeletarAsync(pecaId);

            _repoMock.Verify(r => r.Remove(peca), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarArgumentException_QuandoPecaNaoExiste()
        {
            var pecaId = Guid.NewGuid();
            var dto = new PecaDto
            {
                PecaId = pecaId,
                Nome = "Filtro",
                Preco = 10m,
                Tipo = "Peca",
                Codigo = "COD",
                Descricao = "desc",
                Fabricante = "fab",
                QuantidadeMinima = 1
            };

            // repo returns null -> not found
            _repoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                     .ReturnsAsync((Peca?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(dto));
            Assert.Equal("Peça não encontrada.", ex.Message);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarArgumentException_QuandoPecaNaoExiste()
        {
            var pecaId = Guid.NewGuid();

            // repo returns null -> not found
            _repoMock.Setup(r => r.GetByIdAsync(pecaId, It.IsAny<Expression<Func<Peca, object>>[]>()))
                     .ReturnsAsync((Peca?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.DeletarAsync(pecaId));
            Assert.Equal("Peça não encontrada.", ex.Message);
        }
    }
}