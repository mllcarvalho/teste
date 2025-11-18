namespace Oficina.Common.Domain.Entities
{
    public class EmailMessage
    {
        public string Para { get; set; } = string.Empty;
        public string ParaNome { get; set; } = string.Empty;
        public string Assunto { get; set; } = string.Empty;
        public string CorpoHtml { get; set; } = string.Empty;
        public string? CorpoTexto { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
