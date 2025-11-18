using Moq;
using Oficina.Atendimento.Application.Dto;
using Oficina.Atendimento.Application.Services;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Domain.ValueObjects;
using System.Linq.Expressions;
namespace Oficina.Tests.UnitTests
{
    public class ClienteTests
    {
        private readonly Mock<IClienteRepository> _clienteRepoMock;
        private readonly Mock<IVeiculoRepository> _veiculoRepoMock;
        private readonly ClienteAppService _service;

        public ClienteTests()
        {
            _clienteRepoMock = new Mock<IClienteRepository>();
            _veiculoRepoMock = new Mock<IVeiculoRepository>();
            _service = new ClienteAppService(_clienteRepoMock.Object, _veiculoRepoMock.Object);
        }

        [Fact]
        public async Task CriarAsync_DeveRetornarId()
        {
            var dto = new CriarClienteDto { Nome = "João", Email = "joao@email.com", Documento = "529.982.247-25" };
            _clienteRepoMock.Setup(r => r.AddAsync(It.IsAny<Cliente>()))
                .Callback<Cliente>(c => c.GetType().GetProperty("Id")?.SetValue(c, Guid.NewGuid()))
                .Returns(Task.CompletedTask);
            _clienteRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var id = await _service.CriarAsync(dto);

            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarClienteDto_QuandoExiste()
        {
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente(new CpfCnpj("529.982.247-25"), "João", "joao@email.com");
            cliente.GetType().GetProperty("Id")?.SetValue(cliente, clienteId);
            _clienteRepoMock.Setup(r => r.GetByIdAsync(clienteId, It.IsAny<Expression<Func<Cliente, object>>[]>()))
                .ReturnsAsync(cliente);

            var result = await _service.ObterAsync(clienteId);

            Assert.NotNull(result);
            Assert.Equal("João", result.Nome);
            Assert.Equal("joao@email.com", result.Email);
            Assert.Equal("52998224725", result.Documento);
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarNull_QuandoNaoExiste()
        {
            _clienteRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Cliente)null);

            var result = await _service.ObterAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarListaDeClientes()
        {
            var clientes = new List<Cliente>
            {
                new Cliente(new CpfCnpj("52998224725"), "João", "joao@email.com"),
                new Cliente(new CpfCnpj("12345678909"), "Maria", "maria@email.com")
            };
            _clienteRepoMock.Setup(r => r.GetAllAsync(1, 10, null, It.IsAny<Expression<Func<Cliente, object>>[]>()))
                .ReturnsAsync(clientes);

            var result = await _service.ObterTodosAsync(1, 10);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Nome == "João");
            Assert.Contains(result, c => c.Nome == "Maria");
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarCliente()
        {
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente(new CpfCnpj("529.982.247-25"), "João", "joao@email.com");
            cliente.GetType().GetProperty("Id")?.SetValue(cliente, clienteId);
            var dto = new ClienteDto { ClienteId = clienteId, Nome = "Maria", Email = "maria@email.com", Documento = "52998224725" };
            _clienteRepoMock.Setup(r => r.GetByIdAsync(clienteId, It.IsAny<Expression<Func<Cliente, object>>[]>()))
                .ReturnsAsync(cliente);

            _clienteRepoMock
                .Setup(r => r.SaveChangesAsync())
                .Callback(() => cliente.DataAtualizacao = DateTime.UtcNow)
                .Returns(Task.CompletedTask);

            await _service.AtualizarAsync(dto);

            Assert.Equal("Maria", cliente.Nome);
            Assert.Equal("maria@email.com", cliente.Email);
            Assert.Equal("52998224725", cliente.Documento.Numero);
        }

        [Fact]
        public async Task DeletarAsync_DeveRemoverCliente()
        {
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente(new CpfCnpj("529.982.247-25"), "João", "joao@email.com");
            cliente.GetType().GetProperty("Id")?.SetValue(cliente, clienteId);
            _clienteRepoMock.Setup(r => r.GetByIdAsync(clienteId, It.IsAny<Expression<Func<Cliente, object>>[]>()))
                .ReturnsAsync(cliente);

            await _service.DeletarAsync(clienteId);

            _clienteRepoMock.Verify(r => r.Remove(cliente), Times.Once);
            _clienteRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_DeveFalhar_SeDocumentoJaExiste()
        {
            var dto = new CriarClienteDto { Nome = "João", Email = "joao@email.com", Documento = "52998224725" };
            _clienteRepoMock.Setup(r => r.ExisteDocumentoAsync(dto.Documento, null)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
            Assert.Equal("Já existe um cliente com este documento.", ex.Message);
        }

        [Fact]
        public async Task CriarAsync_DeveFalhar_SeEmailJaExiste()
        {
            var dto = new CriarClienteDto { Nome = "João", Email = "joao@email.com", Documento = "52998224725" };
            _clienteRepoMock.Setup(r => r.ExisteDocumentoAsync(dto.Documento, null)).ReturnsAsync(false);
            _clienteRepoMock.Setup(r => r.ExisteEmailAsync(dto.Email, null)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
            Assert.Equal("Já existe um cliente com este e-mail.", ex.Message);
        }

        [Fact]
        public async Task AtualizarAsync_DeveFalhar_SeDocumentoJaExiste()
        {
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente(new CpfCnpj("52998224725"), "João", "joao@email.com");
            cliente.GetType().GetProperty("Id")?.SetValue(cliente, clienteId);
            var dto = new ClienteDto { ClienteId = clienteId, Nome = "Maria", Email = "maria@email.com", Documento = "52998224725" };

            _clienteRepoMock.Setup(r => r.GetByIdAsync(clienteId, It.IsAny<Expression<Func<Cliente, object>>[]>()))
                .ReturnsAsync(cliente);
            _clienteRepoMock.Setup(r => r.ExisteDocumentoAsync(dto.Documento, clienteId)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(dto));
            Assert.Equal("Já existe um cliente com este documento.", ex.Message);
        }

        [Fact]
        public async Task AtualizarAsync_DeveFalhar_SeEmailJaExiste()
        {
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente(new CpfCnpj("52998224725"), "João", "joao@email.com");
            cliente.GetType().GetProperty("Id")?.SetValue(cliente, clienteId);
            var dto = new ClienteDto { ClienteId = clienteId, Nome = "Maria", Email = "maria@email.com", Documento = "52998224725" };

            _clienteRepoMock.Setup(r => r.GetByIdAsync(clienteId, It.IsAny<Expression<Func<Cliente, object>>[]>()))
                .ReturnsAsync(cliente);
            _clienteRepoMock.Setup(r => r.ExisteDocumentoAsync(dto.Documento, clienteId)).ReturnsAsync(false);
            _clienteRepoMock.Setup(r => r.ExisteEmailAsync(dto.Email, clienteId)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(dto));
            Assert.Equal("Já existe um cliente com este e-mail.", ex.Message);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoClienteNaoExiste()
        {
            var clienteId = Guid.NewGuid();
            var dto = new ClienteDto
            {
                ClienteId = clienteId,
                Nome = "João Silva",
                Email = "joao@teste.com",
                Documento = "12345678900"
            };

            _clienteRepoMock.Setup(r => r.GetByIdAsync(clienteId, It.IsAny<Expression<Func<Cliente, object>>[]>()))
                .ReturnsAsync((Cliente)null);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.AtualizarAsync(dto));

            Assert.Equal("Cliente não encontrado.", exception.Message);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarExcecao_QuandoClienteNaoExiste()
        {
            var clienteId = Guid.NewGuid();
            _clienteRepoMock.Setup(r => r.GetByIdAsync(clienteId, It.IsAny<Expression<Func<Cliente, object>>[]>()))
                .ReturnsAsync((Cliente)null);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.DeletarAsync(clienteId));

            Assert.Equal("Cliente não encontrado.", exception.Message);
        }
    }
}