using Oficina.Atendimento.Domain.Enum;
using Oficina.Common.Domain.Entities;

namespace Oficina.Atendimento.Domain.Entities
{
    public class Orcamento : Base
    {
        private Orcamento() { }

        public Orcamento(Guid ordemDeServicoId, decimal valorServicos, decimal valorPecas)
        {
            OrdemDeServicoId = ordemDeServicoId;
            ValorServicos = valorServicos;
            ValorPecas = valorPecas;
            ValorTotal = valorPecas + valorServicos;
            Status = OrcamentoStatus.Pendente;
            DataCriacao = DateTime.UtcNow;
        }

        public Guid OrdemDeServicoId { get; private set; }
        public decimal ValorServicos { get; private set; }
        public decimal ValorPecas { get; private set; }
        public decimal ValorTotal { get; private set; }
        public DateTime? DataAprovacao { get; set; }
        public OrcamentoStatus Status { get; private set; }
      
        public virtual OrdemDeServico OrdemDeServico { get; private set; }

        public void Aprovar()
        {
            if (Status != OrcamentoStatus.Pendente)
                throw new InvalidOperationException("Orçamento já aprovado.");
            
            Status = OrcamentoStatus.Aprovado;
            DataAprovacao = DateTime.UtcNow;
        }
    }
}
