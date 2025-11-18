using Oficina.Estoque.Application.Dto;
using Oficina.Estoque.Application.IServices;
using Oficina.Estoque.Domain.Entities;
using Oficina.Estoque.Domain.Enum;
using Oficina.Estoque.Domain.IRepositories;
using Oficina.Common.Application.Helper;

namespace Oficina.Estoque.Application.Services
{
    public class PecaAppService : IPecaAppService
    {
        private readonly IPecaRepository _pecaRepo;
        public PecaAppService(IPecaRepository pecaRepo)
        {
            _pecaRepo = pecaRepo;
        }

        public async Task<Guid> CriarAsync(PecaDto dto)
        {
            var tipoPeca = EnumConverter.ConvertToEnum<TipoPeca>(dto.Tipo);
            var peca = new Peca(
                dto.Nome,
                dto.Preco,
                tipoPeca,
                dto.Codigo,
                dto.Descricao,
                dto.Fabricante,
                dto.QuantidadeMinima
            );
            peca.AdicionarQuantidade(dto.Quantidade);

            await _pecaRepo.AddAsync(peca);
            await _pecaRepo.SaveChangesAsync();

            return peca.Id;
        }

        public async Task<PecaDto?> ObterAsync(Guid id)
        {
            var peca = await _pecaRepo.GetByIdAsync(id);
            if (peca == null) return null;

            return new PecaDto
            {
                PecaId = peca.Id,
                Nome = peca.Nome,
                Descricao = peca.Descricao,
                Quantidade = peca.Quantidade,
                Codigo = peca.Codigo,
                Fabricante = peca.Fabricante,
                Preco = peca.Preco,
                Tipo = peca.Tipo.ToString(),
                QuantidadeMinima = peca.QuantidadeMinima
            };
        }

        public async Task<IEnumerable<PecaDto>> ObterTodosAsync(int page = 1, int pageSize = 10)
        {
            var pecas = await _pecaRepo.GetAllAsync(page, pageSize);
            return pecas.Select(c => new PecaDto
            {
                PecaId = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao,
                Quantidade = c.Quantidade,
                Codigo = c.Codigo,
                Fabricante = c.Fabricante,
                Preco = c.Preco,
                Tipo = c.Tipo.ToString(),
                QuantidadeMinima = c.QuantidadeMinima
            });
        }

        public async Task AtualizarAsync(PecaDto dto)
        {
            var peca = await _pecaRepo.GetByIdAsync(dto.PecaId);
            if (peca == null)
                throw new ArgumentException("Peça não encontrada.");
            
            peca.Atualizar(
               dto.Nome,
               dto.Preco,
               EnumConverter.ConvertToEnum<TipoPeca>(dto.Tipo),
               dto.Codigo,
               dto.Descricao,
               dto.Fabricante,
               dto.QuantidadeMinima
           );

            _pecaRepo.Update(peca);
            await _pecaRepo.SaveChangesAsync();
        }

        public async Task DeletarAsync(Guid id)
        {
            var peca = await _pecaRepo.GetByIdAsync(id);
            if (peca == null)
                throw new ArgumentException("Peça não encontrada.");

            _pecaRepo.Remove(peca);
            await _pecaRepo.SaveChangesAsync();
        }
    }
}
