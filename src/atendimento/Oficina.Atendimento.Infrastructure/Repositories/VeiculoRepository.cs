using Microsoft.EntityFrameworkCore;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Infrastructure.Data;
using Oficina.Common.Infrastructure.Repositories;

namespace Oficina.Atendimento.Infrastructure.Repositories
{
    public class VeiculoRepository : GenericRepository<Veiculo, AtendimentoDbContext>, IVeiculoRepository
    {
        public VeiculoRepository(AtendimentoDbContext context): base(context) { }

        /// <summary>
        /// Exclui todos os veículos associados a um cliente específico
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <returns>Número de veículos excluidos</returns>
        public async Task<int> DeletarPorClienteIdAsync(Guid clienteId)
        {
            var veiculos = await _dbSet
                .Where(v => v.ClienteId == clienteId)
                .ToListAsync();

            foreach (var veiculo in veiculos)
                _dbSet.Remove(veiculo);

            await _context.SaveChangesAsync();
            return veiculos.Count;
        }
    }
}
