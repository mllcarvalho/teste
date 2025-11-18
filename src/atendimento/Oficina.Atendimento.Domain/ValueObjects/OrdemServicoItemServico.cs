using System.ComponentModel.DataAnnotations;

namespace Oficina.Atendimento.Domain.ValueObjects
{
    public class OrdemServicoItemServico
    {
        [Required]
        public Guid ServicoId { get; }
        [Required]
        [MaxLength(200)]
        public string NomeServico { get; }
        [Required]
        public decimal Preco { get; }

        private OrdemServicoItemServico() {}

        public OrdemServicoItemServico(Guid servicoId, string nomeServico, decimal preco)
        {
            ServicoId = servicoId;
            NomeServico = nomeServico;
            Preco = preco;
        }
    }
}
