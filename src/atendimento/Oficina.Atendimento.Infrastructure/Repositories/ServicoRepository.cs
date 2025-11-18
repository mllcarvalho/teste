using Microsoft.EntityFrameworkCore;
using Oficina.Atendimento.Domain.Entities;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Infrastructure.Data;
using Oficina.Common.Infrastructure.Repositories;

namespace Oficina.Atendimento.Infrastructure.Repositories
{
    public class ServicoRepository: GenericRepository<Servico, AtendimentoDbContext>, IServicoRepository
    {
        public ServicoRepository(AtendimentoDbContext context): base(context) {}

    }
}
