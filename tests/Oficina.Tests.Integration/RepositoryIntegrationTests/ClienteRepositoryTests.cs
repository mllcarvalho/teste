using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Domain.ValueObjects;
using Oficina.Atendimento.Infrastructure.Data;
using Oficina.Atendimento.Infrastructure.Repositories;
using Oficina.Tests.Integration.Factory;

namespace Oficina.Tests.Integration.RepositoryIntegrationTests
{
    public class ClienteRepositoryTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DbContextOptions<AtendimentoDbContext> _dbContextOptions;

        public ClienteRepositoryTests(CustomWebApplicationFactory factory)
        {
            _serviceProvider = factory.Services;
            _dbContextOptions = _serviceProvider
                .GetRequiredService<DbContextOptions<AtendimentoDbContext>>();
        }

        private AtendimentoDbContext CreateContext()
        {
            return new AtendimentoDbContext(_dbContextOptions);
        }

        private IClienteRepository CreateRepository(AtendimentoDbContext context)
        {
            return new ClienteRepository(context);
        }

        [Fact]
        public async Task AddAsync_DeveSalvarCliente_ComIdGerado()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var cliente = new Cliente(
                new CpfCnpj("529.982.247-25"),
                "João Silva",
                "joao.silva@teste.com");

            await repository.AddAsync(cliente);
            await repository.SaveChangesAsync();

            Assert.NotEqual(Guid.Empty, cliente.Id);

            var savedCliente = await context.Clientes.FirstOrDefaultAsync(c => c.Id == cliente.Id);
            Assert.NotNull(savedCliente);
            Assert.Equal("João Silva", savedCliente.Nome);
            Assert.Equal("joao.silva@teste.com", savedCliente.Email);
            Assert.Equal("52998224725", savedCliente.Documento.Numero);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarCliente_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var cliente = new Cliente(
                new CpfCnpj("123.456.789-09"),
                "Maria Santos",
                "maria.santos@teste.com");

            await repository.AddAsync(cliente);
            await repository.SaveChangesAsync();

            var encontrado = await repository.GetByIdAsync(cliente.Id);

            Assert.NotNull(encontrado);
            Assert.Equal(cliente.Id, encontrado.Id);
            Assert.Equal("Maria Santos", encontrado.Nome);
            Assert.Equal("maria.santos@teste.com", encontrado.Email);
            Assert.Equal("12345678909", encontrado.Documento.Numero);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNull_QuandoNaoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var idInexistente = Guid.NewGuid();

            var resultado = await repository.GetByIdAsync(idInexistente);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task GetAsync_DeveRetornarCliente_QuandoAtendePredicate()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var emailUnico = $"cliente.unico.{Guid.NewGuid():N}@teste.com";
            var cliente = new Cliente(
                new CpfCnpj("987.654.321-00"),
                "Cliente Único",
                emailUnico);

            await repository.AddAsync(cliente);
            await repository.SaveChangesAsync();

            var encontrado = await repository.GetAsync(c => c.Email == emailUnico);

            Assert.NotNull(encontrado);
            Assert.Equal(emailUnico, encontrado.Email);
            Assert.Equal("Cliente Único", encontrado.Nome);
        }

        [Fact]
        public async Task Update_DeveAtualizarCliente_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var cliente = new Cliente(
                new CpfCnpj("768.833.650-36"),
                "Nome Original",
                "original@teste.com");

            await repository.AddAsync(cliente);
            await repository.SaveChangesAsync();

            cliente.Atualizar(
                "Nome Atualizado",
                "atualizado@teste.com",
                new CpfCnpj("772.028.410-02"));

            repository.Update(cliente);
            await repository.SaveChangesAsync();

            using var novoContext = CreateContext();
            var atualizado = await novoContext.Clientes.FindAsync(cliente.Id);

            Assert.NotNull(atualizado);
            Assert.Equal("Nome Atualizado", atualizado.Nome);
            Assert.Equal("atualizado@teste.com", atualizado.Email);
            Assert.Equal("77202841002", atualizado.Documento.Numero);
            Assert.NotNull(atualizado.DataAtualizacao);
        }

        [Fact]
        public async Task Remove_DeveRemoverCliente_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var cliente = new Cliente(
                new CpfCnpj("768.833.650-36"),
                "Cliente para Remover",
                "remover@teste.com");

            await repository.AddAsync(cliente);
            await repository.SaveChangesAsync();

            repository.Remove(cliente);
            await repository.SaveChangesAsync();

            var removido = await context.Clientes.FindAsync(cliente.Id);
            Assert.Null(removido);
        }

        [Fact]
        public async Task ExisteEmailAsync_DeveRetornarTrue_QuandoEmailExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var emailExistente = $"email.existe.{Guid.NewGuid():N}@teste.com";
            var cliente = new Cliente(
                new CpfCnpj("836.893.810-45"),
                "Cliente Email Teste",
                emailExistente);

            await repository.AddAsync(cliente);
            await repository.SaveChangesAsync();

            var existe = await repository.ExisteEmailAsync(emailExistente);

            Assert.True(existe);
        }

        [Fact]
        public async Task ExisteEmailAsync_DeveRetornarFalse_QuandoEmailNaoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var existe = await repository.ExisteEmailAsync("email.nao.existe@teste.com");

            Assert.False(existe);
        }

        [Fact]
        public async Task ExisteEmailAsync_DeveIgnorarClienteEspecifico_QuandoIgnoreIdInformado()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var emailTeste = $"email.ignore.{Guid.NewGuid():N}@teste.com";
            var cliente1 = new Cliente(
                new CpfCnpj("836.893.810-45"),
                "Cliente 1",
                emailTeste);

            await repository.AddAsync(cliente1);
            await repository.SaveChangesAsync();

            var existe = await repository.ExisteEmailAsync(emailTeste, cliente1.Id);

            Assert.False(existe);
        }

        [Fact]
        public async Task ExisteEmailAsync_DeveRetornarTrue_QuandoOutroClienteTemMesmoEmail()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var emailCompartilhado = $"email.compartilhado.{Guid.NewGuid():N}@teste.com";
            var cliente1 = new Cliente(
                new CpfCnpj("836.893.810-45"),
                "Cliente 1",
                emailCompartilhado);
            var cliente2 = new Cliente(
                new CpfCnpj("713.166.570-35"),
                "Cliente 2",
                emailCompartilhado);

            await repository.AddAsync(cliente1);
            await repository.AddAsync(cliente2);
            await repository.SaveChangesAsync();

            var existe = await repository.ExisteEmailAsync(emailCompartilhado, cliente1.Id);

            Assert.True(existe);
        }

        [Fact]
        public async Task ExisteDocumentoAsync_DeveRetornarTrue_QuandoDocumentoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var documentoExistente = "836.893.810-45";
            var cliente = new Cliente(
                new CpfCnpj(documentoExistente),
                "Cliente Documento Teste",
                $"doc.teste.{Guid.NewGuid():N}@teste.com");

            await repository.AddAsync(cliente);
            await repository.SaveChangesAsync();

            var existe = await repository.ExisteDocumentoAsync("83689381045");

            Assert.True(existe);
        }

        [Fact]
        public async Task ExisteDocumentoAsync_DeveRetornarFalse_QuandoDocumentoNaoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var existe = await repository.ExisteDocumentoAsync("99999999999");

            Assert.False(existe);
        }

        [Fact]
        public async Task ExisteDocumentoAsync_DeveRetornarTrue_QuandoOutroClienteTemMesmoDocumento()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var documentoCompartilhado = "836.893.810-45";
            var cliente1 = new Cliente(
                new CpfCnpj(documentoCompartilhado),
                "Cliente 1",
                $"doc.compartilhado1.{Guid.NewGuid():N}@teste.com");
            var cliente2 = new Cliente(
                new CpfCnpj(documentoCompartilhado),
                "Cliente 2",
                $"doc.compartilhado2.{Guid.NewGuid():N}@teste.com");

            await repository.AddAsync(cliente1);
            await repository.AddAsync(cliente2);
            await repository.SaveChangesAsync();

            var existe = await repository.ExisteDocumentoAsync("83689381045", cliente1.Id);

            Assert.True(existe);
        }

        [Fact]
        public async Task GetAllAsync_ComFiltroDeNome_DeveRetornarClientesFiltrados()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var prefixoUnico = $"ClienteFiltro{Guid.NewGuid():N}";

            await repository.AddAsync(new Cliente(
                new CpfCnpj("713.166.570-35"),
                $"{prefixoUnico} Silva",
                $"{prefixoUnico}1@teste.com"));
            await repository.AddAsync(new Cliente(
                new CpfCnpj("435.880.240-00"),
                $"{prefixoUnico} Santos",
                $"{prefixoUnico}2@teste.com"));
            await repository.AddAsync(new Cliente(
                new CpfCnpj("768.833.650-36"),
                "Outro Nome",
                "outro@teste.com"));

            await repository.SaveChangesAsync();

            var clientesFiltrados = await repository.GetAllAsync(1, 20,
                c => c.Nome.StartsWith(prefixoUnico));

            Assert.Equal(2, clientesFiltrados.Count());
            Assert.All(clientesFiltrados, c => Assert.StartsWith(prefixoUnico, c.Nome));
        }
    }
}