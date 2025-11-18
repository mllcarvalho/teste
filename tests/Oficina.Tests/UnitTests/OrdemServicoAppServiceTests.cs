using Moq;
using Oficina.Atendimento.Application.Dto;
using Oficina.Atendimento.Application.Services;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.Enum;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Domain.ValueObjects;
using Oficina.Common.Domain.Entities;
using Oficina.Common.Domain.IHelper;
using Oficina.Estoque.Application.Dto;
using Oficina.Estoque.Application.IServices;
using System.Linq.Expressions;
using System.Reflection;

namespace Oficina.Tests.Atendimento.Application
{
    public class OrdemServicoAppServiceTests
    {
        private readonly Mock<IOrdemDeServicoRepository> _ordemRepoMock;
        private readonly Mock<IClienteRepository> _clienteRepoMock;
        private readonly Mock<IServicoRepository> _servicoRepoMock;
        private readonly Mock<IOrcamentoRepository> _orcamentoRepoMock;
        private readonly Mock<IEstoqueAppService> _estoqueAppMock;
        private readonly Mock<IPecaAppService> _pecaAppMock;
        private readonly Mock<IEmailHelper> _emailServiceMock;
        private readonly OrdemServicoAppService _service;

        public OrdemServicoAppServiceTests()
        {
            _ordemRepoMock = new Mock<IOrdemDeServicoRepository>();
            _clienteRepoMock = new Mock<IClienteRepository>();
            _servicoRepoMock = new Mock<IServicoRepository>();
            _orcamentoRepoMock = new Mock<IOrcamentoRepository>();
            _estoqueAppMock = new Mock<IEstoqueAppService>();
            _pecaAppMock = new Mock<IPecaAppService>();
            _emailServiceMock = new Mock<IEmailHelper>();

            _service = new OrdemServicoAppService(
                _ordemRepoMock.Object,
                _clienteRepoMock.Object,
                _servicoRepoMock.Object,
                _estoqueAppMock.Object,
                _pecaAppMock.Object,
                _orcamentoRepoMock.Object,
                _emailServiceMock.Object
            );
        }

        private void AttachVeiculosToCliente(Cliente cliente, params Veiculo[] veiculos)
        {
            var type = cliente.GetType();
            FieldInfo? field = type.GetField("_veiculos", BindingFlags.Instance | BindingFlags.NonPublic)
                                ?? type.GetField("veiculos", BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
            {
                var prop = type.GetProperty("Veiculos", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null && prop.CanWrite)
                {
                    var list = Activator.CreateInstance(typeof(List<Veiculo>)) as IList<Veiculo>;
                    foreach (var v in veiculos) list.Add(v);
                    prop.SetValue(cliente, list);
                    return;
                }
                throw new InvalidOperationException("Não foi possível anexar veículos ao Cliente (campo privado não encontrado).");
            }

            var newList = Activator.CreateInstance(typeof(List<Veiculo>)) as IList<Veiculo>;
            foreach (var v in veiculos) newList.Add(v);
            field.SetValue(cliente, newList);
        }

        [Fact]
        public async Task CriarAsync_LancaErro_Quando_ClienteNaoEncontrado()
        {
            _clienteRepoMock
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Cliente, bool>>>(),
                    It.IsAny<Expression<Func<Cliente, object>>[]>()))
                .ReturnsAsync((Cliente)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync("52998224725", "ABC1234"));

            Assert.Equal("Cliente não encontrado.", ex.Message);
            _ordemRepoMock.Verify(r => r.AddAsync(It.IsAny<OrdemDeServico>()), Times.Never);
            _ordemRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_LancaErro_Quando_VeiculoNaoEncontrado()
        {
            var cliente = new Cliente(new CpfCnpj("52998224725"), "Cliente X", "a@b.com");
            var veiculoOutro = new Veiculo("Modelo Y", new Placa("DEF-5678"), "Marca Y", 2020, cliente.Id);
            AttachVeiculosToCliente(cliente, veiculoOutro);

            _clienteRepoMock
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Cliente, bool>>>(),
                    It.IsAny<Expression<Func<Cliente, object>>[]>()))
                .ReturnsAsync(cliente);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(cliente.Documento.Numero, "ABC1234"));

            Assert.Equal("Veículo não encontrado.", ex.Message);
            _ordemRepoMock.Verify(r => r.AddAsync(It.IsAny<OrdemDeServico>()), Times.Never);
            _ordemRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaNull_QuandoNaoEncontrado()
        {
            _ordemRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                          .ReturnsAsync((OrdemDeServico)null);

            var result = await _service.ObterPorIdAsync(Guid.NewGuid());
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaDto_QuandoEncontrado()
        {
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem.GetType().GetProperty("Id")?.SetValue(ordem, Guid.NewGuid());
            ordem.AdicionarServico(new OrdemServicoItemServico(Guid.NewGuid(), "S1", 100m));
            ordem.AdicionarPeca(new OrdemServicoPeca(Guid.NewGuid(), "P1", 2, 50m));
            var orc = new Orcamento(ordem.Id, 100m, 100m);
            ordem.GetType().GetProperty("Orcamento", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem, orc);
            var cliente = new Cliente(new CpfCnpj("52998224725"), "ClienteX", "a@b.com");
            cliente.GetType().GetProperty("Id")?.SetValue(cliente, ordem.ClienteId);
            ordem.GetType().GetProperty("Cliente", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem, cliente);

            _ordemRepoMock.Setup(r => r.GetByIdAsync(ordem.Id, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                          .ReturnsAsync(ordem);

            var dto = await _service.ObterPorIdAsync(ordem.Id);

            Assert.NotNull(dto);
            Assert.Equal("Recebida", dto.Status);
            Assert.NotEmpty(dto.Pecas);
            Assert.NotEmpty(dto.Servicos);
            Assert.NotNull(dto.Orcamento);
        }

        [Fact]
        public async Task IniciarDiagnostico_LancaErro_QuandoNaoEncontrado()
        {
            _ordemRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((OrdemDeServico)null);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.IniciarDiagnostico(Guid.NewGuid()));
            Assert.Equal("Ordem de serviço não encontrada.", ex.Message);
        }

        [Fact]
        public async Task IniciarDiagnostico_Sucesso()
        {
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            _ordemRepoMock.Setup(r => r.GetByIdAsync(ordem.Id)).ReturnsAsync(ordem);
            _ordemRepoMock.Setup(r => r.Update(ordem));
            _ordemRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.IniciarDiagnostico(ordem.Id);

            Assert.Equal(OrdemStatus.EmDiagnostico, ordem.Status);
            _ordemRepoMock.Verify(r => r.Update(ordem), Times.Once);
            _ordemRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CalcularTempoMedioExecucao_RetornaZero_QuandoNenhumaOrdemFinalizada()
        {
            _ordemRepoMock.Setup(r => r.ObterFinalizadasAsync()).ReturnsAsync(new List<OrdemDeServico>());

            var result = await _service.CalcularTempoMedioExecucao();
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task CalcularTempoMedioExecucao_RetornaMediaHoras_Arredondada()
        {
            var ordem1 = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem1.GetType().GetProperty("DataInicioExecucao")?.SetValue(ordem1, DateTime.UtcNow.AddHours(-2));
            ordem1.GetType().GetProperty("DataFinalizacao")?.SetValue(ordem1, DateTime.UtcNow);

            var ordem2 = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem2.GetType().GetProperty("DataInicioExecucao")?.SetValue(ordem2, DateTime.UtcNow.AddHours(-1.5));
            ordem2.GetType().GetProperty("DataFinalizacao")?.SetValue(ordem2, DateTime.UtcNow);

            _ordemRepoMock.Setup(r => r.ObterFinalizadasAsync()).ReturnsAsync(new List<OrdemDeServico> { ordem1, ordem2 });

            var result = await _service.CalcularTempoMedioExecucao();
            var expected = Math.Round(((ordem1.TempoExecucao!.Value.TotalHours + ordem2.TempoExecucao!.Value.TotalHours) / 2), 2);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarLista_E_MapearCorretamente()
        {
            var clienteId1 = Guid.NewGuid();
            var ordem1 = new OrdemDeServico(clienteId1, Guid.NewGuid());
            ordem1.GetType().GetProperty("Id")?.SetValue(ordem1, Guid.NewGuid());

            var cliente1 = new Cliente(new CpfCnpj("52998224725"), "Cliente 1", "c1@x.com");
            cliente1.GetType().GetProperty("Id")?.SetValue(cliente1, ordem1.ClienteId);
            ordem1.GetType().GetProperty("Cliente", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem1, cliente1);

            var orc1 = new Orcamento(ordem1.Id, 0m, 0m);
            ordem1.GetType().GetProperty("Orcamento", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem1, orc1);

            var clienteId2 = Guid.NewGuid();
            var ordem2 = new OrdemDeServico(clienteId2, Guid.NewGuid());
            ordem2.GetType().GetProperty("Id")?.SetValue(ordem2, Guid.NewGuid());
            var cliente2 = new Cliente(new CpfCnpj("12345678909"), "Cliente 2", "c2@x.com");
            cliente2.GetType().GetProperty("Id")?.SetValue(cliente2, ordem2.ClienteId);
            ordem2.GetType().GetProperty("Cliente", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem2, cliente2);
            var orc2 = new Orcamento(ordem2.Id, 0m, 0m);
            ordem2.GetType().GetProperty("Orcamento", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem2, orc2);

            _ordemRepoMock.Setup(r => r.GetAllAsync(1, 10, null, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                          .ReturnsAsync(new List<OrdemDeServico> { ordem1, ordem2 });

            var result = await _service.ObterTodosAsync(1, 10);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.ClienteName == "Cliente 1" && dto.ClienteDocumento == cliente1.Documento.Numero);
            Assert.Contains(result, dto => dto.ClienteName == "Cliente 2" && dto.ClienteDocumento == cliente2.Documento.Numero);
        }

        [Fact]
        public async Task AprovarOrcamento_LancaErro_Quando_NaoEncontrada()
        {
            _ordemRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                          .ReturnsAsync((OrdemDeServico?)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AprovarOrcamento(Guid.NewGuid()));
            Assert.Equal("Ordem de serviço não encontrada.", ex.Message);
        }

        [Fact]
        public async Task AprovarOrcamento_LancaErro_QuandoOrcamentoJaAprovado()
        {
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());

            var orc = new Orcamento(ordem.Id, 100m, 50m);
            orc.Aprovar();

            ordem.GetType().GetProperty("Orcamento", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                 ?.SetValue(ordem, orc);

            _ordemRepoMock
                .Setup(r => r.GetByIdAsync(ordem.Id, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                .ReturnsAsync(ordem);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AprovarOrcamento(ordem.Id));
            Assert.Equal("Orçamento já aprovado.", ex.Message);
        }

        [Fact]
        public async Task AprovarOrcamento_Sucesso_AprovaOrcamento_E_IniciaExecucao_E_Persiste()
        {
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            var orc = new Orcamento(ordem.Id, 100m, 50m);
            ordem.IniciarDiagnostico();
            ordem.ConcluirDiagnostico();
            ordem.GetType().GetProperty("Orcamento", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem, orc);

            _ordemRepoMock.Setup(r => r.GetByIdAsync(ordem.Id, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                          .ReturnsAsync(ordem);
            _orcamentoRepoMock.Setup(r => r.Update(ordem.Orcamento));
            _ordemRepoMock.Setup(r => r.Update(ordem));
            _orcamentoRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _ordemRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.AprovarOrcamento(ordem.Id);

            Assert.Equal(OrcamentoStatus.Aprovado, ordem.Orcamento.Status);
            Assert.Equal(OrdemStatus.EmExecucao, ordem.Status);
            _orcamentoRepoMock.Verify(r => r.Update(ordem.Orcamento), Times.Once);
            _orcamentoRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _ordemRepoMock.Verify(r => r.Update(ordem), Times.Once);
            _ordemRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task FinalizarExecucao_LancaErro_Quando_NaoEncontrada()
        {
            _ordemRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((OrdemDeServico?)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.FinalizarExecucao(Guid.NewGuid()));
            Assert.Equal("Ordem de serviço não encontrada.", ex.Message);
        }

        [Fact]
        public async Task FinalizarExecucao_Sucesso_AlteraEstadoERegistraDataFinalizacao()
        {
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem.IniciarDiagnostico();
            ordem.ConcluirDiagnostico();
            ordem.IniciarExcucao();

            var start = DateTime.UtcNow.AddMinutes(-20);
            ordem.GetType().GetProperty("DataInicioExecucao")?.SetValue(ordem, start);

            _ordemRepoMock.Setup(r => r.GetByIdAsync(ordem.Id)).ReturnsAsync(ordem);
            _ordemRepoMock.Setup(r => r.Update(ordem));
            _ordemRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.FinalizarExecucao(ordem.Id);

            Assert.Equal(OrdemStatus.Finalizada, ordem.Status);
            Assert.True(ordem.DataFinalizacao.HasValue);
            Assert.True(ordem.TempoExecucao.HasValue);
            var minutes = ordem.TempoExecucao!.Value.TotalMinutes;
            Assert.InRange(minutes, 19.0, 21.0);
            _ordemRepoMock.Verify(r => r.Update(ordem), Times.Once);
            _ordemRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Entregar_LancaErro_Quando_NaoEncontrada()
        {
            _ordemRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((OrdemDeServico?)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Entregar(Guid.NewGuid()));
            Assert.Equal("Ordem de serviço não encontrada.", ex.Message);
        }

        [Fact]
        public async Task Entregar_Sucesso_DefineDataConclusaoEStatusEntrega()
        {
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());

            ordem.IniciarDiagnostico();
            ordem.ConcluirDiagnostico();
            ordem.IniciarExcucao();
            ordem.GetType().GetProperty("DataInicioExecucao")?.SetValue(ordem, DateTime.UtcNow.AddHours(-1));
            ordem.FinalizarExecucao();

            _ordemRepoMock.Setup(r => r.GetByIdAsync(ordem.Id)).ReturnsAsync(ordem);
            _ordemRepoMock.Setup(r => r.Update(ordem));
            _ordemRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.Entregar(ordem.Id);

            Assert.Equal(OrdemStatus.Entregue, ordem.Status);
            Assert.True(ordem.DataConclusao.HasValue);
            _ordemRepoMock.Verify(r => r.Update(ordem), Times.Once);
            _ordemRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ConcluirDiagnostico_LancaErro_QuandoOrdemNaoEncontrada()
        {
            var dto = new OrdemDeServicoDto
            {
                OrdemId = Guid.NewGuid(),
                ServicosIds = new List<Guid>(),
                Pecas = new List<PecaSolicitadaDto>()
            };

            _ordemRepoMock.Setup(r => r.GetByIdAsync(dto.OrdemId)).ReturnsAsync((OrdemDeServico)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ConcluirDiagnostico(dto));

            Assert.Equal("Ordem de serviço não encontrada.", ex.Message);
            _ordemRepoMock.Verify(r => r.Update(It.IsAny<OrdemDeServico>()), Times.Never);
            _orcamentoRepoMock.Verify(r => r.AddAsync(It.IsAny<Orcamento>()), Times.Never);
        }

        [Fact]
        public async Task ConcluirDiagnostico_LancaErro_Quando_ServicoNaoEncontrado()
        {
            _emailServiceMock.Setup(e => e.EnviarEmailAsync(It.IsAny<EmailMessage>())).Returns(Task.CompletedTask);

            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem.IniciarDiagnostico();
            var ordemId = Guid.NewGuid();
            ordem.GetType().GetProperty("Id")?.SetValue(ordem, ordemId);

            var servicoIdInexistente = Guid.NewGuid();

            _ordemRepoMock
                .Setup(r => r.GetByIdAsync(ordemId, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                .ReturnsAsync(ordem);
            _servicoRepoMock.Setup(r => r.GetByIdAsync(servicoIdInexistente)).ReturnsAsync((Servico)null);

            var dto = new OrdemDeServicoDto
            {
                OrdemId = ordemId,
                ServicosIds = new List<Guid> { servicoIdInexistente },
                Pecas = new List<PecaSolicitadaDto>()
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ConcluirDiagnostico(dto));

            Assert.Equal($"Serviço {servicoIdInexistente} não encontrado.", ex.Message);
            _ordemRepoMock.Verify(r => r.Update(It.IsAny<OrdemDeServico>()), Times.Never);
            _orcamentoRepoMock.Verify(r => r.AddAsync(It.IsAny<Orcamento>()), Times.Never);
            _estoqueAppMock.Verify(e => e.RemoverPecaDoEstoque(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ConcluirDiagnostico_LancaErro_Quando_PecaNaoEncontrada()
        {
            _emailServiceMock.Setup(e => e.EnviarEmailAsync(It.IsAny<EmailMessage>())).Returns(Task.CompletedTask);

            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem.IniciarDiagnostico();
            var ordemId = Guid.NewGuid();
            ordem.GetType().GetProperty("Id")?.SetValue(ordem, ordemId);

            var serv = new Servico("Troca de óleo", 150m);
            var servId = Guid.NewGuid();
            serv.GetType().GetProperty("Id")?.SetValue(serv, servId);

            var pecaIdInexistente = Guid.NewGuid();

            _ordemRepoMock
                .Setup(r => r.GetByIdAsync(ordemId, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                .ReturnsAsync(ordem);
            _servicoRepoMock.Setup(r => r.GetByIdAsync(servId)).ReturnsAsync(serv);
            _pecaAppMock.Setup(p => p.ObterAsync(pecaIdInexistente)).ReturnsAsync((PecaDto)null);

            var dto = new OrdemDeServicoDto
            {
                OrdemId = ordemId,
                ServicosIds = new List<Guid> { servId },
                Pecas = new List<PecaSolicitadaDto> { new PecaSolicitadaDto { PecaId = pecaIdInexistente, Quantidade = 2 } }
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ConcluirDiagnostico(dto));

            Assert.Equal($"Peça {pecaIdInexistente} não encontrada.", ex.Message);
            _estoqueAppMock.Verify(e => e.RemoverPecaDoEstoque(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
            _orcamentoRepoMock.Verify(r => r.AddAsync(It.IsAny<Orcamento>()), Times.Never);
        }

        [Fact]
        public async Task ConcluirDiagnostico_LancaErro_Quando_PecaEstoqueInsuficiente()
        {
            _emailServiceMock.Setup(e => e.EnviarEmailAsync(It.IsAny<EmailMessage>())).Returns(Task.CompletedTask);

            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem.IniciarDiagnostico();
            var ordemId = Guid.NewGuid();
            ordem.GetType().GetProperty("Id")?.SetValue(ordem, ordemId);

            var serv = new Servico("Alinhamento", 100m);
            var servId = Guid.NewGuid();
            serv.GetType().GetProperty("Id")?.SetValue(serv, servId);

            var pecaId = Guid.NewGuid();
            var pecaInfo = new PecaDto { PecaId = pecaId, Nome = "Filtro de Ar", Preco = 30m, Quantidade = 1 };

            _ordemRepoMock
                .Setup(r => r.GetByIdAsync(ordemId, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                .ReturnsAsync(ordem);
            _servicoRepoMock.Setup(r => r.GetByIdAsync(servId)).ReturnsAsync(serv);
            _pecaAppMock.Setup(p => p.ObterAsync(pecaId)).ReturnsAsync(pecaInfo);

            var dto = new OrdemDeServicoDto
            {
                OrdemId = ordemId,
                ServicosIds = new List<Guid> { servId },
                Pecas = new List<PecaSolicitadaDto> { new PecaSolicitadaDto { PecaId = pecaId, Quantidade = 2 } }
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ConcluirDiagnostico(dto));

            Assert.Equal($"Estoque insuficiente para a peça {pecaInfo.Nome}.", ex.Message);
            _estoqueAppMock.Verify(e => e.RemoverPecaDoEstoque(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
            _orcamentoRepoMock.Verify(r => r.AddAsync(It.IsAny<Orcamento>()), Times.Never);
        }

        [Fact]
        public async Task ConcluirDiagnostico_Sucesso_CriaOrcamentoEReservaEstoque()
        {
            _emailServiceMock.Setup(e => e.EnviarEmailAsync(It.IsAny<EmailMessage>())).Returns(Task.CompletedTask);

            var cliente = new Cliente(new CpfCnpj("52998224725"), "Cliente X", "a@b.com");
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem.IniciarDiagnostico();
            var ordemId = Guid.NewGuid();
            ordem.GetType().GetProperty("Id")?.SetValue(ordem, ordemId);
            ordem.GetType().GetProperty("Cliente", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem, cliente);

            var serv = new Servico("Balanceamento", 150m);
            var servId = Guid.NewGuid();
            serv.GetType().GetProperty("Id")?.SetValue(serv, servId);

            var pecaId = Guid.NewGuid();
            var pecaInfo = new PecaDto { PecaId = pecaId, Nome = "Pneu", Preco = 300m, Quantidade = 10 };

            _ordemRepoMock
                .Setup(r => r.GetByIdAsync(ordemId, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                .ReturnsAsync(ordem);
            _servicoRepoMock.Setup(r => r.GetByIdAsync(servId)).ReturnsAsync(serv);
            _pecaAppMock.Setup(p => p.ObterAsync(pecaId)).ReturnsAsync(pecaInfo);
            _estoqueAppMock.Setup(e => e.RemoverPecaDoEstoque(pecaId, 2)).Returns(Task.CompletedTask);
            _ordemRepoMock.Setup(r => r.Update(ordem));
            _ordemRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            Orcamento? captured = null;
            _orcamentoRepoMock.Setup(r => r.AddAsync(It.IsAny<Orcamento>())).Callback<Orcamento>(o => captured = o).Returns(Task.CompletedTask);
            _orcamentoRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new OrdemDeServicoDto
            {
                OrdemId = ordemId,
                ServicosIds = new List<Guid> { servId },
                Pecas = new List<PecaSolicitadaDto> { new PecaSolicitadaDto { PecaId = pecaId, Quantidade = 2 } }
            };

            await _service.ConcluirDiagnostico(dto);

            _estoqueAppMock.Verify(e => e.RemoverPecaDoEstoque(pecaId, 2), Times.Once);
            _ordemRepoMock.Verify(r => r.Update(ordem), Times.Once);
            _ordemRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _orcamentoRepoMock.Verify(r => r.AddAsync(It.IsAny<Orcamento>()), Times.Once);
            _orcamentoRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.NotNull(captured);
            Assert.Equal(150m, captured!.ValorServicos);
            Assert.Equal(600m, captured!.ValorPecas);
            Assert.Equal(750m, captured!.ValorTotal);
            Assert.Equal(OrdemStatus.AguardandoAprovacao, ordem.Status);
        }

        [Fact]
        public async Task ConcluirDiagnostico_Sucesso_AlteraStatusParaAguardandoAprovacao()
        {
            _emailServiceMock.Setup(e => e.EnviarEmailAsync(It.IsAny<EmailMessage>())).Returns(Task.CompletedTask);

            var cliente = new Cliente(new CpfCnpj("52998224725"), "Cliente Y", "y@b.com");
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem.IniciarDiagnostico();
            var ordemId = Guid.NewGuid();
            ordem.GetType().GetProperty("Id")?.SetValue(ordem, ordemId);
            ordem.GetType().GetProperty("Cliente", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem, cliente);

            var serv = new Servico("Lavagem", 50m);
            var servId = Guid.NewGuid();
            serv.GetType().GetProperty("Id")?.SetValue(serv, servId);

            var pecaId = Guid.NewGuid();
            var pecaInfo = new PecaDto { PecaId = pecaId, Nome = "Parafuso", Preco = 2m, Quantidade = 100 };

            _ordemRepoMock
                .Setup(r => r.GetByIdAsync(ordemId, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                .ReturnsAsync(ordem);
            _servicoRepoMock.Setup(r => r.GetByIdAsync(servId)).ReturnsAsync(serv);
            _pecaAppMock.Setup(p => p.ObterAsync(pecaId)).ReturnsAsync(pecaInfo);
            _estoqueAppMock.Setup(e => e.RemoverPecaDoEstoque(pecaId, 4)).Returns(Task.CompletedTask);
            _ordemRepoMock.Setup(r => r.Update(ordem));
            _ordemRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _orcamentoRepoMock.Setup(r => r.AddAsync(It.IsAny<Orcamento>())).Returns(Task.CompletedTask);
            _orcamentoRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new OrdemDeServicoDto
            {
                OrdemId = ordemId,
                ServicosIds = new List<Guid> { servId },
                Pecas = new List<PecaSolicitadaDto> { new PecaSolicitadaDto { PecaId = pecaId, Quantidade = 4 } }
            };

            await _service.ConcluirDiagnostico(dto);

            Assert.Equal(OrdemStatus.AguardandoAprovacao, ordem.Status);
        }

        [Fact]
        public async Task ConcluirDiagnostico_DeveReservarEstoqueDePecas()
        {
            _emailServiceMock.Setup(e => e.EnviarEmailAsync(It.IsAny<EmailMessage>())).Returns(Task.CompletedTask);

            var cliente = new Cliente(new CpfCnpj("52998224725"), "Cliente Z", "z@b.com");
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem.IniciarDiagnostico();
            var ordemId = Guid.NewGuid();
            ordem.GetType().GetProperty("Id")?.SetValue(ordem, ordemId);
            ordem.GetType().GetProperty("Cliente", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem, cliente);

            var serv = new Servico("Revisão", 200m);
            var servId = Guid.NewGuid();
            serv.GetType().GetProperty("Id")?.SetValue(serv, servId);

            var pecaAId = Guid.NewGuid();
            var pecaBId = Guid.NewGuid();
            var pecaAInfo = new PecaDto { PecaId = pecaAId, Nome = "Filtro", Preco = 40m, Quantidade = 10 };
            var pecaBInfo = new PecaDto { PecaId = pecaBId, Nome = "Correia", Preco = 120m, Quantidade = 5 };

            _ordemRepoMock
                .Setup(r => r.GetByIdAsync(ordemId, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                .ReturnsAsync(ordem);
            _servicoRepoMock.Setup(r => r.GetByIdAsync(servId)).ReturnsAsync(serv);
            _pecaAppMock.Setup(p => p.ObterAsync(pecaAId)).ReturnsAsync(pecaAInfo);
            _pecaAppMock.Setup(p => p.ObterAsync(pecaBId)).ReturnsAsync(pecaBInfo);

            _estoqueAppMock.Setup(e => e.RemoverPecaDoEstoque(pecaAId, 3)).Returns(Task.CompletedTask);
            _estoqueAppMock.Setup(e => e.RemoverPecaDoEstoque(pecaBId, 2)).Returns(Task.CompletedTask);

            _ordemRepoMock.Setup(r => r.Update(ordem));
            _ordemRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _orcamentoRepoMock.Setup(r => r.AddAsync(It.IsAny<Orcamento>())).Returns(Task.CompletedTask);
            _orcamentoRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new OrdemDeServicoDto
            {
                OrdemId = ordemId,
                ServicosIds = new List<Guid> { servId },
                Pecas = new List<PecaSolicitadaDto>
                {
                    new PecaSolicitadaDto { PecaId = pecaAId, Quantidade = 3 },
                    new PecaSolicitadaDto { PecaId = pecaBId, Quantidade = 2 }
                }
            };

            await _service.ConcluirDiagnostico(dto);

            _estoqueAppMock.Verify(e => e.RemoverPecaDoEstoque(pecaAId, 3), Times.Once);
            _estoqueAppMock.Verify(e => e.RemoverPecaDoEstoque(pecaBId, 2), Times.Once);
        }

        [Fact]
        public async Task ConcluirDiagnostico_SemEstoqueSuficiente_DeveFalhar()
        {
            _emailServiceMock.Setup(e => e.EnviarEmailAsync(It.IsAny<EmailMessage>())).Returns(Task.CompletedTask);

            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem.IniciarDiagnostico();
            var ordemId = Guid.NewGuid();
            ordem.GetType().GetProperty("Id")?.SetValue(ordem, ordemId);

            var serv = new Servico("Reparo Elétrico", 300m);
            var servId = Guid.NewGuid();
            serv.GetType().GetProperty("Id")?.SetValue(serv, servId);

            var pecaOkId = Guid.NewGuid();
            var pecaFailId = Guid.NewGuid();
            var pecaOk = new PecaDto { PecaId = pecaOkId, Nome = "Fusível", Preco = 5m, Quantidade = 10 };
            var pecaFail = new PecaDto { PecaId = pecaFailId, Nome = "Alternador", Preco = 800m, Quantidade = 0 };

            _ordemRepoMock
                .Setup(r => r.GetByIdAsync(ordemId, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                .ReturnsAsync(ordem);
            _servicoRepoMock.Setup(r => r.GetByIdAsync(servId)).ReturnsAsync(serv);
            _pecaAppMock.Setup(p => p.ObterAsync(pecaOkId)).ReturnsAsync(pecaOk);
            _pecaAppMock.Setup(p => p.ObterAsync(pecaFailId)).ReturnsAsync(pecaFail);

            _estoqueAppMock.Setup(e => e.RemoverPecaDoEstoque(pecaOkId, 2)).Returns(Task.CompletedTask);

            var dto = new OrdemDeServicoDto
            {
                OrdemId = ordemId,
                ServicosIds = new List<Guid> { servId },
                Pecas = new List<PecaSolicitadaDto>
        {
            new PecaSolicitadaDto { PecaId = pecaOkId, Quantidade = 2 },
            new PecaSolicitadaDto { PecaId = pecaFailId, Quantidade = 1 }
        }
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ConcluirDiagnostico(dto));

            Assert.Equal($"Estoque insuficiente para a peça {pecaFail.Nome}.", ex.Message);
            _estoqueAppMock.Verify(e => e.RemoverPecaDoEstoque(pecaOkId, 2), Times.Once);
            _estoqueAppMock.Verify(e => e.RemoverPecaDoEstoque(pecaFailId, It.IsAny<int>()), Times.Never);
            _orcamentoRepoMock.Verify(r => r.AddAsync(It.IsAny<Orcamento>()), Times.Never);
            _ordemRepoMock.Verify(r => r.Update(It.IsAny<OrdemDeServico>()), Times.Never);
        }

        [Fact]
        public async Task ConcluirDiagnostico_ComValoresCalculadosCorretamente()
        {
            _emailServiceMock.Setup(e => e.EnviarEmailAsync(It.IsAny<EmailMessage>())).Returns(Task.CompletedTask);

            var cliente = new Cliente(new CpfCnpj("52998224725"), "Cliente V", "v@b.com");
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            ordem.IniciarDiagnostico();
            var ordemId = Guid.NewGuid();
            ordem.GetType().GetProperty("Id")?.SetValue(ordem, ordemId);
            ordem.GetType().GetProperty("Cliente", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem, cliente);

            var serv1 = new Servico("S1", 120m); var serv1Id = Guid.NewGuid(); serv1.GetType().GetProperty("Id")?.SetValue(serv1, serv1Id);
            var serv2 = new Servico("S2", 80m); var serv2Id = Guid.NewGuid(); serv2.GetType().GetProperty("Id")?.SetValue(serv2, serv2Id);

            var peca1Id = Guid.NewGuid(); var peca1 = new PecaDto { PecaId = peca1Id, Nome = "P1", Preco = 10m, Quantidade = 100 };
            var peca2Id = Guid.NewGuid(); var peca2 = new PecaDto { PecaId = peca2Id, Nome = "P2", Preco = 25m, Quantidade = 10 };

            _ordemRepoMock
                .Setup(r => r.GetByIdAsync(ordemId, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                .ReturnsAsync(ordem);
            _servicoRepoMock.Setup(r => r.GetByIdAsync(serv1Id)).ReturnsAsync(serv1);
            _servicoRepoMock.Setup(r => r.GetByIdAsync(serv2Id)).ReturnsAsync(serv2);
            _pecaAppMock.Setup(p => p.ObterAsync(peca1Id)).ReturnsAsync(peca1);
            _pecaAppMock.Setup(p => p.ObterAsync(peca2Id)).ReturnsAsync(peca2);

            _estoqueAppMock.Setup(e => e.RemoverPecaDoEstoque(peca1Id, 3)).Returns(Task.CompletedTask);
            _estoqueAppMock.Setup(e => e.RemoverPecaDoEstoque(peca2Id, 2)).Returns(Task.CompletedTask);

            _ordemRepoMock.Setup(r => r.Update(ordem));
            _ordemRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            Orcamento? captured = null;
            _orcamentoRepoMock.Setup(r => r.AddAsync(It.IsAny<Orcamento>())).Callback<Orcamento>(o => captured = o).Returns(Task.CompletedTask);
            _orcamentoRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new OrdemDeServicoDto
            {
                OrdemId = ordemId,
                ServicosIds = new List<Guid> { serv1Id, serv2Id },
                Pecas = new List<PecaSolicitadaDto>
                {
                    new PecaSolicitadaDto { PecaId = peca1Id, Quantidade = 3 },
                    new PecaSolicitadaDto { PecaId = peca2Id, Quantidade = 2 }
                }
            };

            await _service.ConcluirDiagnostico(dto);

            Assert.NotNull(captured);
            Assert.Equal(200m, captured!.ValorServicos); // 120 + 80
            Assert.Equal(80m, captured!.ValorPecas);     // (3*10) + (2*25)
            Assert.Equal(280m, captured!.ValorTotal);
        }

        [Fact]
        public async Task ObterOrdemPorId_DeveRetornarOrdemCompleta()
        {
            var ordem = new OrdemDeServico(Guid.NewGuid(), Guid.NewGuid());
            var ordemId = Guid.NewGuid();
            ordem.GetType().GetProperty("Id")?.SetValue(ordem, ordemId);

            var cliente = new Cliente(new CpfCnpj("52998224725"), "Cliente Teste", "c@x.com");
            cliente.GetType().GetProperty("Id")?.SetValue(cliente, ordem.ClienteId);
            ordem.GetType().GetProperty("Cliente", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem, cliente);

            ordem.AdicionarServico(new OrdemServicoItemServico(Guid.NewGuid(), "Serv A", 123.45m));
            ordem.AdicionarServico(new OrdemServicoItemServico(Guid.NewGuid(), "Serv B", 10m));
            ordem.AdicionarPeca(new OrdemServicoPeca(Guid.NewGuid(), "Peca A", 1, 50m));
            ordem.AdicionarPeca(new OrdemServicoPeca(Guid.NewGuid(), "Peca B", 3, 5m));

            var orc = new Orcamento(ordem.Id, 133.45m, 65m); // total 198.45
            ordem.GetType().GetProperty("Orcamento", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(ordem, orc);

            _ordemRepoMock.Setup(r => r.GetByIdAsync(ordemId, It.IsAny<Expression<Func<OrdemDeServico, object>>[]>()))
                          .ReturnsAsync(ordem);

            var dto = await _service.ObterPorIdAsync(ordemId);

            Assert.NotNull(dto);
            Assert.Equal(ordemId, dto!.OrdemId);
            Assert.Equal("Cliente Teste", dto.ClienteName);
            Assert.Equal(cliente.Documento.Numero, dto.ClienteDocumento);
            Assert.NotNull(dto.Orcamento);
            Assert.NotNull(dto.Pecas);
            Assert.NotNull(dto.Servicos);
            Assert.Equal(2, dto.Pecas!.Count);
            Assert.Equal(2, dto.Servicos!.Count);
        }

    }
}
