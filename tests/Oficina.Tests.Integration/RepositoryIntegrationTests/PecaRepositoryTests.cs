using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oficina.Estoque.Domain.Entities;
using Oficina.Estoque.Domain.Enum;
using Oficina.Estoque.Domain.IRepositories;
using Oficina.Estoque.Infrastructure.Data;
using Oficina.Estoque.Infrastructure.Repositories;
using Oficina.Tests.Integration.Factory;

namespace Oficina.Tests.Integration.RepositoryIntegrationTests
{
    public class PecaRepositoryTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DbContextOptions<EstoqueDbContext> _dbContextOptions;

        public PecaRepositoryTests(CustomWebApplicationFactory factory)
        {
            _serviceProvider = factory.Services;
            _dbContextOptions = _serviceProvider
                .GetRequiredService<DbContextOptions<EstoqueDbContext>>();
        }

        private EstoqueDbContext CreateContext()
        {
            return new EstoqueDbContext(_dbContextOptions);
        }

        private IPecaRepository CreateRepository(EstoqueDbContext context)
        {
            return new PecaRepository(context);
        }

        [Fact]
        public async Task AddAsync_DeveSalvarPeca_ComIdGerado()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var peca = new Peca(
                "Filtro de Óleo",
                45.90m,
                TipoPeca.Peca,
                "FILTRO-001",
                "Filtro de óleo sintético premium",
                "Bosch",
                5);

            await repository.AddAsync(peca);
            await repository.SaveChangesAsync();

            Assert.NotEqual(Guid.Empty, peca.Id);

            var savedPeca = await context.Pecas.FirstOrDefaultAsync(p => p.Id == peca.Id);
            Assert.NotNull(savedPeca);
            Assert.Equal("Filtro de Óleo", savedPeca.Nome);
            Assert.Equal(45.90m, savedPeca.Preco);
            Assert.Equal(TipoPeca.Peca, savedPeca.Tipo);
            Assert.Equal("FILTRO-001", savedPeca.Codigo);
            Assert.Equal("Filtro de óleo sintético premium", savedPeca.Descricao);
            Assert.Equal("Bosch", savedPeca.Fabricante);
            Assert.Equal(5, savedPeca.QuantidadeMinima);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarPeca_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var peca = new Peca(
                "Pastilha de Freio",
                120.00m,
                TipoPeca.Peca,
                "PAST-001",
                "Pastilha de freio cerâmica",
                "Frasle",
                3);

            await repository.AddAsync(peca);
            await repository.SaveChangesAsync();

            var encontrado = await repository.GetByIdAsync(peca.Id);

            Assert.NotNull(encontrado);
            Assert.Equal(peca.Id, encontrado.Id);
            Assert.Equal("Pastilha de Freio", encontrado.Nome);
            Assert.Equal(120.00m, encontrado.Preco);
            Assert.Equal(TipoPeca.Peca, encontrado.Tipo);
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
        public async Task GetAllAsync_DeveRetornarTodasPecas_ComPaginacao()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var existingPecas = await context.Pecas
                .Where(p => p.Nome.StartsWith("Peça Paginação"))
                .ToListAsync();

            foreach (var p in existingPecas)
            {
                context.Pecas.Remove(p);
            }
            await context.SaveChangesAsync();

            for (int i = 1; i <= 12; i++)
            {
                await repository.AddAsync(new Peca(
                    $"Peça Paginação {i}",
                    i * 10.50m,
                    TipoPeca.Peca,
                    $"PAG-{i:D3}",
                    $"Descrição da peça {i}",
                    "Fabricante Teste",
                    2));
            }
            await repository.SaveChangesAsync();

            var pagina1 = await repository.GetAllAsync(1, 5,
                p => p.Nome.StartsWith("Peça Paginação"));
            var pagina2 = await repository.GetAllAsync(2, 5,
                p => p.Nome.StartsWith("Peça Paginação"));
            var pagina3 = await repository.GetAllAsync(3, 5,
                p => p.Nome.StartsWith("Peça Paginação"));

            Assert.Equal(5, pagina1.Count());
            Assert.Equal(5, pagina2.Count());
            Assert.Equal(2, pagina3.Count());
            Assert.Contains(pagina1, p => p.Nome == "Peça Paginação 1");
            Assert.Contains(pagina2, p => p.Nome == "Peça Paginação 6");
            Assert.Contains(pagina3, p => p.Nome == "Peça Paginação 11");
        }

        [Fact]
        public async Task GetAsync_DeveRetornarPeca_QuandoAtendePredicate()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var codigo = $"UNIQUE-{Guid.NewGuid():N}";
            var peca = new Peca(
                "Peça Única",
                99.99m,
                TipoPeca.Peca,
                codigo,
                "Descrição única",
                "Fabricante",
                1);

            await repository.AddAsync(peca);
            await repository.SaveChangesAsync();

            var encontrado = await repository.GetAsync(p => p.Codigo == codigo);

            Assert.NotNull(encontrado);
            Assert.Equal(codigo, encontrado.Codigo);
            Assert.Equal("Peça Única", encontrado.Nome);
        }

        [Fact]
        public async Task Update_DeveAtualizarPeca_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var peca = new Peca(
                "Amortecedor Original",
                250.00m,
                TipoPeca.Peca,
                "AMORT-001",
                "Amortecedor dianteiro",
                "Monroe",
                2);

            await repository.AddAsync(peca);
            await repository.SaveChangesAsync();

            peca.Atualizar(
                "Amortecedor Atualizado",
                300.00m,
                TipoPeca.Peca,
                "AMORT-002",
                "Amortecedor dianteiro premium",
                "Cofap",
                3);

            repository.Update(peca);
            await repository.SaveChangesAsync();

            using var novoContext = CreateContext();
            var atualizado = await novoContext.Pecas.FindAsync(peca.Id);

            Assert.NotNull(atualizado);
            Assert.Equal("Amortecedor Atualizado", atualizado.Nome);
            Assert.Equal(300.00m, atualizado.Preco);
            Assert.Equal("AMORT-002", atualizado.Codigo);
            Assert.Equal("Amortecedor dianteiro premium", atualizado.Descricao);
            Assert.Equal("Cofap", atualizado.Fabricante);
            Assert.Equal(3, atualizado.QuantidadeMinima);
            Assert.NotNull(atualizado.DataAtualizacao);
        }

        [Fact]
        public async Task Remove_DeveRemoverPeca_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var peca = new Peca(
                "Peça para Remover",
                15.00m,
                TipoPeca.Insumo,
                "REM-001",
                "Será removida",
                "Teste",
                1);

            await repository.AddAsync(peca);
            await repository.SaveChangesAsync();

            repository.Remove(peca);
            await repository.SaveChangesAsync();

            var removido = await context.Pecas.FindAsync(peca.Id);
            Assert.Null(removido);
        }

        [Fact]
        public async Task AdicionarQuantidade_DeveAumentarEstoque()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var peca = new Peca(
                "Óleo de Motor",
                35.00m,
                TipoPeca.Insumo,
                "OLEO-001",
                "Óleo sintético 5W30",
                "Castrol",
                10);

            await repository.AddAsync(peca);
            await repository.SaveChangesAsync();

            peca.AdicionarQuantidade(20);
            repository.Update(peca);
            await repository.SaveChangesAsync();

            using var novoContext = CreateContext();
            var atualizado = await novoContext.Pecas.FindAsync(peca.Id);

            Assert.NotNull(atualizado);
            Assert.Equal(20, atualizado.Quantidade);
        }

        [Fact]
        public async Task RemoverQuantidade_DeveDiminuirEstoque()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var peca = new Peca(
                "Correia Dentada",
                80.00m,
                TipoPeca.Peca,
                "CORR-001",
                "Correia dentada alta performance",
                "Gates",
                2);

            peca.AdicionarQuantidade(15);
            await repository.AddAsync(peca);
            await repository.SaveChangesAsync();

            peca.RemoverQuantidade(5);
            repository.Update(peca);
            await repository.SaveChangesAsync();

            using var novoContext = CreateContext();
            var atualizado = await novoContext.Pecas.FindAsync(peca.Id);

            Assert.NotNull(atualizado);
            Assert.Equal(10, atualizado.Quantidade);
        }

        [Fact]
        public async Task GetAllAsync_ComFiltroPorTipo_DeveRetornarApenasPecasDoTipo()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var existingPecas = await context.Pecas
                .Where(p => p.Nome.StartsWith("Filtro Tipo"))
                .ToListAsync();

            foreach (var p in existingPecas)
            {
                context.Pecas.Remove(p);
            }
            await context.SaveChangesAsync();
            await repository.AddAsync(new Peca("Filtro Tipo Peça 1", 10m, TipoPeca.Peca, "FTP1", "Desc", "Fab", 1));
            await repository.AddAsync(new Peca("Filtro Tipo Peça 2", 20m, TipoPeca.Peca, "FTP2", "Desc", "Fab", 1));
            await repository.AddAsync(new Peca("Filtro Tipo Insumo 1", 5m, TipoPeca.Insumo, "FTI1", "Desc", "Fab", 1));
            await repository.AddAsync(new Peca("Filtro Tipo Insumo 2", 7m, TipoPeca.Insumo, "FTI2", "Desc", "Fab", 1));

            await repository.SaveChangesAsync();

            var pecas = await repository.GetAllAsync(1, 20,
                p => p.Nome.StartsWith("Filtro Tipo") && p.Tipo == TipoPeca.Peca);
            var insumos = await repository.GetAllAsync(1, 20,
                p => p.Nome.StartsWith("Filtro Tipo") && p.Tipo == TipoPeca.Insumo);

            Assert.Equal(2, pecas.Count());
            Assert.Equal(2, insumos.Count());
            Assert.All(pecas, p => Assert.Equal(TipoPeca.Peca, p.Tipo));
            Assert.All(insumos, i => Assert.Equal(TipoPeca.Insumo, i.Tipo));
        }

        [Fact]
        public async Task GetAllAsync_ComFiltroPorFabricante_DeveRetornarApenasPecasDoFabricante()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var fabricanteUnico = $"Fabricante-{Guid.NewGuid():N}";

            await repository.AddAsync(new Peca("Peça Fab 1", 10m, TipoPeca.Peca, "PF1", "Desc", fabricanteUnico, 1));
            await repository.AddAsync(new Peca("Peça Fab 2", 20m, TipoPeca.Peca, "PF2", "Desc", fabricanteUnico, 1));
            await repository.AddAsync(new Peca("Peça Outro", 30m, TipoPeca.Peca, "PO1", "Desc", "Outro Fabricante", 1));
            await repository.SaveChangesAsync();

            var pecasFabricante = await repository.GetAllAsync(1, 20,
                p => p.Fabricante == fabricanteUnico);

            Assert.Equal(2, pecasFabricante.Count());
            Assert.All(pecasFabricante, p => Assert.Equal(fabricanteUnico, p.Fabricante));
        }
    }
}