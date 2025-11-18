namespace Oficina.Atendimento.Application.Dto
{
    public class OrdemDeServicoDto
    {
        public Guid OrdemId { get; set; }
        public List<PecaSolicitadaDto> Pecas { get; set; } = new();
        public List<Guid> ServicosIds { get; set; } = new();
    }
}
