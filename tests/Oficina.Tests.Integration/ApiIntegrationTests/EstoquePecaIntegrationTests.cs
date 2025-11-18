using System.Net;
using System.Net.Http.Json;
using Oficina.Estoque.Application.Dto;
using Oficina.Tests.Integration.Factory;

namespace Oficina.Tests.Integration.ApiIntegrationTests
{
    public class EstoquePecaIntegrationTests: IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public EstoquePecaIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Cadastrar_Peca_Com_Estoque_Inicial()
        {
            var pecaDto = new PecaDto
            {
                Nome = "Filtro de Óleo Original",
                Tipo = "Peca",
                Codigo = "FO-123456",
                Descricao = "Filtro de óleo para motores 1.0 a 1.6",
                Fabricante = "AutoParts",
                Preco = 35.90m,
                Quantidade = 20,
                QuantidadeMinima = 5
            };

            var postPecaResponse = await _client.PostAsJsonAsync("/api/Peca", pecaDto);
            postPecaResponse.EnsureSuccessStatusCode();
            var pecaId = await postPecaResponse.Content.ReadFromJsonAsync<Guid>();

            Assert.NotEqual(Guid.Empty, pecaId);

            var getEstoqueResponse = await _client.GetAsync($"/api/Estoque/{pecaId}");
            getEstoqueResponse.EnsureSuccessStatusCode();
            var quantidadeEstoque = await getEstoqueResponse.Content.ReadFromJsonAsync<int>();

            Assert.Equal(20, quantidadeEstoque);
        }

        [Fact]
        public async Task Adicionar_Quantidade_Em_Estoque_Existente()
        {
            var pecaDto = new PecaDto
            {
                Nome = "Pastilha de Freio Premium",
                Tipo = "Peca",
                Codigo = "PF-789012",
                Descricao = "Pastilhas de freio de alta durabilidade",
                Fabricante = "BrakeMaster",
                Preco = 89.90m,
                Quantidade = 10,
                QuantidadeMinima = 4
            };

            var postPecaResponse = await _client.PostAsJsonAsync("/api/Peca", pecaDto);
            postPecaResponse.EnsureSuccessStatusCode();
            
            var pecaId = await postPecaResponse.Content.ReadFromJsonAsync<Guid>();
            var quantidadeAdicional = 15;
            var adicionarEstoqueResponse = await _client.PostAsJsonAsync(
                $"/api/Estoque/adicionar?pecaId={pecaId}&quantidade={quantidadeAdicional}",
                new { });
            adicionarEstoqueResponse.EnsureSuccessStatusCode();

            var getEstoqueResponse = await _client.GetAsync($"/api/Estoque/{pecaId}");
            getEstoqueResponse.EnsureSuccessStatusCode();
            var quantidadeEstoque = await getEstoqueResponse.Content.ReadFromJsonAsync<int>();

            Assert.Equal(25, quantidadeEstoque);
        }

        [Fact]
        public async Task Remover_Quantidade_Do_Estoque()
        {
            var pecaDto = new PecaDto
            {
                Nome = "Óleo Sintético 5W30",
                Tipo = "Peca",
                Codigo = "OS-345678",
                Descricao = "Óleo sintético para motores modernos",
                Fabricante = "LubriTech",
                Preco = 45.50m,
                Quantidade = 30,
                QuantidadeMinima = 8
            };

            var postPecaResponse = await _client.PostAsJsonAsync("/api/Peca", pecaDto);
            postPecaResponse.EnsureSuccessStatusCode();
            
            var pecaId = await postPecaResponse.Content.ReadFromJsonAsync<Guid>();
            var quantidadeRemovida = 12;
            var removerEstoqueResponse = await _client.PostAsJsonAsync(
                $"/api/Estoque/remover?pecaId={pecaId}&quantidade={quantidadeRemovida}",
                new { });
            removerEstoqueResponse.EnsureSuccessStatusCode();

            var getEstoqueResponse = await _client.GetAsync($"/api/Estoque/{pecaId}");
            getEstoqueResponse.EnsureSuccessStatusCode();
            var quantidadeEstoque = await getEstoqueResponse.Content.ReadFromJsonAsync<int>();

            Assert.Equal(18, quantidadeEstoque);
        }

        [Fact]
        public async Task Remover_Quantidade_Maior_Que_Estoque_Deve_Falhar()
        {
            var pecaDto = new PecaDto
            {
                Nome = "Vela de Ignição Especial",
                Tipo = "Peca",
                Codigo = "VI-567890",
                Descricao = "Vela de ignição para motores de alto desempenho",
                Fabricante = "SparkTech",
                Preco = 28.75m,
                Quantidade = 5,
                QuantidadeMinima = 2
            };

            var postPecaResponse = await _client.PostAsJsonAsync("/api/Peca", pecaDto);
            postPecaResponse.EnsureSuccessStatusCode();
            var pecaId = await postPecaResponse.Content.ReadFromJsonAsync<Guid>();
            var quantidadeRemovida = 10;
            var removerEstoqueResponse = await _client.PostAsJsonAsync(
                $"/api/Estoque/remover?pecaId={pecaId}&quantidade={quantidadeRemovida}",
                new { });

            Assert.Equal(HttpStatusCode.BadRequest, removerEstoqueResponse.StatusCode);

            var getEstoqueResponse = await _client.GetAsync($"/api/Estoque/{pecaId}");
            getEstoqueResponse.EnsureSuccessStatusCode();
            var quantidadeEstoque = await getEstoqueResponse.Content.ReadFromJsonAsync<int>();

            Assert.Equal(5, quantidadeEstoque);
        }

        [Fact]
        public async Task Atualizar_Peca_Nao_Deve_Afetar_Estoque()
        {
            var pecaDto = new PecaDto
            {
                Nome = "Correia Dentada Standard",
                Tipo = "Peca",
                Codigo = "CD-901234",
                Descricao = "Correia dentada para motores comuns",
                Fabricante = "BeltCo",
                Preco = 65.30m,
                Quantidade = 15,
                QuantidadeMinima = 3
            };

            var postPecaResponse = await _client.PostAsJsonAsync("/api/Peca", pecaDto);
            postPecaResponse.EnsureSuccessStatusCode();
            var pecaId = await postPecaResponse.Content.ReadFromJsonAsync<Guid>();

            var getEstoqueInicialResponse = await _client.GetAsync($"/api/Estoque/{pecaId}");
            getEstoqueInicialResponse.EnsureSuccessStatusCode();
            var quantidadeEstoqueInicial = await getEstoqueInicialResponse.Content.ReadFromJsonAsync<int>();

            var getPecaResponse = await _client.GetAsync($"/api/Peca/{pecaId}");
            getPecaResponse.EnsureSuccessStatusCode();
            var pecaParaAtualizar = await getPecaResponse.Content.ReadFromJsonAsync<PecaDto>();

            pecaParaAtualizar!.Nome = "Correia Dentada Premium";
            pecaParaAtualizar.Preco = 75.90m;
            pecaParaAtualizar.Descricao = "Correia dentada de alta durabilidade";
         
            var putPecaResponse = await _client.PutAsJsonAsync("/api/Peca", pecaParaAtualizar);
            putPecaResponse.EnsureSuccessStatusCode();

            var getEstoqueFinalResponse = await _client.GetAsync($"/api/Estoque/{pecaId}");
            getEstoqueFinalResponse.EnsureSuccessStatusCode();
            var quantidadeEstoqueFinal = await getEstoqueFinalResponse.Content.ReadFromJsonAsync<int>();

            Assert.Equal(quantidadeEstoqueInicial, quantidadeEstoqueFinal);

            var getPecaAtualizadaResponse = await _client.GetAsync($"/api/Peca/{pecaId}");
            getPecaAtualizadaResponse.EnsureSuccessStatusCode();
            var pecaAtualizada = await getPecaAtualizadaResponse.Content.ReadFromJsonAsync<PecaDto>();

            Assert.Equal("Correia Dentada Premium", pecaAtualizada!.Nome);
            Assert.Equal(75.90m, pecaAtualizada.Preco);
        }

        [Fact]
        public async Task Consultar_Estoque_De_Peca_Inexistente_Deve_Falhar()
        {
            var pecaIdInexistente = Guid.NewGuid();
            var getEstoqueResponse = await _client.GetAsync($"/api/Estoque/{pecaIdInexistente}");

            Assert.Equal(HttpStatusCode.BadRequest, getEstoqueResponse.StatusCode);
        }
    }
}
