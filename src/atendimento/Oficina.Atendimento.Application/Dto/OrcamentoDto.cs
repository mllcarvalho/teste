namespace Oficina.Atendimento.Application.Dto
{
    public class OrcamentoDto
    {
        public decimal ValorTotal { get; set; }
        public decimal ValorServicos { get; set; }
        public decimal ValorPecas { get; set; }
        public string? DataAprovacao { get; set; }
        public string Status { get; set; }
    }
}
