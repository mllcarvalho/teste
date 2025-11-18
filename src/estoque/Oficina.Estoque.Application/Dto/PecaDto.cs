namespace Oficina.Estoque.Application.Dto
{
    public class PecaDto
    {
        public Guid PecaId { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Fabricante { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public int QuantidadeMinima { get; set; }
    }
}
