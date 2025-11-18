using Oficina.Atendimento.Domain.ValueObjects;
using Oficina.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Oficina.Atendimento.Domain.Entities
{
    public class Veiculo : Base
    {
        public Veiculo(string modelo, Placa placa, string marca, int ano, Guid clienteId)
        {
            Modelo = modelo;
            Placa = placa;
            Marca = marca;
            Ano = ano;
            ClienteId = clienteId;
            DataCriacao = DateTime.UtcNow;
        }

        private Veiculo(){}

        [Required]
        [MaxLength(50)]
        public string Modelo { get; private set; }

        [Required]
        [MaxLength(7)]
        public Placa Placa { get; private set; }

        [Required]
        [MaxLength(50)]
        public string Marca { get; set; }
        
        [Required]
        [MaxLength(4)]
        public int Ano { get; set; }


        [Required]
        public Guid ClienteId { get; private set; }

        public virtual Cliente Cliente { get; private set; }

        public void Atualizar(string modelo, Placa placa, string marca, int ano)
        {
            Modelo = modelo;
            Placa = placa;
            Marca = marca;
            Ano = ano;
        }
    }
}
