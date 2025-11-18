namespace Oficina.Atendimento.Application.Dto
{
    public class VeiculoDto
    {
        public string Modelo { get; set; }
        public string Placa { get; set; }
        public string Marca { get; set; }
        public int Ano { get; set; }
        public Guid ClienteId { get; set; }
        public Guid VeiculoId { get; set; }
    }
}
