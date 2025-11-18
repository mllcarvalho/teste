using Oficina.Atendimento.Application.Dto;
using Oficina.Atendimento.Application.IServices;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Domain.Services;
using Oficina.Atendimento.Domain.ValueObjects;
using Oficina.Common.Domain.Entities;
using Oficina.Common.Domain.IHelper;
using Oficina.Estoque.Application.IServices;

namespace Oficina.Atendimento.Application.Services
{
    public class OrdemServicoAppService : IOrdemServicoAppService
    {
        private readonly IOrdemDeServicoRepository _ordemRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly IServicoRepository _servicoRepo;
        private readonly IOrcamentoRepository _orcamentoRepo;
        private readonly IEstoqueAppService _estoqueAppService;
        private readonly IPecaAppService _pecaAppService;
        private readonly IEmailHelper _emailService;        
        private const string ordemNaoEncontradaMsg = "Ordem de serviço não encontrada.";
        private const string DateTimeFormat = "dd/MM/yyyy HH:mm";

        public OrdemServicoAppService(
            IOrdemDeServicoRepository ordemRepo,
            IClienteRepository clienteRepo,
            IServicoRepository servicoRepo,
            IEstoqueAppService estoqueAppService,
            IPecaAppService pecaAppService,
            IOrcamentoRepository orcamentoRepo,
            IEmailHelper emailService)
        {
            _ordemRepo = ordemRepo;
            _clienteRepo = clienteRepo;
            _servicoRepo = servicoRepo;
            _estoqueAppService = estoqueAppService;
            _pecaAppService = pecaAppService;
            _orcamentoRepo = orcamentoRepo;
            _emailService = emailService;
        }

        public async Task<Guid> CriarAsync(string clienteDoc, string veiculoPlaca)
        {
            var cliente = await _clienteRepo.GetAsync(x => x.Documento.Numero == clienteDoc, x => x.Veiculos);
            if (cliente == null) throw new InvalidOperationException("Cliente não encontrado.");

            var veiculo = cliente.Veiculos.FirstOrDefault(v => v.Placa.Numero == veiculoPlaca);
            if (veiculo == null) throw new InvalidOperationException("Veículo não encontrado.");

            var ordem = new OrdemDeServico(cliente.Id, veiculo.Id);

            await _ordemRepo.AddAsync(ordem);
            await _ordemRepo.SaveChangesAsync();

            return ordem.Id;
        }

        public async Task<ListarOrdemDeServicoDto?> ObterPorIdAsync(Guid id)
        {
            var ordem = await _ordemRepo.GetByIdAsync(id, x => x.Cliente, x => x.Pecas, x => x.Servicos, x => x.Orcamento);
            if (ordem == null) return null;

            return ListarOrdemDeServicoDto(ordem);
        }

        public async Task<List<ListarOrdemDeServicoDto>> ObterTodosAsync(int page, int pageSize)
        {
            var ordens = await _ordemRepo.GetAllAsync(page, pageSize, null, x => x.Cliente, x => x.Pecas, x => x.Servicos, x => x.Orcamento);
            return ordens.Select(ordem => ListarOrdemDeServicoDto(ordem)).ToList();
        }

        public async Task IniciarDiagnostico(Guid ordemId)
        {
            var ordem = await _ordemRepo.GetByIdAsync(ordemId);
            if (ordem == null) throw new InvalidOperationException(ordemNaoEncontradaMsg);
            ordem.IniciarDiagnostico();
            _ordemRepo.Update(ordem);
            await _ordemRepo.SaveChangesAsync();
        }

        public async Task ConcluirDiagnostico(OrdemDeServicoDto dto)
        {
            var ordem = await _ordemRepo.GetByIdAsync(dto.OrdemId, x => x.Cliente);
            if (ordem == null) throw new InvalidOperationException(ordemNaoEncontradaMsg);

            // Adiciona serviços
            foreach (var servicoId in dto.ServicosIds)
            {
                var servico = await _servicoRepo.GetByIdAsync(servicoId);
                if (servico == null) throw new InvalidOperationException($"Serviço {servicoId} não encontrado.");

                var itemServico = new OrdemServicoItemServico(servico.Id, servico.Nome, servico.Preco);
                ordem.AdicionarServico(itemServico);
            }

            // Adiciona peças e reserva no estoque
            foreach (var pecaDto in dto.Pecas)
            {
                // Consulta informações atualizadas da peça no estoque
                var pecaInfo = await _pecaAppService.ObterAsync(pecaDto.PecaId);
                if (pecaInfo == null) throw new InvalidOperationException($"Peça {pecaDto.PecaId} não encontrada.");

                if (pecaInfo.Quantidade < pecaDto.Quantidade)
                    throw new InvalidOperationException($"Estoque insuficiente para a peça {pecaInfo.Nome}.");

                // Adiciona como VO na Ordem
                ordem.AdicionarPeca(new OrdemServicoPeca(
                    pecaInfo.PecaId,
                    pecaInfo.Nome,
                    pecaDto.Quantidade,
                    pecaInfo.Preco));

                // Reserva no estoque chamando outro contexto
                await _estoqueAppService.RemoverPecaDoEstoque(pecaInfo.PecaId, pecaDto.Quantidade);
            }

            var valorServicos = OrdemDeServicoDomainService.CalcularValorServico(ordem);
            var valorPecas = OrdemDeServicoDomainService.CalcularValorPecas(ordem);
            ordem.DefinirValorTotal(valorServicos, valorPecas);
            
            ordem.ConcluirDiagnostico();
            
            _ordemRepo.Update(ordem);
            await _ordemRepo.SaveChangesAsync();

            var orcamento = new Orcamento(ordem.Id, valorServicos, valorPecas);
            await _orcamentoRepo.AddAsync(orcamento);
            await _orcamentoRepo.SaveChangesAsync();

            // Envia orçamento por email de forma assíncrona (fire-and-forget)
            _ = Task.Run(async () => await EnviarOrcamentoPorEmailAsync(ordem.Cliente, ordem, ordem.Orcamento));
        }

        public async Task AprovarOrcamento(Guid ordemId)
        {
            var ordem = await _ordemRepo.GetByIdAsync(ordemId, x => x.Orcamento);
            if (ordem == null) throw new InvalidOperationException(ordemNaoEncontradaMsg);
            
            ordem.Orcamento.Aprovar();
            _orcamentoRepo.Update(ordem.Orcamento);
            await _orcamentoRepo.SaveChangesAsync();

            ordem.IniciarExcucao();
            _ordemRepo.Update(ordem);
            await _ordemRepo.SaveChangesAsync();
        }
      
        public async Task FinalizarExecucao(Guid ordemId)
        {
            var ordem = await _ordemRepo.GetByIdAsync(ordemId);
            if (ordem == null) throw new InvalidOperationException(ordemNaoEncontradaMsg);
            ordem.FinalizarExecucao();
            _ordemRepo.Update(ordem);
            await _ordemRepo.SaveChangesAsync();
        }

        public async Task Entregar(Guid ordemId)
        {
            var ordem = await _ordemRepo.GetByIdAsync(ordemId);
            if (ordem == null) throw new InvalidOperationException(ordemNaoEncontradaMsg);
            ordem.Entregar();
            _ordemRepo.Update(ordem);
            await _ordemRepo.SaveChangesAsync();
        }

        public async Task<double> CalcularTempoMedioExecucao()
        {
            var ordens = await _ordemRepo.ObterFinalizadasAsync();            
            var tempoDeExecucaoPorOrdens = ordens
                .Where(o => o.TempoExecucao.HasValue)
                .Select(o => o.TempoExecucao.Value.TotalHours)
                .ToList();

            if (!tempoDeExecucaoPorOrdens.Any())
                return 0;

            return Math.Round(tempoDeExecucaoPorOrdens.Average(), 2);
        }

        #region Private Methods

        private ListarOrdemDeServicoDto ListarOrdemDeServicoDto(OrdemDeServico ordem)
        {
            var dataAprovacaoOrcamento = ordem.Orcamento != null && ordem.Orcamento.DataAprovacao.HasValue
                ? ordem.Orcamento.DataAprovacao.Value.ToString(DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture)
                : "";

            return new ListarOrdemDeServicoDto
            {
                OrdemId = ordem.Id,
                ClienteName = ordem.Cliente.Nome,
                ClienteDocumento = ordem.Cliente.Documento.Numero,
                DataCriacao = ordem.DataCriacao.ToString(DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture),
                Status = ordem.Status.ToString(),
                CustoTotal = ordem.CustoTotal,
                DataConclusao = ordem.DataConclusao.HasValue
                    ? ordem.DataConclusao.Value.ToString(DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture)
                    : "",
                DataInicioExecucao = ordem.DataInicioExecucao.HasValue
                    ? ordem.DataInicioExecucao.Value.ToString(DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture)
                    : "",
                DataFinalizacao = ordem.DataFinalizacao.HasValue
                    ? ordem.DataFinalizacao.Value.ToString(DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture)
                    : "",
                TempoExecucao = ordem.TempoExecucao.HasValue 
                    ? $"{(int)ordem.TempoExecucao.Value.TotalHours:00}:{ordem.TempoExecucao.Value.Minutes:00}" 
                    : "",
                Pecas = ordem.Pecas.Select(p => new PecaSolicitadaDto
                {
                    PecaId = p.PecaId,
                    Nome = p.NomePeca,
                    Quantidade = p.Quantidade,
                    Preco = p.Preco
                }).ToList(),
                Servicos = ordem.Servicos.Select(s => new ServicoDto
                {
                    ServicoId = s.ServicoId,
                    Nome = s.NomeServico,
                    Preco = s.Preco
                }).ToList(),
                Orcamento = ordem.Orcamento != null ? new OrcamentoDto
                {
                    ValorTotal = ordem.Orcamento.ValorTotal,
                    ValorPecas = ordem.Orcamento.ValorPecas,
                    ValorServicos = ordem.Orcamento.ValorServicos,
                    DataAprovacao = dataAprovacaoOrcamento,
                    Status = ordem.Orcamento.Status.ToString(),
                } : new OrcamentoDto { }
            };
        }

        private async Task EnviarOrcamentoPorEmailAsync(Cliente cliente, OrdemDeServico ordem, Orcamento orcamento)
        {
            var corpoHtml = EmailTemplateService.GerarTemplateOrcamento(cliente, ordem, orcamento);

            var emailMessage = new EmailMessage
            {
                Para = cliente.Email,
                ParaNome = cliente.Nome,
                Assunto = $"Orçamento - Ordem de Serviço #{ordem.Id}",
                CorpoHtml = corpoHtml,
                CorpoTexto = $"Orçamento da Ordem de Serviço #{ordem.Id} - Valor Total: R$ {orcamento.ValorTotal:N2}",
                Metadata = new Dictionary<string, string>
                {
                    { "OrdemId", ordem.Id.ToString() },
                    { "ClienteId", cliente.Id.ToString() },
                    { "TipoEmail", "Orcamento" }
                }
            };

            await _emailService.EnviarEmailAsync(emailMessage);
        }
        #endregion
    }
}