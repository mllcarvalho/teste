namespace Oficina.Atendimento.Application.Dto
{
    public class ListarOrdemDeServicoDto
    {
        public Guid OrdemId { get; set; }
        public string? ClienteName {get; set; }
        public string? ClienteDocumento { get; set; }
        public string? DataCriacao { get; set; }
        public string? Status { get; set; }
        public decimal CustoTotal { get; set; }
        public string? DataConclusao { get; set; }
        public string? DataInicioExecucao { get; set; }
        public string? DataFinalizacao { get; set; }
        public string? TempoExecucao { get; set; }
        public OrcamentoDto? Orcamento { get; set; }
        public IList<PecaSolicitadaDto>? Pecas { get; set; }
        public IList<ServicoDto>? Servicos { get; set; }
    }
}
