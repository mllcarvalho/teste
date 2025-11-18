
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.Enum;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Domain.ValueObjects;
using Oficina.Atendimento.Infrastructure.Data;
using Oficina.Atendimento.Infrastructure.Repositories;
using Oficina.Tests.Integration.Factory;

namespace Oficina.Tests.Integration.RepositoryIntegrationTests
{
    public class OrdemDeServicoRepositoryTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DbContextOptions<AtendimentoDbContext> _dbContextOptions;

        public OrdemDeServicoRepositoryTests(CustomWebApplicationFactory factory)
        {
            _serviceProvider = factory.Services;
            _dbContextOptions = _serviceProvider
                .GetRequiredService<DbContextOptions<AtendimentoDbContext>>();
        }

        private AtendimentoDbContext CreateContext()
        {
            return new AtendimentoDbContext(_dbContextOptions);
        }

        private IOrdemDeServicoRepository CreateRepository(AtendimentoDbContext context)
        {
            return new OrdemRepository(context);
        }

        private async Task<(Cliente cliente, Veiculo veiculo)> CriarClienteEVeiculoAsync(AtendimentoDbContext context)
        {
            var cpf = new CpfCnpj("22536260070");
            var cliente = new Cliente(cpf, "Cliente Teste", "cliente@teste.com");
            cliente.DataAtualizacao = DateTime.UtcNow;
            await context.Clientes.AddAsync(cliente);

            var placa = new Placa("FZB-4303");
            var veiculo = new Veiculo("Civic", placa, "Honda", 2020, cliente.Id);
            veiculo.DataAtualizacao = DateTime.UtcNow;  
            await context.Veiculos.AddAsync(veiculo);

            await context.SaveChangesAsync();
            return (cliente, veiculo);
        }

        [Fact]
        public async Task AddAsync_DeveSalvarOrdemDeServico_ComIdGerado()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var (cliente, veiculo) = await CriarClienteEVeiculoAsync(context);

            var ordem = new OrdemDeServico(cliente.Id, veiculo.Id);

            await repository.AddAsync(ordem);
            await repository.SaveChangesAsync();

            Assert.NotEqual(Guid.Empty, ordem.Id);

            var savedOrdem = await context.OrdensDeServico.FirstOrDefaultAsync(o => o.Id == ordem.Id);
            Assert.NotNull(savedOrdem);
            Assert.Equal(cliente.Id, savedOrdem.ClienteId);
            Assert.Equal(veiculo.Id, savedOrdem.VeiculoId);
            Assert.Equal(OrdemStatus.Recebida, savedOrdem.Status);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarOrdemDeServico_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var (cliente, veiculo) = await CriarClienteEVeiculoAsync(context);

            var ordem = new OrdemDeServico(cliente.Id, veiculo.Id);
            await repository.AddAsync(ordem);
            await repository.SaveChangesAsync();

            var encontrada = await repository.GetByIdAsync(ordem.Id);

            Assert.NotNull(encontrada);
            Assert.Equal(ordem.Id, encontrada.Id);
            Assert.Equal(cliente.Id, encontrada.ClienteId);
            Assert.Equal(veiculo.Id, encontrada.VeiculoId);
            Assert.Equal(OrdemStatus.Recebida, encontrada.Status);
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
        public async Task GetAllAsync_DeveRetornarTodasOrdensDeServico_ComPaginacao()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var (cliente, veiculo) = await CriarClienteEVeiculoAsync(context);

            // Limpar ordens de teste anteriores
            var existingOrdens = await context.OrdensDeServico
                .Where(o => o.ClienteId == cliente.Id)
                .ToListAsync();

            foreach (var o in existingOrdens)
            {
                context.OrdensDeServico.Remove(o);
            }
            await context.SaveChangesAsync();

            // Adicionar ordens para teste de paginação
            for (int i = 1; i <= 12; i++)
            {
                await repository.AddAsync(new OrdemDeServico(cliente.Id, veiculo.Id));
            }
            await repository.SaveChangesAsync();

            // Act
            var pagina1 = await repository.GetAllAsync(1, 5, o => o.ClienteId == cliente.Id);
            var pagina2 = await repository.GetAllAsync(2, 5, o => o.ClienteId == cliente.Id);
            var pagina3 = await repository.GetAllAsync(3, 5, o => o.ClienteId == cliente.Id);

            // Assert
            Assert.Equal(5, pagina1.Count());
            Assert.Equal(5, pagina2.Count());
            Assert.Equal(2, pagina3.Count());
        }

        [Fact]
        public async Task Update_DeveAtualizarOrdemDeServico_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var (cliente, veiculo) = await CriarClienteEVeiculoAsync(context);

            var ordem = new OrdemDeServico(cliente.Id, veiculo.Id);
            await repository.AddAsync(ordem);
            await repository.SaveChangesAsync();

            ordem.IniciarDiagnostico();
            repository.Update(ordem);
            await repository.SaveChangesAsync();

            using var novoContext = CreateContext();
            var atualizada = await novoContext.OrdensDeServico.FindAsync(ordem.Id);

            Assert.NotNull(atualizada);
            Assert.Equal(OrdemStatus.EmDiagnostico, atualizada.Status);
        }

        [Fact]
        public async Task Remove_DeveRemoverOrdemDeServico_QuandoExiste()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var (cliente, veiculo) = await CriarClienteEVeiculoAsync(context);

            var ordem = new OrdemDeServico(cliente.Id, veiculo.Id);
            await repository.AddAsync(ordem);
            await repository.SaveChangesAsync();

            repository.Remove(ordem);
            await repository.SaveChangesAsync();

            var removida = await context.OrdensDeServico.FindAsync(ordem.Id);
            Assert.Null(removida);
        }

        [Fact]
        public async Task GetAsync_DeveRetornarOrdemDeServico_QuandoAtendePredicate()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var (cliente, veiculo) = await CriarClienteEVeiculoAsync(context);

            var ordem = new OrdemDeServico(cliente.Id, veiculo.Id);
            await repository.AddAsync(ordem);
            await repository.SaveChangesAsync();

            var encontrada = await repository.GetAsync(o => o.ClienteId == cliente.Id && o.VeiculoId == veiculo.Id);

            Assert.NotNull(encontrada);
            Assert.Equal(ordem.Id, encontrada.Id);
            Assert.Equal(cliente.Id, encontrada.ClienteId);
            Assert.Equal(veiculo.Id, encontrada.VeiculoId);
        }

        [Fact]
        public async Task ObterFinalizadasAsync_DeveRetornarApenasOrdensFinalizadas()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var (cliente, veiculo) = await CriarClienteEVeiculoAsync(context);

            // Criar ordens com diferentes status
            var ordemRecebida = new OrdemDeServico(cliente.Id, veiculo.Id);
            await repository.AddAsync(ordemRecebida);

            var ordemFinalizada1 = new OrdemDeServico(cliente.Id, veiculo.Id);
            ordemFinalizada1.IniciarDiagnostico();
            ordemFinalizada1.ConcluirDiagnostico();
            ordemFinalizada1.IniciarExcucao();
            ordemFinalizada1.FinalizarExecucao();
            await repository.AddAsync(ordemFinalizada1);

            var ordemFinalizada2 = new OrdemDeServico(cliente.Id, veiculo.Id);
            ordemFinalizada2.IniciarDiagnostico();
            ordemFinalizada2.ConcluirDiagnostico();
            ordemFinalizada2.IniciarExcucao();
            ordemFinalizada2.FinalizarExecucao();
            await repository.AddAsync(ordemFinalizada2);

            var ordemEmExecucao = new OrdemDeServico(cliente.Id, veiculo.Id);
            ordemEmExecucao.IniciarDiagnostico();
            ordemEmExecucao.ConcluirDiagnostico();
            ordemEmExecucao.IniciarExcucao();
            await repository.AddAsync(ordemEmExecucao);

            await repository.SaveChangesAsync();

            var finalizadas = await repository.ObterFinalizadasAsync();

            Assert.NotNull(finalizadas);
            Assert.Contains(finalizadas, o => o.Id == ordemFinalizada1.Id);
            Assert.Contains(finalizadas, o => o.Id == ordemFinalizada2.Id);
            Assert.DoesNotContain(finalizadas, o => o.Id == ordemRecebida.Id);
            Assert.DoesNotContain(finalizadas, o => o.Id == ordemEmExecucao.Id);
        }

        [Fact]
        public async Task ObterFinalizadasAsync_DeveRetornarListaVazia_QuandoNaoExistemFinalizadas()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var (cliente, veiculo) = await CriarClienteEVeiculoAsync(context);

            var ordem = new OrdemDeServico(cliente.Id, veiculo.Id);
            await repository.AddAsync(ordem);
            await repository.SaveChangesAsync();

            var finalizadas = await repository.ObterFinalizadasAsync();

            Assert.NotNull(finalizadas);
            Assert.DoesNotContain(finalizadas, o => o.Id == ordem.Id);
        }

        [Fact]
        public async Task AddAsync_DeveSalvarOrdemComServicosEPecas()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var (cliente, veiculo) = await CriarClienteEVeiculoAsync(context);

            var ordem = new OrdemDeServico(cliente.Id, veiculo.Id);
            var servico = new OrdemServicoItemServico(Guid.NewGuid(), "Troca de Óleo", 150.00m);
            var peca = new OrdemServicoPeca(Guid.NewGuid(), "Filtro de Óleo", 2, 45.00m);

            ordem.AdicionarServico(servico);
            ordem.AdicionarPeca(peca);
            ordem.DefinirValorTotal(150.00m, 90.00m);

            await repository.AddAsync(ordem);
            await repository.SaveChangesAsync();

            var savedOrdem = await context.OrdensDeServico
                .Include(o => o.Servicos)
                .Include(o => o.Pecas)
                .FirstOrDefaultAsync(o => o.Id == ordem.Id);

            Assert.NotNull(savedOrdem);
            Assert.Single(savedOrdem.Servicos);
            Assert.Single(savedOrdem.Pecas);
            Assert.Equal(240.00m, savedOrdem.CustoTotal);
        }

        [Fact]
        public async Task Update_DeveAtualizarFluxoCompletoDeStatus()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var (cliente, veiculo) = await CriarClienteEVeiculoAsync(context);

            var ordem = new OrdemDeServico(cliente.Id, veiculo.Id);
            await repository.AddAsync(ordem);
            await repository.SaveChangesAsync();

            // Recebida -> EmDiagnostico
            ordem.IniciarDiagnostico();
            repository.Update(ordem);
            await repository.SaveChangesAsync();
            Assert.Equal(OrdemStatus.EmDiagnostico, ordem.Status);

            // EmDiagnostico -> AguardandoAprovacao
            ordem.ConcluirDiagnostico();
            repository.Update(ordem);
            await repository.SaveChangesAsync();
            Assert.Equal(OrdemStatus.AguardandoAprovacao, ordem.Status);

            // AguardandoAprovacao -> EmExecucao
            ordem.IniciarExcucao();
            repository.Update(ordem);
            await repository.SaveChangesAsync();
            Assert.Equal(OrdemStatus.EmExecucao, ordem.Status);
            Assert.NotNull(ordem.DataInicioExecucao);

            // EmExecucao -> Finalizada
            ordem.FinalizarExecucao();
            repository.Update(ordem);
            await repository.SaveChangesAsync();
            Assert.Equal(OrdemStatus.Finalizada, ordem.Status);
            Assert.NotNull(ordem.DataFinalizacao);
            Assert.NotNull(ordem.TempoExecucao);

            // Finalizada -> Entregue
            ordem.Entregar();
            repository.Update(ordem);
            await repository.SaveChangesAsync();
            Assert.Equal(OrdemStatus.Entregue, ordem.Status);
            Assert.NotNull(ordem.DataConclusao);
        }

        [Fact]
        public async Task GetByIdAsync_DeveCarregarNavegacoes_QuandoIncludes()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var (cliente, veiculo) = await CriarClienteEVeiculoAsync(context);

            var ordem = new OrdemDeServico(cliente.Id, veiculo.Id);
            await repository.AddAsync(ordem);
            await repository.SaveChangesAsync();

            var encontrada = await repository.GetByIdAsync(ordem.Id, o => o.Cliente, o => o.Veiculo);

            Assert.NotNull(encontrada);
            Assert.NotNull(encontrada.Cliente);
            Assert.NotNull(encontrada.Veiculo);
            Assert.Equal(cliente.Nome, encontrada.Cliente.Nome);
            Assert.Equal(veiculo.Modelo, encontrada.Veiculo.Modelo);
        }
    }
}
