namespace Oficina.Atendimento.Application.Dto
{
    public class PecaSolicitadaDto
    {
        public Guid PecaId { get; set; }
        public string? Nome { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }
}
