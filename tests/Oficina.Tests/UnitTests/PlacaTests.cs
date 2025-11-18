using Oficina.Atendimento.Domain.ValueObjects;

namespace Oficina.Tests.UnitTests
{
    public class PlacaTests
    {
        [Fact]
        public void Placa_Valida_No_Formato_Antigo_DeveSerCriada()
        {
            // Formato antigo válido
            var placa = new Placa("ABC-1234");
            Assert.Equal("ABC-1234", placa.Numero);
        }

        [Fact]
        public void Placa_Valida_No_Formato_Mercosul_DeveSerCriada()
        {
            // Formato Mercosul válido
            var placaMercosul = new Placa("ABC1D23");
            Assert.Equal("ABC1D23", placaMercosul.Numero);
        }

        [Fact]
        public void Placa_Invalida_DeveLancarExcecao()
        {
            Assert.Throws<ArgumentException>(() => new Placa("1234-ABC")); // formato inválido
            Assert.Throws<ArgumentException>(() => new Placa("ABCD123"));  // formato inválido
            Assert.Throws<ArgumentException>(() => new Placa(""));         // vazio
            Assert.Throws<ArgumentException>(() => new Placa("A1B2C3D"));  // formato inválido
        }
    }
}
