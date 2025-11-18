using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oficina.Estoque.Domain.Enum;
using Oficina.Estoque.Domain.IRepositories;
using Oficina.Estoque.Infrastructure.Data;
using Oficina.Estoque.Infrastructure.Repositories;
using Oficina.Tests.Integration.Factory;

namespace Oficina.Tests.Integration.RepositoryIntegrationTests
{
    public class EstoqueRepositoryTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DbContextOptions<EstoqueDbContext> _dbContextOptions;

        public EstoqueRepositoryTests(CustomWebApplicationFactory factory)
        {
            _serviceProvider = factory.Services;
            _dbContextOptions = _serviceProvider
                .GetRequiredService<DbContextOptions<EstoqueDbContext>>();
        }

        private EstoqueDbContext CreateContext()
        {
            return new EstoqueDbContext(_dbContextOptions);
        }

        private IEstoqueRepository CreateRepository(EstoqueDbContext context)
        {
            return new EstoqueRepository(context);
        }

        private async Task<Guid> CriarPecaTeste(EstoqueDbContext context, string nome = "Peça Teste")
        {
            var peca = new Oficina.Estoque.Domain.Entities.Peca(
                nome,
                100.00m,
                TipoPeca.Peca,
                $"COD-{Guid.NewGuid():N}",
                "Descrição teste",
                "Fabricante Teste",
                5);
            peca.DataAtualizacao = DateTime.UtcNow;

            await context.Pecas.AddAsync(peca);
            await context.SaveChangesAsync();
            return peca.Id;
        }

        [Fact]
        public async Task AddAsync_DeveSalvarMovimentoEstoque_ComIdGerado()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var pecaId = await CriarPecaTeste(context);

            var movimentoEstoque = new Oficina.Estoque.Domain.Entities.Estoque(
                pecaId,
                TipoMovimentoEstoque.Entrada,
                10);

            await repository.AddAsync(movimentoEstoque);
            await repository.SaveChangesAsync();

            Assert.NotEqual(Guid.Empty, movimentoEstoque.Id);

            var savedEstoque = await context.Estoque
                .FirstOrDefaultAsync(e => e.Id == movimentoEstoque.Id);

            Assert.NotNull(savedEstoque);
            Assert.Equal(pecaId, savedEstoque.PecaId);
            Assert.Equal(TipoMovimentoEstoque.Entrada, savedEstoque.TipoMovimento);
            Assert.Equal(10, savedEstoque.Quantidade);
            Assert.NotEqual(default(DateTime), savedEstoque.DataMovimento);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarMovimento_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var pecaId = await CriarPecaTeste(context);

            var movimentoEstoque = new Oficina.Estoque.Domain.Entities.Estoque(
                pecaId,
                TipoMovimentoEstoque.Saida,
                5);

            await repository.AddAsync(movimentoEstoque);
            await repository.SaveChangesAsync();

            var encontrado = await repository.GetByIdAsync(movimentoEstoque.Id);

            Assert.NotNull(encontrado);
            Assert.Equal(movimentoEstoque.Id, encontrado.Id);
            Assert.Equal(pecaId, encontrado.PecaId);
            Assert.Equal(TipoMovimentoEstoque.Saida, encontrado.TipoMovimento);
            Assert.Equal(5, encontrado.Quantidade);
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
        public async Task GetAllAsync_DeveRetornarTodosMovimentos_ComPaginacao()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var pecaId = await CriarPecaTeste(context, "Peça Paginação Estoque");

            var existingMovimentos = await context.Estoque
                .Where(e => e.PecaId == pecaId)
                .ToListAsync();

            foreach (var m in existingMovimentos)
            {
                context.Estoque.Remove(m);
            }
            await context.SaveChangesAsync();

            for (int i = 1; i <= 12; i++)
            {
                var tipo = i % 2 == 0 ? TipoMovimentoEstoque.Entrada : TipoMovimentoEstoque.Saida;
                await repository.AddAsync(new Oficina.Estoque.Domain.Entities.Estoque(
                    pecaId,
                    tipo,
                    i));
            }
            await repository.SaveChangesAsync();

            var pagina1 = await repository.GetAllAsync(1, 5, e => e.PecaId == pecaId);
            var pagina2 = await repository.GetAllAsync(2, 5, e => e.PecaId == pecaId);
            var pagina3 = await repository.GetAllAsync(3, 5, e => e.PecaId == pecaId);

            Assert.Equal(5, pagina1.Count());
            Assert.Equal(5, pagina2.Count());
            Assert.Equal(2, pagina3.Count());
        }

        [Fact]
        public async Task GetAsync_DeveRetornarMovimento_QuandoAtendePredicate()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var pecaId = await CriarPecaTeste(context);

            var movimentoAjuste = new Oficina.Estoque.Domain.Entities.Estoque(
                pecaId,
                TipoMovimentoEstoque.Ajuste,
                15);

            await repository.AddAsync(movimentoAjuste);
            await repository.SaveChangesAsync();

            var encontrado = await repository.GetAsync(
                e => e.PecaId == pecaId && e.TipoMovimento == TipoMovimentoEstoque.Ajuste);

            Assert.NotNull(encontrado);
            Assert.Equal(TipoMovimentoEstoque.Ajuste, encontrado.TipoMovimento);
            Assert.Equal(15, encontrado.Quantidade);
        }

        [Fact]
        public async Task Update_DeveAtualizarMovimento_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var pecaId = await CriarPecaTeste(context);

            var movimento = new Oficina.Estoque.Domain.Entities.Estoque(
                pecaId,
                TipoMovimentoEstoque.Entrada,
                20);

            await repository.AddAsync(movimento);
            await repository.SaveChangesAsync();

            movimento.Quantidade = 25;

            repository.Update(movimento);
            await repository.SaveChangesAsync();

            using var novoContext = CreateContext();
            var atualizado = await novoContext.Estoque.FindAsync(movimento.Id);

            Assert.NotNull(atualizado);
            Assert.Equal(25, atualizado.Quantidade);
            Assert.NotNull(atualizado.DataAtualizacao);
        }

        [Fact]
        public async Task Remove_DeveRemoverMovimento_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var pecaId = await CriarPecaTeste(context);

            var movimento = new Oficina.Estoque.Domain.Entities.Estoque(
                pecaId,
                TipoMovimentoEstoque.Saida,
                8);

            await repository.AddAsync(movimento);
            await repository.SaveChangesAsync();

            repository.Remove(movimento);
            await repository.SaveChangesAsync();

            var removido = await context.Estoque.FindAsync(movimento.Id);
            Assert.Null(removido);
        }

        [Fact]
        public async Task GetAllAsync_ComFiltroPorTipoMovimento_DeveRetornarApenasMovimentosDoTipo()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var pecaId = await CriarPecaTeste(context, "Peça Filtro Tipo Movimento");

            await repository.AddAsync(new Oficina.Estoque.Domain.Entities.Estoque(pecaId, TipoMovimentoEstoque.Entrada, 10));
            await repository.AddAsync(new Oficina.Estoque.Domain.Entities.Estoque(pecaId, TipoMovimentoEstoque.Entrada, 20));
            await repository.AddAsync(new Oficina.Estoque.Domain.Entities.Estoque(pecaId, TipoMovimentoEstoque.Saida, 5));
            await repository.AddAsync(new Oficina.Estoque.Domain.Entities.Estoque(pecaId, TipoMovimentoEstoque.Ajuste, 3));

            await repository.SaveChangesAsync();

            var entradas = await repository.GetAllAsync(1, 20,
                e => e.PecaId == pecaId && e.TipoMovimento == TipoMovimentoEstoque.Entrada);
            var saidas = await repository.GetAllAsync(1, 20,
                e => e.PecaId == pecaId && e.TipoMovimento == TipoMovimentoEstoque.Saida);
            var ajustes = await repository.GetAllAsync(1, 20,
                e => e.PecaId == pecaId && e.TipoMovimento == TipoMovimentoEstoque.Ajuste);

            Assert.Equal(2, entradas.Count());
            Assert.Single(saidas);
            Assert.Single(ajustes);

            Assert.All(entradas, e => Assert.Equal(TipoMovimentoEstoque.Entrada, e.TipoMovimento));
            Assert.All(saidas, e => Assert.Equal(TipoMovimentoEstoque.Saida, e.TipoMovimento));
            Assert.All(ajustes, e => Assert.Equal(TipoMovimentoEstoque.Ajuste, e.TipoMovimento));
        }

        [Fact]
        public async Task GetAllAsync_ComFiltroPorPeriodo_DeveRetornarMovimentosNoPeriodo()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var pecaId = await CriarPecaTeste(context, "Peça Filtro Período");
            var dataInicio = DateTime.UtcNow.AddDays(-5);
            var dataFim = DateTime.UtcNow.AddDays(-1);

            var movimentoAntigo = new Oficina.Estoque.Domain.Entities.Estoque(pecaId, TipoMovimentoEstoque.Entrada, 10);
            var movimentoRecente = new Oficina.Estoque.Domain.Entities.Estoque(pecaId, TipoMovimentoEstoque.Saida, 5);

            await repository.AddAsync(movimentoAntigo);
            await repository.AddAsync(movimentoRecente);
            await repository.SaveChangesAsync();

            var movimentosRecentes = await repository.GetAllAsync(1, 20,
                e => e.PecaId == pecaId && e.DataMovimento >= dataInicio);

            Assert.True(movimentosRecentes.Any());
            Assert.All(movimentosRecentes, m => Assert.True(m.DataMovimento >= dataInicio));
        }

        [Fact]
        public async Task GetAllAsync_ComFiltroPorQuantidade_DeveRetornarMovimentosComQuantidadeMaiorQue()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var pecaId = await CriarPecaTeste(context, "Peça Filtro Quantidade");

            await repository.AddAsync(new Oficina.Estoque.Domain.Entities.Estoque(pecaId, TipoMovimentoEstoque.Entrada, 5));
            await repository.AddAsync(new Oficina.Estoque.Domain.Entities.Estoque(pecaId, TipoMovimentoEstoque.Entrada, 15));
            await repository.AddAsync(new Oficina.Estoque.Domain.Entities.Estoque(pecaId, TipoMovimentoEstoque.Entrada, 25));

            await repository.SaveChangesAsync();

            var movimentosGrandes = await repository.GetAllAsync(1, 20,
                e => e.PecaId == pecaId && e.Quantidade > 10);

            Assert.Equal(2, movimentosGrandes.Count());
            Assert.All(movimentosGrandes, m => Assert.True(m.Quantidade > 10));
        }
    }
}