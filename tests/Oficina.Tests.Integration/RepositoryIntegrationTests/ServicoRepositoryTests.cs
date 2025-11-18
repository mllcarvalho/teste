using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Infrastructure.Data;
using Oficina.Atendimento.Infrastructure.Repositories;
using Oficina.Tests.Integration.Factory;

namespace Oficina.Tests.Integration.RepositoryIntegrationTests
{
    public class ServicoRepositoryTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DbContextOptions<AtendimentoDbContext> _dbContextOptions;

        public ServicoRepositoryTests(CustomWebApplicationFactory factory)
        {
            _serviceProvider = factory.Services;
            _dbContextOptions = _serviceProvider
                .GetRequiredService<DbContextOptions<AtendimentoDbContext>>();
        }

        private AtendimentoDbContext CreateContext()
        {
            return new AtendimentoDbContext(_dbContextOptions);
        }

        private IServicoRepository CreateRepository(AtendimentoDbContext context)
        {
            return new ServicoRepository(context);
        }

        [Fact]
        public async Task AddAsync_DeveSalvarServico_ComIdGerado()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var servico = new Servico("Teste de Serviço", 123.45m);

            await repository.AddAsync(servico);
            await repository.SaveChangesAsync();

            Assert.NotEqual(Guid.Empty, servico.Id);

            var savedServico = await context.Servicos.FirstOrDefaultAsync(s => s.Id == servico.Id);
            Assert.NotNull(savedServico);
            Assert.Equal("Teste de Serviço", savedServico.Nome);
            Assert.Equal(123.45m, savedServico.Preco);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarServico_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var servico = new Servico("Serviço de Teste", 99.99m);
            await repository.AddAsync(servico);
            await repository.SaveChangesAsync();

            var encontrado = await repository.GetByIdAsync(servico.Id);

            Assert.NotNull(encontrado);
            Assert.Equal(servico.Id, encontrado.Id);
            Assert.Equal("Serviço de Teste", encontrado.Nome);
            Assert.Equal(99.99m, encontrado.Preco);
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
        public async Task GetAllAsync_DeveRetornarTodosServicos_ComPaginacao()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var existingServicos = await context.Servicos
                .Where(s => s.Nome.StartsWith("Serviço Paginação"))
                .ToListAsync();

            foreach (var s in existingServicos)
            {
                context.Servicos.Remove(s);
            }
            await context.SaveChangesAsync();

            // Adicionar serviços para teste de paginação
            for (int i = 1; i <= 15; i++)
            {
                await repository.AddAsync(new Servico($"Serviço Paginação {i}", i * 10));
            }
            await repository.SaveChangesAsync();

            // Act
            var pagina1 = await repository.GetAllAsync(1, 5,
                s => s.Nome.StartsWith("Serviço Paginação"));
            var pagina2 = await repository.GetAllAsync(2, 5,
                s => s.Nome.StartsWith("Serviço Paginação"));
            var pagina3 = await repository.GetAllAsync(3, 5,
                s => s.Nome.StartsWith("Serviço Paginação"));
            var pagina4 = await repository.GetAllAsync(4, 5,
                s => s.Nome.StartsWith("Serviço Paginação"));

            // Assert
            Assert.Equal(5, pagina1.Count());
            Assert.Equal(5, pagina2.Count());
            Assert.Equal(5, pagina3.Count());
            Assert.Empty(pagina4);

            // Verificar conteúdo da primeira página
            Assert.Contains(pagina1, s => s.Nome == "Serviço Paginação 1");
            Assert.Contains(pagina1, s => s.Nome == "Serviço Paginação 5");

            // Verificar conteúdo da segunda página
            Assert.Contains(pagina2, s => s.Nome == "Serviço Paginação 6");
            Assert.Contains(pagina2, s => s.Nome == "Serviço Paginação 10");

            // Verificar conteúdo da terceira página
            Assert.Contains(pagina3, s => s.Nome == "Serviço Paginação 11");
            Assert.Contains(pagina3, s => s.Nome == "Serviço Paginação 15");
        }

        [Fact]
        public async Task Update_DeveAtualizarServico_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var servico = new Servico("Serviço Original", 50m);
            await repository.AddAsync(servico);
            await repository.SaveChangesAsync();

            servico.Atualizar("Serviço Atualizado", 75.5m);
            repository.Update(servico);
            await repository.SaveChangesAsync();

            using var novoContext = CreateContext();
            var atualizado = await novoContext.Servicos.FindAsync(servico.Id);

            Assert.NotNull(atualizado);
            Assert.Equal("Serviço Atualizado", atualizado.Nome);
            Assert.Equal(75.5m, atualizado.Preco);
            Assert.NotEqual(atualizado.DataCriacao, atualizado.DataAtualizacao);
        }

        [Fact]
        public async Task Remove_DeveRemoverServico_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var servico = new Servico("Serviço para Remover", 30m);
            await repository.AddAsync(servico);
            await repository.SaveChangesAsync();

            repository.Remove(servico);
            await repository.SaveChangesAsync();

            var removido = await context.Servicos.FindAsync(servico.Id);
            Assert.Null(removido);
        }

        [Fact]
        public async Task GetAsync_DeveRetornarServico_QuandoAtendePredicate()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var nome = $"Serviço Único {Guid.NewGuid()}";
            var servico = new Servico(nome, 44.44m);
            await repository.AddAsync(servico);
            await repository.SaveChangesAsync();

            var encontrado = await repository.GetAsync(s => s.Nome == nome);

            Assert.NotNull(encontrado);
            Assert.Equal(nome, encontrado.Nome);
            Assert.Equal(44.44m, encontrado.Preco);
        }
    }
}