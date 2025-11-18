using Oficina.Atendimento.Domain.Entities;

namespace Oficina.Atendimento.Domain.Services
{
    public static class OrdemDeServicoDomainService
    {
        public static decimal CalcularValorServico(OrdemDeServico ordem)
        {
            ArgumentNullException.ThrowIfNull(ordem);

            return ordem.Servicos.Sum(p => p.Preco);
        }

        public static decimal CalcularValorPecas(OrdemDeServico ordem)
        {
            ArgumentNullException.ThrowIfNull(ordem);

            return ordem.Pecas.Sum(p => p.Preco * p.Quantidade);
        }
    }
}
