using System.Net;
using System.Net.Http.Json;
using Oficina.Atendimento.Application.Dto;
using Oficina.Estoque.Application.Dto;
using Oficina.Tests.Integration.Factory;

namespace Oficina.Tests.Integration.ApiIntegrationTests
{
    public class OrdemDeServicoIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public OrdemDeServicoIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        #region Métodos Auxiliares

        private string GerarCpfUnico()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            // Gera os 9 primeiros dígitos aleatórios
            var cpfBase = new int[9];
            for (int i = 0; i < 9; i++)
            {
                cpfBase[i] = random.Next(0, 10);
            }

            // Garante que não são todos iguais
            if (cpfBase.All(d => d == cpfBase[0]))
            {
                cpfBase[1] = (cpfBase[0] + 1) % 10;
            }

            // Calcula o primeiro dígito verificador
            int[] mult1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += cpfBase[i] * mult1[i];
            }
            int resto = sum % 11;
            int dig1 = resto < 2 ? 0 : 11 - resto;

            // Calcula o segundo dígito verificador
            int[] mult2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += cpfBase[i] * mult2[i];
            }
            sum += dig1 * mult2[9];
            resto = sum % 11;
            int dig2 = resto < 2 ? 0 : 11 - resto;

            // Monta o CPF completo
            return $"{string.Join("", cpfBase)}{dig1}{dig2}";
        }

        private string GerarPlacaUnica()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            // Gera 3 letras maiúsculas aleatórias (A-Z)
            var letras1 = new char[3];
            for (int i = 0; i < 3; i++)
            {
                letras1[i] = (char)random.Next('A', 'Z' + 1);
            }

            // Gera 1 número aleatório (0-9)
            var numero1 = random.Next(0, 10);

            // Gera 1 letra maiúscula aleatória (A-Z)
            var letra2 = (char)random.Next('A', 'Z' + 1);

            // Gera 2 números aleatórios (0-9)
            var numero2 = random.Next(0, 10);
            var numero3 = random.Next(0, 10);

            // Formato Mercosul: LLLNLNN (ex: ABC1D23)
            return $"{new string(letras1)}{numero1}{letra2}{numero2}{numero3}";
        }

        private async Task<Guid> CriarClienteAsync(string nome, string email, string? documento = null)
        {
            var clienteDto = new ClienteDto
            {
                Nome = nome,
                Email = email,
                Documento = documento ?? GerarCpfUnico()
            };

            var response = await _client.PostAsJsonAsync("/api/Cliente", clienteDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Falha ao criar cliente: {response.StatusCode} - {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<Guid>();
        }

        private async Task<Guid> CriarVeiculoAsync(Guid clienteId, string marca, string modelo, string? placa = null)
        {
            var veiculoDto = new VeiculoDto
            {
                Marca = marca,
                Modelo = modelo,
                Ano = 2020,
                Placa = placa ?? GerarPlacaUnica(),
                ClienteId = clienteId
            };

            var response = await _client.PostAsJsonAsync("/api/Veiculo", veiculoDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Falha ao criar veículo: {response.StatusCode} - {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<Guid>();
        }

        private async Task<Guid> CriarPecaAsync(string nome, string codigo, decimal preco, int quantidade)
        {
            var pecaDto = new PecaDto
            {
                Nome = nome,
                Tipo = "Peca",
                Codigo = codigo,
                Descricao = $"Descrição da {nome}",
                Fabricante = "Fabricante Teste",
                Preco = preco,
                Quantidade = quantidade,
                QuantidadeMinima = 5
            };

            var response = await _client.PostAsJsonAsync("/api/Peca", pecaDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Falha ao criar peça: {response.StatusCode} - {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<Guid>();
        }

        private async Task<Guid> CriarServicoAsync(string nome, decimal preco)
        {
            var servicoDto = new ServicoDto
            {
                Nome = nome,
                Preco = preco
            };

            var response = await _client.PostAsJsonAsync("/api/Servico", servicoDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Falha ao criar serviço: {response.StatusCode} - {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<Guid>();
        }

        private async Task<Guid> CriarOrdemDeServicoAsync(string clienteDoc, string veiculoPlaca)
        {
            var response = await _client.PostAsync($"/api/OrdemDeServico?clienteDoc={clienteDoc}&veiculoPlaca={veiculoPlaca}", content: null);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Falha ao criar ordem de serviço: {response.StatusCode} - {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<Guid>();
        }

        #endregion

        [Fact]
        public async Task ObterOrdemPorId_ComIdInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var ordemIdInexistente = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/OrdemDeServico/{ordemIdInexistente}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task IniciarDiagnostico_ComOrdemInexistente_DeveFalhar()
        {
            // Arrange
            var ordemIdInexistente = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/OrdemDeServico/iniciar-diagnostico?ordemId={ordemIdInexistente}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task FluxoCompleto_OrdemDeServico_DevePassarPorTodosStatus()
        {
            // Arrange
            var cpf = GerarCpfUnico();
            var email = $"fernando.{Guid.NewGuid():N}@teste.com";
            var placa = GerarPlacaUnica();

            var clienteId = await CriarClienteAsync("Fernando Alves", email, cpf);
            await CriarVeiculoAsync(clienteId, "Toyota", "Corolla", placa);

            var pecaId = await CriarPecaAsync($"Bateria Fluxo {Guid.NewGuid():N}", $"BAT-FLX-{Guid.NewGuid():N}", 350.00m, 15);
            var servicoId = await CriarServicoAsync($"Instalação Bateria {Guid.NewGuid():N}", 80.00m);

            var ordemId = await CriarOrdemDeServicoAsync(cpf, placa);

            // 1. Iniciar Diagnóstico
            var diagnosticoResponse = await _client.PostAsync($"/api/OrdemDeServico/iniciar-diagnostico?ordemId={ordemId}", content: null);
            diagnosticoResponse.EnsureSuccessStatusCode();

            // 2. Concluir Diagnóstico
            var concluirDto = new OrdemDeServicoDto
            {
                OrdemId = ordemId,
                ServicosIds = new List<Guid> { servicoId },
                Pecas = new List<PecaSolicitadaDto>
                {
                    new() { PecaId = pecaId, Quantidade = 1 }
                }
            };
            var concluirResp = await _client.PostAsJsonAsync("/api/OrdemDeServico/concluir-diagnostico", concluirDto);
            concluirResp.EnsureSuccessStatusCode();

            // 3. Aprovar Orçamento
            var aprovarResponse = await _client.PostAsync($"/api/OrdemDeServico/aprovar-orcamento?ordemId={ordemId}", content: null);
            aprovarResponse.EnsureSuccessStatusCode();

            // 4. Finalizar Execução
            var finalizarResponse = await _client.PostAsync($"/api/OrdemDeServico/finalizar-execucao?ordemId={ordemId}", content: null);
            finalizarResponse.EnsureSuccessStatusCode();

            // 5. Entregar
            var entregarResponse = await _client.PostAsync($"/api/OrdemDeServico/entregar?ordemId={ordemId}", content: null);
            entregarResponse.EnsureSuccessStatusCode();

            // Assert final
            var ordemDto = await _client.GetFromJsonAsync<ListarOrdemDeServicoDto>($"/api/OrdemDeServico/{ordemId}");
            Assert.NotNull(ordemDto);
            Assert.Equal("Entregue", ordemDto!.Status);
            Assert.NotNull(ordemDto.Orcamento);
            Assert.Equal("Aprovado", ordemDto.Orcamento!.Status);
            Assert.False(string.IsNullOrWhiteSpace(ordemDto.DataFinalizacao));
             Assert.False(string.IsNullOrWhiteSpace(ordemDto.DataConclusao));
            Assert.False(string.IsNullOrWhiteSpace(ordemDto.TempoExecucao));
        }

        [Fact]
        public async Task ObterTempoMedioExecucao_DeveRetornarMedia()
        {
            var cpf1 = GerarCpfUnico();
            var cliente1 = await CriarClienteAsync("Cliente TM1", "tm1@x.com", cpf1);
            var placa1 = GerarPlacaUnica();
            await CriarVeiculoAsync(cliente1, "MarcaTM", "ModeloTM1", placa1);
            var serv1 = await CriarServicoAsync("Servico TM1", 50m);
            var peca1 = await CriarPecaAsync("Peca TM1", "PTM1", 10m, 100);
            var os1 = await CriarOrdemDeServicoAsync(cpf1, placa1);

            var cpf2 = GerarCpfUnico();
            var cliente2 = await CriarClienteAsync("Cliente TM2", "tm2@x.com", cpf2);
            var placa2 = GerarPlacaUnica();
            await CriarVeiculoAsync(cliente2, "MarcaTM", "ModeloTM2", placa2);
            var serv2 = await CriarServicoAsync("Servico TM2", 60m);
            var peca2 = await CriarPecaAsync("Peca TM2", "PTM2", 20m, 100);
            var os2 = await CriarOrdemDeServicoAsync(cpf2, placa2);

            await _client.PostAsync($"/api/OrdemDeServico/iniciar-diagnostico?ordemId={os1}", content: null);
            await _client.PostAsJsonAsync("/api/OrdemDeServico/concluir-diagnostico", new OrdemDeServicoDto
            {
                OrdemId = os1,
                ServicosIds = new List<Guid> { serv1 },
                Pecas = new List<PecaSolicitadaDto> { new PecaSolicitadaDto { PecaId = peca1, Quantidade = 1 } }
            });
            await _client.PostAsync($"/api/OrdemDeServico/aprovar-orcamento?ordemId={os1}", content: null);
            await Task.Delay(50); 
            await _client.PostAsync($"/api/OrdemDeServico/finalizar-execucao?ordemId={os1}", content: null);

            await _client.PostAsync($"/api/OrdemDeServico/iniciar-diagnostico?ordemId={os2}", content: null);
            await _client.PostAsJsonAsync("/api/OrdemDeServico/concluir-diagnostico", new OrdemDeServicoDto
            {
                OrdemId = os2,
                ServicosIds = new List<Guid> { serv2 },
                Pecas = new List<PecaSolicitadaDto> { new PecaSolicitadaDto { PecaId = peca2, Quantidade = 2 } }
            });
            await _client.PostAsync($"/api/OrdemDeServico/aprovar-orcamento?ordemId={os2}", content: null);
            await Task.Delay(80);
            await _client.PostAsync($"/api/OrdemDeServico/finalizar-execucao?ordemId={os2}", content: null);

            var response = await _client.GetAsync("/api/OrdemDeServico/tempo-medio-execucao");

            Assert.True(response.IsSuccessStatusCode);
            var resultado = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(resultado);
        }

        [Fact]
        public async Task Status_DeveRetornarStatusDaOrdem_SemAutenticacao()
        {
            var cpf = GerarCpfUnico();
            var clienteId = await CriarClienteAsync("Cliente Status", "status@x.com", cpf);
            var placa = GerarPlacaUnica();
            await CriarVeiculoAsync(clienteId, "MarcaS", "ModeloS", placa);
            var ordemId = await CriarOrdemDeServicoAsync(cpf, placa);

            var ordem = await _client.GetFromJsonAsync<ListarOrdemDeServicoDto>($"/api/OrdemDeServico/{ordemId}");

            Assert.NotNull(ordem);
            Assert.Equal("Recebida", ordem!.Status);
        }
    }
}