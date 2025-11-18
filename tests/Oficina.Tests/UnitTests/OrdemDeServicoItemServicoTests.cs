using Oficina.Atendimento.Domain.ValueObjects;
using System;
using Xunit;

namespace Oficina.Tests.Atendimento.Domain.ValueObjects
{
    public class OrdemServicoItemServicoTests
    {
        [Fact]
        public void Construtor_DeveCriarItemServico()
        {
            // Arrange
            var servicoId = Guid.NewGuid();
            var nome = "Troca de Óleo";
            var preco = 100m;

            // Act
            var itemServico = new OrdemServicoItemServico(servicoId, nome, preco);

            // Assert
            Assert.Equal(servicoId, itemServico.ServicoId);
            Assert.Equal(nome, itemServico.NomeServico);
            Assert.Equal(preco, itemServico.Preco);
        }
    }
}