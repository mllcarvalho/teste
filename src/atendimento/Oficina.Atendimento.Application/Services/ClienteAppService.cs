using Oficina.Atendimento.Application.Dto;
using Oficina.Atendimento.Application.IServices;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Domain.ValueObjects;

namespace Oficina.Atendimento.Application.Services
{
    public class ClienteAppService : IClienteAppService
    {
        private readonly IClienteRepository _clienteRepo;
        private readonly IVeiculoRepository _veiculoRepo;
        public ClienteAppService(IClienteRepository clienteRepo, IVeiculoRepository veiculoRepo)
        {
            _clienteRepo = clienteRepo;
            _veiculoRepo = veiculoRepo;
        }

        public async Task<Guid> CriarAsync(CriarClienteDto dto)
        {
            var doc = new CpfCnpj(dto.Documento);
            
            if (await _clienteRepo.ExisteDocumentoAsync(doc.Numero))
                throw new ArgumentException("Já existe um cliente com este documento.");

            if (await _clienteRepo.ExisteEmailAsync(dto.Email))
                throw new ArgumentException("Já existe um cliente com este e-mail.");

            var cliente = new Cliente(doc, dto.Nome, dto.Email);
           
            await _clienteRepo.AddAsync(cliente);
            await _clienteRepo.SaveChangesAsync();

            return cliente.Id;
        }

        public async Task<ClienteDto?> ObterAsync(Guid id)
        {
            var cliente = await _clienteRepo.GetByIdAsync(id);
            if (cliente == null) return null;

            return new ClienteDto
            {
                ClienteId = cliente.Id,
                Documento = cliente.Documento.ToString(),
                Nome = cliente.Nome,
                Email = cliente.Email
            };
        }

        public async Task<IEnumerable<ClienteDto>> ObterTodosAsync(int page =1, int pageSize = 10)
        {
            var clientes = await _clienteRepo.GetAllAsync(page, pageSize);
            return clientes.Select(c => new ClienteDto
            {
                ClienteId = c.Id,
                Documento = c.Documento.ToString(),
                Nome = c.Nome,
                Email = c.Email
            });
        }

        public async Task AtualizarAsync(ClienteDto dto)
        {
            var cliente = await _clienteRepo.GetByIdAsync(dto.ClienteId);
            if (cliente == null)
                throw new ArgumentException("Cliente não encontrado.");

            var doc = new CpfCnpj(dto.Documento);

            if (await _clienteRepo.ExisteDocumentoAsync(doc.Numero, dto.ClienteId))
                throw new ArgumentException("Já existe um cliente com este documento.");

            if (await _clienteRepo.ExisteEmailAsync(dto.Email, dto.ClienteId))
                throw new ArgumentException("Já existe um cliente com este e-mail.");

            cliente.Atualizar(dto.Nome, dto.Email, doc);

            _clienteRepo.Update(cliente);
            await _clienteRepo.SaveChangesAsync();
        }

        public async Task DeletarAsync(Guid id)
        {
            var cliente = await _clienteRepo.GetByIdAsync(id);
            if (cliente == null)
                throw new ArgumentException("Cliente não encontrado.");

            await _veiculoRepo.DeletarPorClienteIdAsync(id);

            _clienteRepo.Remove(cliente);
            await _clienteRepo.SaveChangesAsync();
        }
    }
}
