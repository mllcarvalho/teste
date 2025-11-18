using Oficina.Atendimento.Domain.ValueObjects;
using Oficina.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Oficina.Atendimento.Domain.Entities
{
    public class Cliente : Base
    {
        private Cliente() { }

        public Cliente(CpfCnpj doc, string name, string email)
        {
            Documento = doc;
            Nome = name;
            Email = email;
            DataCriacao = DateTime.UtcNow;
        }

        public CpfCnpj Documento { get; private set; }
        
        [Required]
        [MaxLength(150)]
        public string Nome { get; private set; }
        
        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; private set; }
        
        public virtual ICollection<Veiculo> Veiculos { get; private set; } = [];

        public void Atualizar(string name, string email, CpfCnpj doc)
        {
            Nome = name;
            Email = email;
            Documento = doc;
        }
    }
}
