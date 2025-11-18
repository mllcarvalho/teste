using Oficina.Atendimento.Domain.ValueObjects;

namespace Oficina.Tests.UnitTests
{
    public  class CpfCnpjTests
    {
        [Fact]
        public void Deve_Criar_Cpf_Valido()
        {
            var cpf = new CpfCnpj("529.982.247-25");
            Assert.Equal("52998224725", cpf.Numero);
        }

        [Fact]
        public void Deve_Criar_Cnpj_Valido()
        {
            var cnpj = new CpfCnpj("04.252.011/0001-10");
            Assert.Equal("04252011000110", cnpj.Numero);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Para_Cpf_Invalido()
        {
            Assert.Throws<ArgumentException>(() => new CpfCnpj("123.456.789-00"));
        }

        [Fact]
        public void Deve_Lancar_Excecao_Para_Cnpj_Invalido()
        {
            Assert.Throws<ArgumentException>(() => new CpfCnpj("11.111.111/1111-11"));
        }

        [Fact]
        public void Deve_Lancar_Excecao_Para_Comprimento_Invalido()
        {
            Assert.Throws<ArgumentException>(() => new CpfCnpj("123456789")); // Menor que 11
            Assert.Throws<ArgumentException>(() => new CpfCnpj("123456789012")); // Menor que 14 para CNPJ
            Assert.Throws<ArgumentException>(() => new CpfCnpj("123456789012345")); // Maior que 14
        }

        [Fact]
        public void Deve_Lancar_Excecao_Para_Numero_Em_Branco()
        {
            Assert.Throws<ArgumentException>(() => new CpfCnpj(""));
            Assert.Throws<ArgumentException>(() => new CpfCnpj("   "));
            Assert.Throws<ArgumentException>(() => new CpfCnpj(null!));
        }

        [Fact]
        public void Deve_Lancar_Excecao_Para_Cpf_Com_Digitos_Iguais()
        {
            Assert.Throws<ArgumentException>(() => new CpfCnpj("111.111.111-11"));
        }

        [Fact]
        public void Deve_Lancar_Excecao_Para_Cnpj_Com_Digitos_Iguais()
        {
            Assert.Throws<ArgumentException>(() => new CpfCnpj("000.000.000/0000-00"));
        }
    }
}
