using Oficina.Atendimento.Domain.Entities;
using System.Text;

namespace Oficina.Atendimento.Application.Services
{
    public static class EmailTemplateService
    {
        public static string GerarTemplateOrcamento(Cliente cliente, OrdemDeServico ordem, Orcamento orcamento)
        {
            var html = new StringBuilder();

            // Implementacao da geracao do template de email

            return html.ToString();
        }
    }
}
