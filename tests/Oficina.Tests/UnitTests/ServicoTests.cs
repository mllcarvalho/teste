using Oficina.Atendimento.Application.Dto;
using Oficina.Atendimento.Application.Services;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Moq;
using System.Linq.Expressions;

namespace Oficina.Tests.UnitTests
{
    public class ServicoTests
    {
        private readonly Mock<IServicoRepository> _repoMock;

        private readonly ServicoAppService _service;

        public ServicoTests()
        {
            _repoMock = new Mock<IServicoRepository>();
            _service = new ServicoAppService(_repoMock.Object);
        }

        [Fact]
        public async Task CriarAsync_DeveRetornarId()
        {
            var dto = new ServicoDto { Nome = "Troca de óleo", Preco = 100m };
            // Simule que o Servico criado tem um Id válido
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Servico>()))
                .Callback<Servico>(s => s.GetType().GetProperty("Id")?.SetValue(s, Guid.NewGuid()))
                .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var id = await _service.CriarAsync(dto);

            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarServicoDto_QuandoExiste()
        {
            var servicoId = Guid.NewGuid();
            var servico = new Servico("Troca de óleo", 100m);
            servico.GetType().GetProperty("Id")?.SetValue(servico, servicoId);
            _repoMock.Setup(r => r.GetByIdAsync(servicoId, It.IsAny<Expression<Func<Servico, object>>[]>()))
             .ReturnsAsync(servico);

            var result = await _service.ObterAsync(servicoId);

            Assert.NotNull(result);
            Assert.Equal("Troca de óleo", result.Nome);
            Assert.Equal(100m, result.Preco);
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarNull_QuandoNaoExiste()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Servico)null);

            var result = await _service.ObterAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarListaDeServicos()
        {
            var servicos = new List<Servico>
            {
                new Servico("Troca de óleo", 100m),
                new Servico("Alinhamento", 150m)
            };
            _repoMock.Setup(r => r.GetAllAsync(1, 10, null, It.IsAny<Expression<Func<Servico, object>>[]>()))
                .ReturnsAsync(servicos);

            var result = await _service.ObterTodosAsync(1, 10);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, s => s.Nome == "Troca de óleo");
            Assert.Contains(result, s => s.Nome == "Alinhamento");
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarServico()
        {
            var servicoId = Guid.NewGuid();
            var servico = new Servico("Troca de óleo", 100m);
            servico.GetType().GetProperty("Id")?.SetValue(servico, servicoId);
            var dto = new ServicoDto { ServicoId = servicoId, Nome = "Alinhamento", Preco = 150m };
            _repoMock.Setup(r => r.GetByIdAsync(servicoId, It.IsAny<Expression<Func<Servico, object>>[]>()))
             .ReturnsAsync(servico);

            _repoMock
             .Setup(r => r.SaveChangesAsync())
             .Callback(() => servico.DataAtualizacao = DateTime.UtcNow)
             .Returns(Task.CompletedTask);

            await _service.AtualizarAsync(dto);

            Assert.Equal("Alinhamento", servico.Nome);
            Assert.Equal(150m, servico.Preco);
        }

        [Fact]
        public async Task DeletarAsync_DeveRemoverServico()
        {
            var servicoId = Guid.NewGuid();
            var servico = new Servico("Troca de óleo", 100m);
            servico.GetType().GetProperty("Id")?.SetValue(servico, servicoId);
            _repoMock.Setup(r => r.GetByIdAsync(servicoId, It.IsAny<Expression<Func<Servico, object>>[]>()))
              .ReturnsAsync(servico);

            await _service.DeletarAsync(servicoId);

            _repoMock.Verify(r => r.Remove(servico), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
        
        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoServicoNaoExiste()
        {
            var servicoId = Guid.NewGuid();
            var dto = new ServicoDto { ServicoId = servicoId, Nome = "Teste", Preco = 100m };
            _repoMock.Setup(r => r.GetByIdAsync(servicoId, It.IsAny<Expression<Func<Servico, object>>[]>()))
                .ReturnsAsync((Servico)null);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.AtualizarAsync(dto));

            Assert.Equal("Serviço não encontrada.", exception.Message);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarExcecao_QuandoServicoNaoExiste()
        {
            var servicoId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(servicoId, It.IsAny<Expression<Func<Servico, object>>[]>()))
                .ReturnsAsync((Servico)null);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.DeletarAsync(servicoId));

            Assert.Equal("Serviço não encontrado.", exception.Message);
        }
    }
}