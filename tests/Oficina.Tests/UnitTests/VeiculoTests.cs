using Oficina.Atendimento.Application.Dto;
using Oficina.Atendimento.Application.Services;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Domain.ValueObjects;
using Moq;
using System.Linq.Expressions;

namespace Oficina.Tests.UnitTests
{
    public class VeiculoTests
    {
        private readonly Mock<IVeiculoRepository> _repoMock;
        private readonly VeiculoAppService _service;

        public VeiculoTests()
        {
            _repoMock = new Mock<IVeiculoRepository>();
            _service = new VeiculoAppService(_repoMock.Object);
        }

        [Fact]
        public async Task CriarAsync_DeveRetornarId()
        {
            var dto = new VeiculoDto { Modelo = "Civic", Placa = "ABC1D23", Marca = "Honda", Ano = 2020, ClienteId = Guid.NewGuid() };
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Veiculo>()))
                .Callback<Veiculo>(v => v.GetType().GetProperty("Id")?.SetValue(v, Guid.NewGuid()))
                .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var id = await _service.CriarAsync(dto);

            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarVeiculoDto_QuandoExiste()
        {
            var veiculoId = Guid.NewGuid();
            var veiculo = new Veiculo("Civic", new Placa("ABC1D23"), "Honda", 2020, Guid.NewGuid());
            veiculo.GetType().GetProperty("Id")?.SetValue(veiculo, veiculoId);
            _repoMock.Setup(r => r.GetByIdAsync(veiculoId, It.IsAny<Expression<Func<Veiculo, object>>[]>()))
                .ReturnsAsync(veiculo);

            var result = await _service.ObterAsync(veiculoId);

            Assert.NotNull(result);
            Assert.Equal("Civic", result.Modelo);
            Assert.Equal("Honda", result.Marca);
            Assert.Equal(2020, result.Ano);
            Assert.Equal("ABC1D23", result.Placa);
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarNull_QuandoNaoExiste()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Veiculo)null);

            var result = await _service.ObterAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarListaDeVeiculos()
        {
            var veiculos = new List<Veiculo>
            {
                new Veiculo("Civic", new Placa("ABC1D23"), "Honda", 2020, Guid.NewGuid()),
                new Veiculo("Corolla", new Placa("ABC1D24"), "Toyota", 2022, Guid.NewGuid())
            };
            _repoMock.Setup(r => r.GetAllAsync(1, 10, null, It.IsAny<Expression<Func<Veiculo, object>>[]>()))
                .ReturnsAsync(veiculos);

            var result = await _service.ObterTodosAsync(1, 10);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, v => v.Modelo == "Civic");
            Assert.Contains(result, v => v.Modelo == "Corolla");
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarVeiculo()
        {
            var veiculoId = Guid.NewGuid();
            var veiculo = new Veiculo("Civic", new Placa("ABC1D23"), "Honda", 2020, Guid.NewGuid());
            veiculo.GetType().GetProperty("Id")?.SetValue(veiculo, veiculoId);
            var dto = new VeiculoDto { VeiculoId = veiculoId, Modelo = "Corolla", Placa = "ABC1D23", Marca = "Toyota", Ano = 2022 };
            _repoMock.Setup(r => r.GetByIdAsync(veiculoId, It.IsAny<Expression<Func<Veiculo, object>>[]>()))
                .ReturnsAsync(veiculo);

            await _service.AtualizarAsync(dto);

            Assert.Equal("Corolla", veiculo.Modelo);
            Assert.Equal("Toyota", veiculo.Marca);
            Assert.Equal(2022, veiculo.Ano);
            Assert.Equal("ABC1D23", veiculo.Placa.Numero);
        }

        [Fact]
        public async Task DeletarAsync_DeveRemoverVeiculo()
        {
            var veiculoId = Guid.NewGuid();
            var veiculo = new Veiculo("Civic", new Placa("ABC1D23"), "Honda", 2020, Guid.NewGuid());
            veiculo.GetType().GetProperty("Id")?.SetValue(veiculo, veiculoId);
            _repoMock.Setup(r => r.GetByIdAsync(veiculoId, It.IsAny<Expression<Func<Veiculo, object>>[]>()))
                .ReturnsAsync(veiculo);

            await _service.DeletarAsync(veiculoId);

            _repoMock.Verify(r => r.Remove(veiculo), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarArgumentException_QuandoVeiculoNaoExiste()
        {
            var veiculoId = Guid.NewGuid();
            var dto = new VeiculoDto { VeiculoId = veiculoId, Modelo = "Corolla", Placa = "ABC1D23", Marca = "Toyota", Ano = 2022 };

            _repoMock
                .Setup(r => r.GetByIdAsync(veiculoId, It.IsAny<Expression<Func<Veiculo, object>>[]>()))
                .ReturnsAsync((Veiculo)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(dto));
            Assert.Equal("Veículo não encontrado.", ex.Message);
        }

        [Fact]
        public async Task ObterTodosPorClienteAsync_DeveRetornarSomenteVeiculosDoCliente()
        {
            var clientId = Guid.NewGuid();
            var veiculos = new List<Veiculo>
            {
                new Veiculo("Civic", new Placa("ABC1D23"), "Honda", 2020, clientId),
                new Veiculo("Corolla", new Placa("ABC1D24"), "Toyota", 2022, Guid.NewGuid())
            };

            _repoMock
                .Setup(r => r.GetAllAsync(1, 10, It.IsAny<Expression<Func<Veiculo, bool>>>(), It.IsAny<Expression<Func<Veiculo, object>>[]>()))
                .ReturnsAsync(veiculos.Where(v => v.ClienteId == clientId));

            var result = await _service.ObterTodosPorClienteAsync(clientId);

            Assert.Single(result);
            Assert.All(result, v => Assert.Equal(clientId, v.ClienteId));
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarArgumentException_QuandoVeiculoNaoExiste()
        {
            var veiculoId = Guid.NewGuid();

            _repoMock
                .Setup(r => r.GetByIdAsync(veiculoId, It.IsAny<Expression<Func<Veiculo, object>>[]>()))
                .ReturnsAsync((Veiculo)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.DeletarAsync(veiculoId));
            Assert.Equal("Veículo não encontrado.", ex.Message);
        }
    }
}