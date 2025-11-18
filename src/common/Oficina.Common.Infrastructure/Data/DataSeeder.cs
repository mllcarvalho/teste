using Microsoft.Extensions.DependencyInjection;
using Oficina.Common.Domain.Entities;
using Oficina.Common.Domain.Enum;
using Oficina.Common.Domain.ISecurity;

namespace Oficina.Common.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static void SeedUsers(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CommonDbContext>();

            // Se já houver usuários, não faz nada
            if (context.Users.Any())
                return;

            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var password = Environment.GetEnvironmentVariable("USERSEED__PASSWORD");

            var admin = new User(
                "admin",
                "admin@oficina.com",
                passwordHasher.Hash(password),
                UserRole.Admin                
            );
            admin.DataAtualizacao = DateTime.UtcNow;

            var atendimento = new User(
                "atendimento",
                "atendimento@oficina.com",
                passwordHasher.Hash(password),
                UserRole.Atendimento
            );
            atendimento.DataAtualizacao = DateTime.UtcNow;

            var estoque = new User(
                "estoque",
                "estoque@oficina.com",
                passwordHasher.Hash(password),
                UserRole.Estoque
            );
            estoque.DataAtualizacao = DateTime.UtcNow;

            context.Users.AddRange(admin, atendimento, estoque);
            context.SaveChanges();
        }
    }
}
