using System.ComponentModel.DataAnnotations;

namespace Oficina.Common.Domain.Entities
{
    public class Base
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }

        [Required]
        public DateTime? DataAtualizacao { get; set; }
    }
}
