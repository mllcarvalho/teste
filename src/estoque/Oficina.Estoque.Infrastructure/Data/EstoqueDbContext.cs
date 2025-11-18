using Microsoft.EntityFrameworkCore;
using Oficina.Common.Domain.Entities;
using Oficina.Estoque.Domain.Entities;
using Oficina.Estoque.Domain.Enum;

namespace Oficina.Estoque.Infrastructure.Data
{
    public  class EstoqueDbContext : DbContext
    {
        public DbSet<Peca> Pecas { get; set; } = null!;
        public DbSet<Domain.Entities.Estoque> Estoque { get; set; } = null!;

        public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options)
            : base(options)
        {

        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<Base>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Property("DataAtualizacao").CurrentValue = DateTime.UtcNow;
                }
            }
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var dataSeed = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            var p1 = new Peca("Filtro de óleo", 45.90m, TipoPeca.Peca, "FILTRO-001", "Filtro de óleo sintético para motores 1.0 a 2.0", "Bosch", 2)
            { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), DataCriacao = dataSeed, DataAtualizacao = dataSeed };
            p1.AdicionarQuantidade(10);

            var p2 = new Peca("Pastilha de freio", 120.00m, TipoPeca.Peca, "PAST-FREIO-002", "Pastilha de freio dianteira para veículos leves", "TRW", 2)
            { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), DataCriacao = dataSeed, DataAtualizacao = dataSeed };
            p2.AdicionarQuantidade(8);

            var p3 = new Peca("Óleo 5W30", 32.50m, TipoPeca.Insumo, "OLEO-5W30-003", "Óleo lubrificante sintético 5W30", "Mobil", 5)
            { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), DataCriacao = dataSeed, DataAtualizacao = dataSeed };
            p3.AdicionarQuantidade(20);

            var p4 = new Peca("Filtro de ar", 38.00m, TipoPeca.Peca, "FILTRO-AR-004", "Filtro de ar para motores flex", "Mann", 2)
            { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), DataCriacao = dataSeed, DataAtualizacao = dataSeed };
            p4.AdicionarQuantidade(6);

            var p5 = new Peca("Correia dentada", 85.00m, TipoPeca.Peca, "COR-DENT-005", "Correia dentada para motores 1.6", "Gates", 1)
            { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), DataCriacao = dataSeed, DataAtualizacao = dataSeed };
            p5.AdicionarQuantidade(4);

            var p6 = new Peca("Vela de ignição", 22.00m, TipoPeca.Peca, "VELA-IGN-006", "Vela de ignição para motores flex", "NGK", 4)
            { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), DataCriacao = dataSeed, DataAtualizacao = dataSeed };
            p6.AdicionarQuantidade(16);

            var p7 = new Peca("Amortecedor dianteiro", 210.00m, TipoPeca.Peca, "AMORT-D-007", "Amortecedor dianteiro para veículos compactos", "Monroe", 2)
            { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), DataCriacao = dataSeed, DataAtualizacao = dataSeed };
            p7.AdicionarQuantidade(2);

            modelBuilder.Entity<Peca>().HasData(p1, p2, p3, p4, p5, p6, p7);
        }
    }
}
