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
    public class VeiculoRepositoryTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DbContextOptions<AtendimentoDbContext> _dbContextOptions;

        public VeiculoRepositoryTests(CustomWebApplicationFactory factory)
        {
            _serviceProvider = factory.Services;

            // Obtém as mesmas opções de conexão que a fábrica usa
            _dbContextOptions = _serviceProvider
                .GetRequiredService<DbContextOptions<AtendimentoDbContext>>();
        }

        private AtendimentoDbContext CreateContext()
        {
            return new AtendimentoDbContext(_dbContextOptions);
        }

        private IVeiculoRepository CreateRepository(AtendimentoDbContext context)
        {
            return new VeiculoRepository(context);
        }

        private IClienteRepository CreateClienteRepository(AtendimentoDbContext context)
        {
            return new ClienteRepository(context);
        }

        private async Task<Guid> CriarClienteTeste(AtendimentoDbContext context)
        {
            // Criar um cliente para associar aos veículos
            var clienteRepo = CreateClienteRepository(context);
            var cliente = new Cliente(new CpfCnpj("42975620063"), "Cliente Teste Veículos", "clienteveiculos@teste.com");
            await clienteRepo.AddAsync(cliente);
            await clienteRepo.SaveChangesAsync();
            return cliente.Id;
        }

        [Fact]
        public async Task AddAsync_DeveSalvarVeiculo_ComIdGerado()
        {
            // Arrange
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var clienteId = await CriarClienteTeste(context);

            var veiculo = new Veiculo(
                "Modelo Teste",
                new Placa("ABC-1234"),
                "Marca Teste",
                2022,
                clienteId);

            // Act
            await repository.AddAsync(veiculo);
            await repository.SaveChangesAsync();

            // Assert
            Assert.NotEqual(Guid.Empty, veiculo.Id);

            // Verificação adicional: consultar o banco para confirmar que foi salvo
            var savedVeiculo = await context.Veiculos
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.Id == veiculo.Id);

            Assert.NotNull(savedVeiculo);
            Assert.Equal("Modelo Teste", savedVeiculo.Modelo);
            Assert.Equal("Marca Teste", savedVeiculo.Marca);
            Assert.Equal(2022, savedVeiculo.Ano);
            Assert.Equal("ABC-1234", savedVeiculo.Placa.Numero);
            Assert.Equal(clienteId, savedVeiculo.ClienteId);
            Assert.NotNull(savedVeiculo.Cliente);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarVeiculo_QuandoExiste()
        {
            // Arrange
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var clienteId = await CriarClienteTeste(context);

            var veiculo = new Veiculo(
                "Modelo Recuperação",
                new Placa("DEF-5678"),
                "Marca Recuperação",
                2023,
                clienteId);

            await repository.AddAsync(veiculo);
            await repository.SaveChangesAsync();

            // Act
            var encontrado = await repository.GetByIdAsync(veiculo.Id);

            // Assert
            Assert.NotNull(encontrado);
            Assert.Equal(veiculo.Id, encontrado.Id);
            Assert.Equal("Modelo Recuperação", encontrado.Modelo);
            Assert.Equal("Marca Recuperação", encontrado.Marca);
            Assert.Equal(2023, encontrado.Ano);
            Assert.Equal("DEF-5678", encontrado.Placa.Numero);
            Assert.Equal(clienteId, encontrado.ClienteId);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNull_QuandoNaoExiste()
        {
            // Arrange
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var idInexistente = Guid.NewGuid();

            // Act
            var resultado = await repository.GetByIdAsync(idInexistente);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodosVeiculos_ComPaginacao()
        {
            // Arrange
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var clienteId = await CriarClienteTeste(context);

            // Limpar veículos existentes que possam interferir no teste
            var existingVeiculos = await context.Veiculos
                .Where(v => v.Modelo.StartsWith("Veículo Paginação"))
                .ToListAsync();

            foreach (var v in existingVeiculos)
            {
                context.Veiculos.Remove(v);
            }
            await context.SaveChangesAsync();

            // Adicionar veículos para teste de paginação
            for (int i = 1; i <= 12; i++)
            {
                var placa = new Placa($"PAG-{i.ToString().PadLeft(4, '0')}");
                await repository.AddAsync(new Veiculo(
                    $"Veículo Paginação {i}",
                    placa,
                    "Marca Teste",
                    2020 + i % 5,
                    clienteId));
            }
            await repository.SaveChangesAsync();

            // Act
            var pagina1 = await repository.GetAllAsync(1, 5,
                v => v.Modelo.StartsWith("Veículo Paginação"));
            var pagina2 = await repository.GetAllAsync(2, 5,
                v => v.Modelo.StartsWith("Veículo Paginação"));
            var pagina3 = await repository.GetAllAsync(3, 5,
                v => v.Modelo.StartsWith("Veículo Paginação"));

            // Assert
            Assert.Equal(5, pagina1.Count());
            Assert.Equal(5, pagina2.Count());
            Assert.Equal(2, pagina3.Count());

            // Verificar conteúdo das páginas
            Assert.Contains(pagina1, v => v.Modelo == "Veículo Paginação 1");
            Assert.Contains(pagina2, v => v.Modelo == "Veículo Paginação 6");
            Assert.Contains(pagina3, v => v.Modelo == "Veículo Paginação 11");
        }

        [Fact]
        public async Task GetAllByClienteIdAsync_DeveRetornarVeiculosDoCliente()
        {
            // Arrange
            using var context = CreateContext();
            var repository = CreateRepository(context);

            // Criar dois clientes diferentes
            var cliente1Id = await CriarClienteTeste(context);

            // Criar cliente 2 manualmente para ter um nome diferente
            var clienteRepo = CreateClienteRepository(context);
            var cliente2 = new Cliente(new CpfCnpj("06292215024"), "Cliente 2 Teste", "cliente2@teste.com");
            await clienteRepo.AddAsync(cliente2);
            await clienteRepo.SaveChangesAsync();
            var cliente2Id = cliente2.Id;

            // Adicionar veículos para o cliente 1
            for (int i = 1; i <= 3; i++)
            {
                var placa = new Placa($"CZV-{i.ToString().PadLeft(4, '0')}");
                await repository.AddAsync(new Veiculo(
                    $"Veículo Cliente 1 - {i}",
                    placa,
                    "Marca C1",
                    2020,
                    cliente1Id));
            }

            // Adicionar veículos para o cliente 2
            for (int i = 1; i <= 2; i++)
            {
                var placa = new Placa($"CBV-{i.ToString().PadLeft(4, '0')}");
                await repository.AddAsync(new Veiculo(
                    $"Veículo Cliente 2 - {i}",
                    placa,
                    "Marca C2",
                    2021,
                    cliente2Id));
            }

            await repository.SaveChangesAsync();

            // Act            
            var veiculosCliente1 = await repository.GetAllAsync(1, 50, x => x.ClienteId == cliente1Id);
            var veiculosCliente2 = await repository.GetAllAsync(1, 50, x => x.ClienteId == cliente2Id);

            // Assert
            Assert.Equal(3, veiculosCliente1.Count());
            Assert.Equal(2, veiculosCliente2.Count());

            // Verificar que os veículos estão associados aos clientes corretos
            Assert.All(veiculosCliente1, v => Assert.Equal(cliente1Id, v.ClienteId));
            Assert.All(veiculosCliente2, v => Assert.Equal(cliente2Id, v.ClienteId));

            // Verificar conteúdo específico
            Assert.Contains(veiculosCliente1, v => v.Modelo == "Veículo Cliente 1 - 1");
            Assert.Contains(veiculosCliente2, v => v.Modelo == "Veículo Cliente 2 - 1");
        }

        [Fact]
        public async Task Update_DeveAtualizarVeiculo_QuandoExiste()
        {
            // Arrange
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var clienteId = await CriarClienteTeste(context);

            var veiculo = new Veiculo(
                "Modelo Original",
                new Placa("UPD-1234"),
                "Marca Original",
                2020,
                clienteId);

            await repository.AddAsync(veiculo);
            await repository.SaveChangesAsync();

            // Modificar o veículo
            veiculo.Atualizar("Modelo Atualizado", new Placa("UPD-1235"), "Marca Atualizada", 2021);

            // Act
            repository.Update(veiculo);
            await repository.SaveChangesAsync();

            // Assert - Usar um contexto novo para garantir que estamos buscando do banco
            using var novoContext = CreateContext();
            var atualizado = await novoContext.Veiculos.FindAsync(veiculo.Id);

            Assert.NotNull(atualizado);
            Assert.Equal("Modelo Atualizado", atualizado.Modelo);
            Assert.Equal("Marca Atualizada", atualizado.Marca);
            Assert.Equal(2021, atualizado.Ano);
            Assert.Equal("UPD-1235", atualizado.Placa.Numero); // Placa não deve mudar
            Assert.Equal(clienteId, atualizado.ClienteId);      // ClienteId não deve mudar
        }

        [Fact]
        public async Task Remove_DeveRemoverVeiculo_QuandoExiste()
        {
            // Arrange
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var clienteId = await CriarClienteTeste(context);

            var veiculo = new Veiculo(
                "Modelo para Remover",
                new Placa("REM-9876"),
                "Marca para Remover",
                2022,
                clienteId);

            await repository.AddAsync(veiculo);
            await repository.SaveChangesAsync();

            // Act
            repository.Remove(veiculo);
            await repository.SaveChangesAsync();

            // Assert
            var removido = await context.Veiculos.FindAsync(veiculo.Id);
            Assert.Null(removido);
        }

        [Fact]
        public async Task GetByPlacaAsync_DeveRetornarVeiculo_QuandoExiste()
        {
            // Arrange
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var clienteId = await CriarClienteTeste(context);

            var placa = new Placa("PLC-5555");
            var veiculo = new Veiculo(
                "Modelo Busca por Placa",
                placa,
                "Marca Teste",
                2023,
                clienteId);

            await repository.AddAsync(veiculo);
            await repository.SaveChangesAsync();

            var encontrado = await repository.GetAsync(X =>X.Placa.Numero == "PLC-5555");

            // Assert
            Assert.NotNull(encontrado);
            Assert.Equal("Modelo Busca por Placa", encontrado.Modelo);
            Assert.Equal("PLC-5555", encontrado.Placa.Numero);
        }

        [Fact]
        public async Task DeletarPorClienteIdAsync_DeveRemoverTodosVeiculosDoCliente()
        {
            // Arrange
            using var context = CreateContext();
            var repository = CreateRepository(context);

            // Criar um cliente para o teste
            var clienteId = await CriarClienteTeste(context);

            // Adicionar vários veículos para este cliente
            var veiculos = new List<Veiculo>();
            for (int i = 1; i <= 5; i++)
            {
                var placa = new Placa($"DEL-{i.ToString().PadLeft(4, '0')}");
                var veiculo = new Veiculo(
                    $"Veículo para Delete {i}",
                    placa,
                    "Marca Teste",
                    2020 + i,
                    clienteId);

                await repository.AddAsync(veiculo);
                veiculos.Add(veiculo);
            }

            // Criar um segundo cliente com alguns veículos para verificar que não são removidos
            var clienteRepo = CreateClienteRepository(context);
            var cliente2 = new Cliente(new CpfCnpj("06292215024"), "Cliente 2 Para Não Excluir", "naoexcluir@teste.com");
            await clienteRepo.AddAsync(cliente2);
            await clienteRepo.SaveChangesAsync();
            var cliente2Id = cliente2.Id;

            // Adicionar veículos para o segundo cliente
            for (int i = 1; i <= 3; i++)
            {
                var placa = new Placa($"NDL-{i.ToString().PadLeft(4, '0')}");
                await repository.AddAsync(new Veiculo(
                    $"Veículo para Não Excluir {i}",
                    placa,
                    "Marca 2",
                    2019 + i,
                    cliente2Id));
            }

            await repository.SaveChangesAsync();

            // Verificar antes da exclusão que existem veículos para ambos os clientes
            var veiculosCliente1Antes = await repository.GetAllAsync(1, 20, v => v.ClienteId == clienteId);
            var veiculosCliente2Antes = await repository.GetAllAsync(1, 20, v => v.ClienteId == cliente2Id);

            Assert.Equal(5, veiculosCliente1Antes.Count());
            Assert.Equal(3, veiculosCliente2Antes.Count());

            // Act - Executar o método para excluir todos os veículos do cliente 1
            var quantidadeRemovida = await repository.DeletarPorClienteIdAsync(clienteId);

            // Assert - Verificar que todos os veículos do cliente 1 foram removidos
            Assert.Equal(5, quantidadeRemovida); // Deve retornar o número de veículos removidos

            // Verificar que não existem mais veículos para o cliente 1
            var veiculosCliente1Depois = await repository.GetAllAsync(1, 20, v => v.ClienteId == clienteId);
            Assert.Empty(veiculosCliente1Depois);

            // Verificar que os veículos do cliente 2 permaneceram intactos
            var veiculosCliente2Depois = await repository.GetAllAsync(1, 20, v => v.ClienteId == cliente2Id);
            Assert.Equal(3, veiculosCliente2Depois.Count());
        }

        [Fact]
        public async Task DeletarPorClienteIdAsync_DeveRetornarZero_QuandoNaoHaVeiculos()
        {
            // Arrange
            using var context = CreateContext();
            var repository = CreateRepository(context);

            // Criar um cliente sem veículos
            var clienteId = await CriarClienteTeste(context);

            // Act - Tentar excluir veículos de um cliente que não tem veículos
            var quantidadeRemovida = await repository.DeletarPorClienteIdAsync(clienteId);

            // Assert
            Assert.Equal(0, quantidadeRemovida);
        }

        [Fact]
        public async Task DeletarPorClienteIdAsync_DeveRetornarZero_QuandoClienteNaoExiste()
        {
            // Arrange
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var clienteIdInexistente = Guid.NewGuid();

            // Act - Tentar excluir veículos de um cliente que não existe
            var quantidadeRemovida = await repository.DeletarPorClienteIdAsync(clienteIdInexistente);

            // Assert
            Assert.Equal(0, quantidadeRemovida);
        }
    }
}