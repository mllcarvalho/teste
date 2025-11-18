using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.Services;
using Oficina.Atendimento.Domain.ValueObjects;

namespace Oficina.Tests.Atendimento.Domain.Services
{
    public class OrdemDeServicoDomainServiceTests
    {
        [Fact]
        public void CalcularValorServico_RetornaSoma_QuandoServicoPresente()
        {
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem.AdicionarServico(new OrdemServicoItemServico(Guid.NewGuid(), "S1", 100m));
            ordem.AdicionarServico(new OrdemServicoItemServico(Guid.NewGuid(), "S2", 150.5m));

            var result = OrdemDeServicoDomainService.CalcularValorServico(ordem);

            Assert.Equal(250.5m, result);
        }

        [Fact]
        public void CalcularValorServico_RetornaZero_QuandoSemServico()
        {
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            var result = OrdemDeServicoDomainService.CalcularValorServico(ordem);

            Assert.Equal(0m, result);
        }

        [Fact]
        public void CalcularValorServico_DeveLancarException_QuandoOrdemNula()
        {
            OrdemDeServico ordem = null!;
            Assert.Throws<ArgumentNullException>(() => OrdemDeServicoDomainService.CalcularValorServico(ordem));
        }

        [Fact]
        public void CalcularValorPecas_DeveRetornarValorTotal_QuandoPecasPresente()
        {
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem.AdicionarPeca(new OrdemServicoPeca(Guid.NewGuid(), "P1", 2, 30m)); // 60
            ordem.AdicionarPeca(new OrdemServicoPeca(Guid.NewGuid(), "P2", 3, 12.5m)); // 37.5

            var result = OrdemDeServicoDomainService.CalcularValorPecas(ordem);

            Assert.Equal(97.5m, result);
        }

        [Fact]
        public void CalcularValorPecas_DeveRetornarZero_QuandoSemPecas()
        {
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            var result = OrdemDeServicoDomainService.CalcularValorPecas(ordem);

            Assert.Equal(0m, result);
        }

        [Fact]
        public void CalcularValorPecas_DeveLancarException_QuandoOrdemNula()
        {
            OrdemDeServico ordem = null!;
            Assert.Throws<ArgumentNullException>(() => OrdemDeServicoDomainService.CalcularValorPecas(ordem));
        }
    }
}