using Microsoft.EntityFrameworkCore;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Infrastructure.Data;
using Oficina.Common.Infrastructure.Repositories;

namespace Oficina.Atendimento.Infrastructure.Repositories
{
    public class ClienteRepository: GenericRepository<Cliente, AtendimentoDbContext>, IClienteRepository
    {
        public ClienteRepository(AtendimentoDbContext context) : base(context) { }

        public async Task<bool> ExisteEmailAsync(string email, Guid? ignoreId = null)
        {
            var query = _dbSet.AsQueryable();
            if (ignoreId.HasValue)
                query = query.Where(c => c.Id != ignoreId.Value);

            return await query.AnyAsync(c => c.Email == email);
        }

        public async Task<bool> ExisteDocumentoAsync(string documento, Guid? ignoreId = null)
        {
            var query = _dbSet.AsQueryable();
            if (ignoreId.HasValue)
                query = query.Where(c => c.Id != ignoreId.Value);

            return await query.AnyAsync(c => c.Documento.Numero == documento);
        }
    }
}
