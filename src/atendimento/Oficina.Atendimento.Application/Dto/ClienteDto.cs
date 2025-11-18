using System.ComponentModel.DataAnnotations;

namespace Oficina.Atendimento.Application.Dto
{
    public class ClienteDto
    {
        public Guid ClienteId { get; set; }
        public string? Nome { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Documento { get; set; }
    }
}
