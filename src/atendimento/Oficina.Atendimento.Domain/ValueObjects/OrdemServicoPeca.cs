using System.ComponentModel.DataAnnotations;

namespace Oficina.Atendimento.Domain.ValueObjects
{
    public class OrdemServicoPeca
    {
        [Required]
        public Guid PecaId { get; }
        [Required]
        [MaxLength(200)]
        public string NomePeca { get; }
        [Required]
        public int Quantidade { get; }
        [Required]
        public decimal Preco { get; }

        private OrdemServicoPeca() {}
        public OrdemServicoPeca(Guid pecaId, string nomePeca, int quantidade, decimal precoUnitario)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));

            PecaId = pecaId;
            NomePeca = nomePeca;
            Quantidade = quantidade;
            Preco = precoUnitario;
        }
        public decimal Subtotal => Quantidade * Preco;
    }
}
