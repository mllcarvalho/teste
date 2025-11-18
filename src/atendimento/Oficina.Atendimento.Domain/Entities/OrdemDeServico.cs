using Oficina.Atendimento.Domain.Enum;
using Oficina.Atendimento.Domain.ValueObjects;
using Oficina.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Oficina.Atendimento.Domain.Entities
{
    public class OrdemDeServico : Base
    {
        private OrdemDeServico() { }

        public OrdemDeServico(Guid clienteId, Guid veiculoId)
        {
            ClienteId = clienteId;
            VeiculoId = veiculoId;
            Status = OrdemStatus.Recebida;
            DataCriacao = DateTime.UtcNow;
        }

        public Guid ClienteId { get; private set; }

        public Guid VeiculoId { get; private set; }

        [Required]
        public decimal CustoTotal { get; private set; }

        [Required]
        public OrdemStatus Status { get; private set; }

        public DateTime? DataConclusao { get; private set; }

        public DateTime? DataInicioExecucao { get; private set; }
        public DateTime? DataFinalizacao { get; private set; }

        // VOs
        private readonly List<OrdemServicoPeca> _pecas = new();
        public IReadOnlyCollection<OrdemServicoPeca> Pecas => _pecas.AsReadOnly();

        private readonly List<OrdemServicoItemServico> _servicos = new();
        public IReadOnlyCollection<OrdemServicoItemServico> Servicos => _servicos.AsReadOnly();

        // Navegacao - apenas leitura
        public virtual Cliente Cliente { get; private set; }
        public virtual Veiculo Veiculo { get; private set; }

        public virtual Orcamento Orcamento { get; private set; }

        public void AdicionarPeca(OrdemServicoPeca peca)
        {
            _pecas.Add(peca);
        }

        public void AdicionarServico(OrdemServicoItemServico servico)
        {
            _servicos.Add(servico);
        }

        public void DefinirValorTotal(decimal valorServicos, decimal valorPecas)
        {
            CustoTotal = valorServicos + valorPecas;
        }

        public TimeSpan? TempoExecucao
        {
            get
            {
                if (DataInicioExecucao.HasValue && DataFinalizacao.HasValue)
                    return DataFinalizacao.Value - DataInicioExecucao.Value;
                return null;
            }
        }

        public void IniciarDiagnostico() => AtualizarStatus(OrdemStatus.EmDiagnostico, OrdemStatus.Recebida);
        public void ConcluirDiagnostico() => AtualizarStatus(OrdemStatus.AguardandoAprovacao, OrdemStatus.EmDiagnostico);
        public void IniciarExcucao() => AtualizarStatus(OrdemStatus.EmExecucao, OrdemStatus.AguardandoAprovacao);
        public void FinalizarExecucao() => AtualizarStatus(OrdemStatus.Finalizada, OrdemStatus.EmExecucao);
        public void Entregar() => AtualizarStatus(OrdemStatus.Entregue, OrdemStatus.Finalizada);

        private void AtualizarStatus(OrdemStatus novoStatus, OrdemStatus esperado)
        {
            if (Status != esperado)
                throw new InvalidOperationException($"A OS deve estar em {esperado} para mudar para {novoStatus}.");

            switch (novoStatus)
            {
                case OrdemStatus.EmExecucao:
                    DataInicioExecucao = DateTime.UtcNow;
                    break;
                case OrdemStatus.Finalizada:
                    DataFinalizacao = DateTime.UtcNow;
                    break;
                case OrdemStatus.Entregue:
                    DataConclusao = DateTime.UtcNow;
                    break;
                default:
                    break;
            }

            Status = novoStatus;
        }
    }
}
