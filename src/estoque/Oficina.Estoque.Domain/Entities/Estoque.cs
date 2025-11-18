using Oficina.Common.Domain.Entities;
using Oficina.Estoque.Domain.Enum;

namespace Oficina.Estoque.Domain.Entities
{
	public class Estoque: Base
    {
        private Estoque() {}

        public Estoque(Guid pecaId, TipoMovimentoEstoque tipo, int quantidade)
        {
            PecaId = pecaId;
            TipoMovimento = tipo;
            Quantidade = quantidade;
            DataMovimento = DateTime.UtcNow;
            DataCriacao = DateTime.UtcNow;
        }

        public Guid PecaId { get; set; }
        public TipoMovimentoEstoque TipoMovimento { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataMovimento { get; set; }
    }

}
