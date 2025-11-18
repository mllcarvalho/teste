using Oficina.Common.Domain.Entities;
using Oficina.Estoque.Domain.Enum;

namespace Oficina.Estoque.Domain.Entities
{
    public class Peca : Base
    {
        public Peca(string nome, decimal preco, TipoPeca tipo, string codigo, string descricao, string fabricante, int quantidadeMinima)
        {
            Nome = nome;
            Preco = preco;
            Tipo = tipo;
            Codigo = codigo;
            Descricao = descricao;
            Fabricante = fabricante;
            QuantidadeMinima = quantidadeMinima;
            DataCriacao = DateTime.UtcNow;
        }

        private Peca() { }

        public string Nome { get; private set; }

        public TipoPeca Tipo { get; set; }
        public string Codigo { get; private set; }
        public string Descricao { get; private set; }

        // Dados técnicos
        public string Fabricante { get; private set; }

        // Precificação
        public decimal Preco { get; private set; }

        // Estoque
        public int Quantidade { get; private set; }
        public int QuantidadeMinima { get; private set; }  // ponto de reposição

        public void AdicionarQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade inválida para reposição.");

            Quantidade += quantidade;
        }

        public void RemoverQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade inválida para baixa.");
            if (quantidade > Quantidade)
                throw new ArgumentException("Estoque insuficiente.");

            Quantidade -= quantidade;
        }

        public void Atualizar(string nome, decimal preco, TipoPeca tipo, string codigo, string descricao, string fabricante, int quantidadeMinima)
        {
            Nome = nome;
            Preco = preco;
            Tipo = tipo;
            Codigo = codigo;
            Descricao = descricao;
            Fabricante = fabricante;
            QuantidadeMinima = quantidadeMinima;
        }
    }
}
