using System.ComponentModel.DataAnnotations;

namespace Oficina.Atendimento.Application.Dto
{
    public class CriarClienteDto
    {
        public string? Nome { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Documento { get; set; }
    }
}
