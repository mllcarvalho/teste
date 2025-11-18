using Oficina.Atendimento.Application.Dto;
using Oficina.Atendimento.Application.IServices;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Domain.ValueObjects;

namespace Oficina.Atendimento.Application.Services
{
    public class VeiculoAppService : IVeiculoAppService
    {
        private readonly IVeiculoRepository _veiculoRepo;
        public VeiculoAppService(IVeiculoRepository veiculoRepo)
        {
            _veiculoRepo = veiculoRepo;
        }

        public async Task<Guid> CriarAsync(VeiculoDto dto)
        {
            var placa = new Placa(dto.Placa);
            var veiculo = new Veiculo(dto.Modelo, placa, dto.Marca, dto.Ano, dto.ClienteId);
            await _veiculoRepo.AddAsync(veiculo);
            await _veiculoRepo.SaveChangesAsync();
            return veiculo.Id;
        }

        public async Task<VeiculoDto?> ObterAsync(Guid id)
        {
            var veiculo = await _veiculoRepo.GetByIdAsync(id);
            if (veiculo == null) return null;

            return new VeiculoDto
            {
                Ano = veiculo.Ano,
                ClienteId = veiculo.ClienteId,
                Marca = veiculo.Marca,
                Modelo = veiculo.Modelo,
                Placa = veiculo.Placa.Numero,
                VeiculoId = veiculo.Id
            };
        }
        
        public async Task<IEnumerable<VeiculoDto>> ObterTodosPorClienteAsync(Guid clientId, int page = 1, int pageSize = 10)
        {
            var veiculos = await _veiculoRepo.GetAllAsync(page, pageSize, X => X.ClienteId == clientId);
            return veiculos.Select(c => new VeiculoDto
            {
                Ano = c.Ano,
                ClienteId = c.ClienteId,
                Marca = c.Marca,
                Modelo = c.Modelo,
                Placa = c.Placa.Numero,
                VeiculoId = c.Id
            });
        }
        public async Task<IEnumerable<VeiculoDto>> ObterTodosAsync(int page = 1, int pageSize = 10)
        {
            var veiculos = await _veiculoRepo.GetAllAsync(page, pageSize);
            return veiculos.Select(c => new VeiculoDto
            {
                Ano = c.Ano,                
                Marca = c.Marca,
                Modelo = c.Modelo,
                Placa = c.Placa.Numero,
                VeiculoId = c.Id
            });
        }
        public async Task AtualizarAsync(VeiculoDto dto)
        {
            var veiculo = await _veiculoRepo.GetByIdAsync(dto.VeiculoId);
            if (veiculo == null)
                throw new ArgumentException("Veículo não encontrado.");

            veiculo.Atualizar(dto.Modelo, new Placa(dto.Placa), dto.Marca, dto.Ano);
            _veiculoRepo.Update(veiculo);
            await _veiculoRepo.SaveChangesAsync();
        }
        public async Task DeletarAsync(Guid id)
        {
            var veiculo = await _veiculoRepo.GetByIdAsync(id);
            if (veiculo == null)
                throw new ArgumentException("Veículo não encontrado.");

            _veiculoRepo.Remove(veiculo);
            await _veiculoRepo.SaveChangesAsync();
        }
    }
}
