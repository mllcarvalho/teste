using Azure;
using Oficina.Atendimento.Application.Dto;
using Oficina.Tests.Integration.Factory;
using System.Net;
using System.Net.Http.Json;

namespace Oficina.Tests.Integration.ApiIntegrationTests
{
    [Collection("ClienteVeiculoIntegrationTests Collection")]
    public class ClienteVeiculoIntegrationTests
    {
        //[Fact]
        //public async Task Cadastrar_Cliente_E_Veiculos_Fluxo_Completo()
        //{
        //    var (factory, client, dbPath) = TestHelpers.CreateFactoryWithTempFileDb();
        //    try
        //    {
        //        var clienteDto = new ClienteDto
        //        {
        //            Nome = "Marcos Silva",
        //            Email = "marcos@exemplo.com",
        //            Documento = "42975620063"
        //        };

        //        var postClienteResponse = await client.PostAsJsonAsync("/api/Cliente", clienteDto);
        //        var responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        postClienteResponse.EnsureSuccessStatusCode();
        //        var clienteId = await postClienteResponse.Content.ReadFromJsonAsync<Guid>();

        //        Assert.NotEqual(Guid.Empty, clienteId);

        //        var veiculoDto = new VeiculoDto
        //        {
        //            Marca = "Honda",
        //            Modelo = "Civic",
        //            Ano = 2021,
        //            Placa = "ABC-1234",
        //            ClienteId = clienteId
        //        };

        //        var postVeiculoResponse = await client.PostAsJsonAsync("/api/Veiculo", veiculoDto);
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        postVeiculoResponse.EnsureSuccessStatusCode();
        //        var veiculoId = await postVeiculoResponse.Content.ReadFromJsonAsync<Guid>();

        //        Assert.NotEqual(Guid.Empty, veiculoId);

        //        var veiculoDto2 = new VeiculoDto
        //        {
        //            Marca = "Honda",
        //            Modelo = "Fit",
        //            Ano = 2019,
        //            Placa = "DEF1G23",
        //            ClienteId = clienteId
        //        };

        //        var postVeiculoResponse2 = await client.PostAsJsonAsync("/api/Veiculo", veiculoDto2);
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        postVeiculoResponse2.EnsureSuccessStatusCode();
        //        var veiculoId2 = await postVeiculoResponse2.Content.ReadFromJsonAsync<Guid>();

        //        var getVeiculosResponse = await client.GetAsync($"/api/Veiculo/todos/{clienteId}");
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        getVeiculosResponse.EnsureSuccessStatusCode();
        //        var veiculos = await getVeiculosResponse.Content.ReadFromJsonAsync<IEnumerable<VeiculoDto>>();

        //        Assert.NotNull(veiculos);
        //        Assert.Equal(2, veiculos!.Count());
        //        Assert.Contains(veiculos, v => v.VeiculoId == veiculoId);
        //        Assert.Contains(veiculos, v => v.VeiculoId == veiculoId2);

        //        var getClienteResponse = await client.GetAsync($"/api/Cliente/{clienteId}");
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        getClienteResponse.EnsureSuccessStatusCode();
        //        var clienteParaAtualizar = await getClienteResponse.Content.ReadFromJsonAsync<ClienteDto>();

        //        clienteParaAtualizar!.Nome = "Marcos Silva Junior";
        //        clienteParaAtualizar.Email = "marcos.junior@exemplo.com";

        //        var putClienteResponse = await client.PutAsJsonAsync("/api/Cliente", clienteParaAtualizar);
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        putClienteResponse.EnsureSuccessStatusCode();

        //        var getClienteAtualizadoResponse = await client.GetAsync($"/api/Cliente/{clienteId}");
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        getClienteAtualizadoResponse.EnsureSuccessStatusCode();
        //        var clienteAtualizado = await getClienteAtualizadoResponse.Content.ReadFromJsonAsync<ClienteDto>();

        //        Assert.NotNull(clienteAtualizado);
        //        Assert.Equal("Marcos Silva Junior", clienteAtualizado!.Nome);
        //        Assert.Equal("marcos.junior@exemplo.com", clienteAtualizado.Email);
        //    }
        //    finally
        //    {
        //        factory.Dispose();
        //        if (File.Exists(dbPath)) File.Delete(dbPath);
        //    }
        //}

        //[Fact]
        //public async Task Adicionar_Veiculo_Com_Placa_Invalida_Deve_Falhar()
        //{
        //    var (factory, client, dbPath) = TestHelpers.CreateFactoryWithTempFileDb();
        //    try
        //    {
        //        var clienteDto = new ClienteDto
        //        {
        //            Nome = "João Silva",
        //            Email = "joao@exemplo.com",
        //            Documento = "42975620063"
        //        };

        //        var postClienteResponse = await client.PostAsJsonAsync("/api/Cliente", clienteDto);
        //        var responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        postClienteResponse.EnsureSuccessStatusCode();
        //        var clienteId = await postClienteResponse.Content.ReadFromJsonAsync<Guid>();

        //        var veiculoDto = new VeiculoDto
        //        {
        //            Marca = "Toyota",
        //            Modelo = "Corolla",
        //            Ano = 2022,
        //            Placa = "1234-ABC",
        //            ClienteId = clienteId
        //        };

        //        var postVeiculoResponse = await client.PostAsJsonAsync("/api/Veiculo", veiculoDto);

        //        Assert.False(postVeiculoResponse.IsSuccessStatusCode);
        //        var errorContent = await postVeiculoResponse.Content.ReadAsStringAsync();
        //        Assert.Contains("Placa", errorContent, StringComparison.OrdinalIgnoreCase);
        //    }
        //    finally
        //    {
        //        factory.Dispose();
        //        if (File.Exists(dbPath)) File.Delete(dbPath);
        //    }
        //}

        //[Fact]
        //public async Task Atualizar_Veiculo_De_Cliente_Existente()
        //{
        //    var (factory, client, dbPath) = TestHelpers.CreateFactoryWithTempFileDb();
        //    try
        //    {
        //        var clienteDto = new ClienteDto
        //        {
        //            Nome = "Pedro Santos",
        //            Email = "pedro@exemplo.com",
        //            Documento = "42975620063"
        //        };

        //        var postClienteResponse = await client.PostAsJsonAsync("/api/Cliente", clienteDto);
        //        var responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        postClienteResponse.EnsureSuccessStatusCode();
        //        var clienteId = await postClienteResponse.Content.ReadFromJsonAsync<Guid>();

        //        var veiculoDto = new VeiculoDto
        //        {
        //            Marca = "Fiat",
        //            Modelo = "Uno",
        //            Ano = 2018,
        //            Placa = "XYZ-9876",
        //            ClienteId = clienteId
        //        };

        //        var postVeiculoResponse = await client.PostAsJsonAsync("/api/Veiculo", veiculoDto);
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        postVeiculoResponse.EnsureSuccessStatusCode();
        //        var veiculoId = await postVeiculoResponse.Content.ReadFromJsonAsync<Guid>();

        //        var getVeiculoResponse = await client.GetAsync($"/api/Veiculo/{veiculoId}");
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        getVeiculoResponse.EnsureSuccessStatusCode();
        //        var veiculoParaAtualizar = await getVeiculoResponse.Content.ReadFromJsonAsync<VeiculoDto>();

        //        string placaOriginal = veiculoParaAtualizar!.Placa;
        //        veiculoParaAtualizar.Modelo = "Uno Way";
        //        veiculoParaAtualizar.Ano = 2019;
        //        veiculoParaAtualizar.Placa = placaOriginal;

        //        var putVeiculoResponse = await client.PutAsJsonAsync("/api/Veiculo", veiculoParaAtualizar);
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        putVeiculoResponse.EnsureSuccessStatusCode();

        //        var getVeiculoAtualizadoResponse = await client.GetAsync($"/api/Veiculo/{veiculoId}");
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        getVeiculoAtualizadoResponse.EnsureSuccessStatusCode();
        //        var veiculoAtualizado = await getVeiculoAtualizadoResponse.Content.ReadFromJsonAsync<VeiculoDto>();

        //        Assert.NotNull(veiculoAtualizado);
        //        Assert.Equal("Uno Way", veiculoAtualizado!.Modelo);
        //        Assert.Equal(2019, veiculoAtualizado.Ano);
        //        Assert.Equal(placaOriginal, veiculoAtualizado.Placa);
        //    }
        //    finally
        //    {
        //        factory.Dispose();
        //        if (File.Exists(dbPath)) File.Delete(dbPath);
        //    }
        //}

        //[Fact]
        //public async Task Deletar_Veiculo_Deve_Remover_Da_Lista_Do_Cliente()
        //{
        //    var (factory, client, dbPath) = TestHelpers.CreateFactoryWithTempFileDb();
        //    try
        //    {
        //        var clienteDto = new ClienteDto
        //        {
        //            Nome = "Ana Oliveira",
        //            Email = "ana@exemplo.com",
        //            Documento = "42975620063"
        //        };

        //        var postClienteResponse = await client.PostAsJsonAsync("/api/Cliente", clienteDto);
        //        var responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        postClienteResponse.EnsureSuccessStatusCode();
        //        var clienteId = await postClienteResponse.Content.ReadFromJsonAsync<Guid>();

        //        var veiculoDto1 = new VeiculoDto
        //        {
        //            Marca = "Chevrolet",
        //            Modelo = "Onix",
        //            Ano = 2020,
        //            Placa = "JKL-5678",
        //            ClienteId = clienteId
        //        };

        //        var postVeiculo1Response = await client.PostAsJsonAsync("/api/Veiculo", veiculoDto1);
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        postVeiculo1Response.EnsureSuccessStatusCode();
        //        var veiculo1Id = await postVeiculo1Response.Content.ReadFromJsonAsync<Guid>();

        //        var veiculoDto2 = new VeiculoDto
        //        {
        //            Marca = "Volkswagen",
        //            Modelo = "Gol",
        //            Ano = 2017,
        //            Placa = "MNO2P34",
        //            ClienteId = clienteId
        //        };

        //        var postVeiculo2Response = await client.PostAsJsonAsync("/api/Veiculo", veiculoDto2);
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        postVeiculo2Response.EnsureSuccessStatusCode();
        //        var veiculo2Id = await postVeiculo2Response.Content.ReadFromJsonAsync<Guid>();

        //        var getVeiculosInitialResponse = await client.GetAsync($"/api/Veiculo/todos/{clienteId}");
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        getVeiculosInitialResponse.EnsureSuccessStatusCode();
        //        var veiculosInicial = await getVeiculosInitialResponse.Content.ReadFromJsonAsync<IEnumerable<VeiculoDto>>();
        //        Assert.Equal(2, veiculosInicial!.Count());

        //        var deleteResponse = await client.DeleteAsync($"/api/Veiculo/{veiculo1Id}");
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        deleteResponse.EnsureSuccessStatusCode();

        //        var getVeiculosFinalResponse = await client.GetAsync($"/api/Veiculo/todos/{clienteId}");
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        getVeiculosFinalResponse.EnsureSuccessStatusCode();
        //        var veiculosFinal = await getVeiculosFinalResponse.Content.ReadFromJsonAsync<IEnumerable<VeiculoDto>>();

        //        Assert.NotNull(veiculosFinal);
        //        Assert.Single(veiculosFinal!);
        //        Assert.Equal(veiculo2Id, veiculosFinal.First().VeiculoId);
        //    }
        //    finally
        //    {
        //        factory.Dispose();
        //        if (File.Exists(dbPath)) File.Delete(dbPath);
        //    }
        //}

        //[Fact]
        //public async Task Deletar_Cliente_Com_Veiculos_Deve_Remover_Todos_Veiculos()
        //{
        //    var (factory, client, dbPath) = TestHelpers.CreateFactoryWithTempFileDb();
        //    try
        //    {
        //        var clienteDto = new ClienteDto
        //        {
        //            Nome = "Carlos Mendes",
        //            Email = "carlos@exemplo.com",
        //            Documento = "42975620063"
        //        };

        //        var postClienteResponse = await client.PostAsJsonAsync("/api/Cliente", clienteDto);
        //        var responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        postClienteResponse.EnsureSuccessStatusCode();
        //        var clienteId = await postClienteResponse.Content.ReadFromJsonAsync<Guid>();

        //        var veiculoDto = new VeiculoDto
        //        {
        //            Marca = "Nissan",
        //            Modelo = "Kicks",
        //            Ano = 2022,
        //            Placa = "UVW-1357",
        //            ClienteId = clienteId
        //        };

        //        var postVeiculoResponse = await client.PostAsJsonAsync("/api/Veiculo", veiculoDto);
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        postVeiculoResponse.EnsureSuccessStatusCode();

        //        var deleteClienteResponse = await client.DeleteAsync($"/api/Cliente/{clienteId}");
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        deleteClienteResponse.EnsureSuccessStatusCode();

        //        var getClienteResponse = await client.GetAsync($"/api/Cliente/{clienteId}");
        //        Assert.Equal(HttpStatusCode.NotFound, getClienteResponse.StatusCode);

        //        var getVeiculosResponse = await client.GetAsync($"/api/Veiculo/todos/{clienteId}");
        //        responseString = await postClienteResponse.Content.ReadAsStringAsync();
        //        Console.WriteLine(responseString);
        //        getVeiculosResponse.EnsureSuccessStatusCode();
        //        var veiculos = await getVeiculosResponse.Content.ReadFromJsonAsync<IEnumerable<VeiculoDto>>();

        //        Assert.Empty(veiculos!);
        //    }
        //    finally
        //    {
        //        factory.Dispose();
        //        if (File.Exists(dbPath)) File.Delete(dbPath);
        //    }
        //}
    }
}
