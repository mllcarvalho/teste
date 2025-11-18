using Oficina.Atendimento.Domain.ValueObjects;
using System;
using Xunit;

namespace Oficina.Tests.Atendimento.Domain.ValueObjects
{
    public class OrdemServicoPecaTests
    {
        [Fact]
        public void Construtor_DeveCriarPeca()
        {
            // Arrange
            var pecaId = Guid.NewGuid();
            var nome = "Filtro de Óleo";
            var quantidade = 2;
            var preco = 50m;

            // Act
            var peca = new OrdemServicoPeca(pecaId, nome, quantidade, preco);

            // Assert
            Assert.Equal(pecaId, peca.PecaId);
            Assert.Equal(nome, peca.NomePeca);
            Assert.Equal(quantidade, peca.Quantidade);
            Assert.Equal(preco, peca.Preco);
            Assert.Equal(preco * quantidade, peca.Subtotal);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Construtor_Deve_Lancar_Exception_Quando_Quantidade_Menor_Ou_Igual_Zero(int quantidade)
        {
            var pecaId = Guid.NewGuid();
            var ex = Assert.Throws<ArgumentException>(() => new OrdemServicoPeca(pecaId, "Nome Peca", quantidade, 10m));

            Assert.Contains("Quantidade deve ser maior que zero.", ex.Message);
            Assert.Equal("quantidade", ex.ParamName);
        }

        [Fact]
        public void ValorTotal_DeveCalcularCorretamente_QuandoAcessado()
        {
            // Arrange
            var pecaId = Guid.NewGuid();
            var nome = "Filtro de Óleo";
            var quantidade = 3;
            var preco = 40m;
            var peca = new OrdemServicoPeca(pecaId, nome, quantidade, preco);

            // Act
            var valorTotal = peca.Subtotal;

            // Assert
            Assert.Equal(120m, valorTotal); // 3 * 40
        }
    }
}