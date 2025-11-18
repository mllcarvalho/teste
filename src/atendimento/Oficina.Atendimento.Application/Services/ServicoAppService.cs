using Oficina.Atendimento.Application.Dto;
using Oficina.Atendimento.Application.IServices;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;

namespace Oficina.Atendimento.Application.Services
{
    public class ServicoAppService : IServicoAppService
    {
        private readonly IServicoRepository _servicoRepo;
        public ServicoAppService(IServicoRepository servicoRepo)
        {
            _servicoRepo = servicoRepo;
        }

        public async Task<Guid> CriarAsync(ServicoDto dto)
        {
            var servico = new Servico(dto.Nome, dto.Preco);

            await _servicoRepo.AddAsync(servico);
            await _servicoRepo.SaveChangesAsync();

            return servico.Id;
        }

        public async Task<ServicoDto?> ObterAsync(Guid id)
        {
            var servico = await _servicoRepo.GetByIdAsync(id);
            if (servico == null) return null;

            return new ServicoDto
            {
                ServicoId = servico.Id,
                Nome = servico.Nome,
                Preco = servico.Preco
            };
        }

        public async Task<IEnumerable<ServicoDto>> ObterTodosAsync(int page = 1, int pageSize = 10)
        {
            var servicos = await _servicoRepo.GetAllAsync(page, pageSize);
            return servicos.Select(c => new ServicoDto
            {
                Nome = c.Nome,
                Preco = c.Preco,
                ServicoId = c.Id
            });
        }
        public async Task AtualizarAsync(ServicoDto dto)
        {
            var servico = await _servicoRepo.GetByIdAsync(dto.ServicoId);
            if (servico == null)
                throw new ArgumentException("Serviço não encontrada.");

            servico.Atualizar(dto.Nome, dto.Preco);

            _servicoRepo.Update(servico);
            await _servicoRepo.SaveChangesAsync();
        }

        public async Task DeletarAsync(Guid id)
        {
            var servico = await _servicoRepo.GetByIdAsync(id);
            if (servico == null)
                throw new ArgumentException("Serviço não encontrado.");

            _servicoRepo.Remove(servico);
            await _servicoRepo.SaveChangesAsync();
        }
    }
}
