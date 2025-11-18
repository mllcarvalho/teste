using Oficina.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Oficina.Atendimento.Domain.Entities
{
    public class Servico : Base
    {
        private Servico() { }
        public Servico(string nome, decimal preco)
        {
            Nome = nome;
            Preco = preco;
            DataCriacao = DateTime.UtcNow;
        }

        [Required]
        public string Nome { get; set; }
        [Required]
        public decimal Preco { get; private set; }

        public void Atualizar(string nome, decimal preco)
        {
            Nome = nome;
            Preco = preco;
        }
    }
}
