using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.Enum;
using Oficina.Atendimento.Domain.ValueObjects;

namespace Oficina.Tests.Atendimento.Domain.Entities
{
    public class OrdemDeServicoTests
    {
        private readonly Guid _clienteId = Guid.NewGuid();
        private readonly Guid _veiculoId = Guid.NewGuid();

        [Fact]
        public void Construtor_DeveCriarOrdemComStatusAberto_QuandoChamado()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);

            Assert.Equal(_clienteId, ordem.ClienteId);
            Assert.Equal(_veiculoId, ordem.VeiculoId);
            Assert.Equal(OrdemStatus.Recebida, ordem.Status);
            Assert.Equal(0, ordem.CustoTotal);
            Assert.Empty(ordem.Pecas);
            Assert.Empty(ordem.Servicos);
        }

        [Fact]
        public void AdicionarPeca_DeveAdicionarPecaALista_QuandoChamado()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);
            var peca = new OrdemServicoPeca(
                Guid.NewGuid(),
                "Filtro de Óleo",
                2,
                50);

            ordem.AdicionarPeca(peca);

            Assert.Single(ordem.Pecas);
            Assert.Contains(peca, ordem.Pecas);
        }

        [Fact]
        public void AdicionarServico_DeveAdicionarServicoALista_QuandoChamado()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);
            var servico = new OrdemServicoItemServico(
                Guid.NewGuid(),
                "Troca de Óleo",
                100);

            ordem.AdicionarServico(servico);

            Assert.Single(ordem.Servicos);
            Assert.Contains(servico, ordem.Servicos);
        }

        [Fact]
        public void AdicionarMultiplosPecasEServicos_DeveAdicionarCorretamente_QuandoChamado()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);
            var peca1 = new OrdemServicoPeca(Guid.NewGuid(), "Filtro de Óleo", 1, 50);
            var peca2 = new OrdemServicoPeca(Guid.NewGuid(), "Óleo Motor", 4, 30);
            var servico1 = new OrdemServicoItemServico(Guid.NewGuid(), "Troca de Óleo", 80);
            var servico2 = new OrdemServicoItemServico(Guid.NewGuid(), "Revisão Geral", 150);

            ordem.AdicionarPeca(peca1);
            ordem.AdicionarPeca(peca2);
            ordem.AdicionarServico(servico1);
            ordem.AdicionarServico(servico2);

            Assert.Equal(2, ordem.Pecas.Count);
            Assert.Equal(2, ordem.Servicos.Count);
            Assert.Contains(peca1, ordem.Pecas);
            Assert.Contains(peca2, ordem.Pecas);
            Assert.Contains(servico1, ordem.Servicos);
            Assert.Contains(servico2, ordem.Servicos);
        }

        [Fact]
        public void IniciarDiagnostico_DeveMudarParaEmDiagnostico()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);

            ordem.IniciarDiagnostico();

            Assert.Equal(OrdemStatus.EmDiagnostico, ordem.Status);  
            Assert.Null(ordem.DataInicioExecucao);
            Assert.Null(ordem.DataFinalizacao);
            Assert.Null(ordem.DataConclusao);
        }

        [Fact]
        public void ConcluirDiagnostico_DeveMudarParaAguardandoAprovacao_QuandoEmDiagnostico()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);

            ordem.IniciarDiagnostico();
            ordem.ConcluirDiagnostico();

            Assert.Equal(OrdemStatus.AguardandoAprovacao, ordem.Status);
            Assert.Null(ordem.DataInicioExecucao);
        }

        [Fact]
        public void ConcluirDiagnostico_DeveLancarException_QuandoNaoEstiverEmDiagnostico()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);

            var ex = Assert.Throws<InvalidOperationException>(() => ordem.ConcluirDiagnostico());
            Assert.Contains(nameof(OrdemStatus.EmDiagnostico), ex.Message);
        }

        [Fact]
        public void IniciarExcucao_DeveDefinirDataInicioEStatus_QuandoAguardandoAprovacao()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);

            ordem.IniciarDiagnostico();
            ordem.ConcluirDiagnostico();
            ordem.IniciarExcucao();

            Assert.Equal(OrdemStatus.EmExecucao, ordem.Status);
            Assert.True(ordem.DataInicioExecucao.HasValue);
            Assert.InRange(ordem.DataInicioExecucao.Value, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
        }

        [Fact]
        public void IniciarExcucao_DeveLancar_QuandoNaoAguardandoAprovacao()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);

            var ex = Assert.Throws<InvalidOperationException>(() => ordem.IniciarExcucao());
            Assert.Contains(nameof(OrdemStatus.AguardandoAprovacao), ex.Message);
        }

        [Fact]
        public void FinalizarExecucao_DeveDefinirDataFinalizacao_E_TempoExecucao_QuandoEmExecucao()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);

            ordem.IniciarDiagnostico();
            ordem.ConcluirDiagnostico();
            ordem.IniciarExcucao();

            var start = DateTime.UtcNow.AddMinutes(-30);
            
            ordem.GetType().GetProperty("DataInicioExecucao")?.SetValue(ordem, start);
            ordem.FinalizarExecucao();

            Assert.Equal(OrdemStatus.Finalizada, ordem.Status);
            Assert.True(ordem.DataFinalizacao.HasValue);
            Assert.True(ordem.TempoExecucao.HasValue);
            var minutos = ordem.TempoExecucao.Value.TotalMinutes;
            Assert.InRange(minutos, 29.0, 31.0);
        }

        [Fact]
        public void FinalizarExecucao_DeveLancarException_QuandoNaoEmExecucao()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);

            var ex = Assert.Throws<InvalidOperationException>(() => ordem.FinalizarExecucao());
            Assert.Contains(nameof(OrdemStatus.EmExecucao), ex.Message);
        }

        [Fact]
        public void Entregar_DeveDefinirDataConclusaoEStatus_QuandoFinalizada()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);

            ordem.IniciarDiagnostico();
            ordem.ConcluirDiagnostico();
            ordem.IniciarExcucao();
            ordem.GetType().GetProperty("DataInicioExecucao")?.SetValue(ordem, DateTime.UtcNow.AddHours(-2));
            ordem.FinalizarExecucao();
            ordem.Entregar();

            Assert.Equal(OrdemStatus.Entregue, ordem.Status);
            Assert.True(ordem.DataConclusao.HasValue);
            Assert.InRange(ordem.DataConclusao.Value, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
        }

        [Fact]
        public void Entregar_DeveLancarException_QuandoNaoFinalizada()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);

            var ex = Assert.Throws<InvalidOperationException>(() => ordem.Entregar());
            Assert.Contains(nameof(OrdemStatus.Finalizada), ex.Message);
        }

        [Fact]
        public void AtualizarStatus_DeveLancarMensagemClara_QuandoEstadoDivergeDoEsperado()
        {
            var ordem = new OrdemDeServico(_clienteId, _veiculoId);

            var ex = Assert.Throws<InvalidOperationException>(() => ordem.IniciarExcucao());
          
            Assert.Contains(nameof(OrdemStatus.AguardandoAprovacao), ex.Message);
            Assert.Contains(nameof(OrdemStatus.EmExecucao), ex.Message);
        }
    }
}
