using Microsoft.EntityFrameworkCore;
using Oficina.Atendimento.Domain.Entities;

namespace Oficina.Atendimento.Infrastructure.Data
{
    public class AtendimentoDbContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<OrdemDeServico> OrdensDeServico { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Orcamento> Orcamentos { get; set; }

        public AtendimentoDbContext(DbContextOptions<AtendimentoDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>(builder =>
            {
                builder.OwnsOne(c => c.Documento, doc =>
                {
                    doc.Property(d => d.Numero)
                        .HasColumnName("Documento")
                        .IsRequired();
                });

                builder.HasMany(c => c.Veiculos)
                   .WithOne(v => v.Cliente)
                   .HasForeignKey(v => v.ClienteId)
                   .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Veiculo>(entity =>
            {
                entity.OwnsOne(c => c.Placa, doc =>
                {
                    doc.Property(d => d.Numero)
                        .HasColumnName("Placa")
                        .IsRequired();
                });

                entity.HasOne(v => v.Cliente)
                    .WithMany(c => c.Veiculos)
                    .HasForeignKey(v => v.ClienteId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrdemDeServico>(entity =>
            {
                entity.Property(o => o.DataCriacao)
                      .IsRequired();

                entity.HasOne(o => o.Cliente)
                      .WithMany()
                      .HasForeignKey(o => o.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Veiculo)
                    .WithMany()
                    .HasForeignKey(o => o.VeiculoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.OwnsMany(o => o.Pecas, p =>
                {
                    p.WithOwner().HasForeignKey("OrdemDeServicoId");
                    p.Property<int>("Id"); // PK surrogate
                    p.HasKey("Id");
                    
                    p.Property(x => x.PecaId).IsRequired();
                    p.Property(x => x.NomePeca).HasMaxLength(200).IsRequired();
                    p.Property(x => x.Quantidade).IsRequired();
                    p.Property(x => x.Preco).HasColumnType("decimal(18,2)").IsRequired();
                    p.ToTable("OrdemDeServicoPecas"); // tabela separada
                });

                entity.OwnsMany(o => o.Servicos, s =>
                {
                    s.WithOwner().HasForeignKey("OrdemDeServicoId");
                    s.Property<int>("Id"); // PK surrogate
                    s.HasKey("Id");
                    s.Property(x => x.ServicoId).IsRequired();
                    s.Property(x => x.NomeServico).HasMaxLength(200).IsRequired();
                    s.Property(x => x.Preco).HasColumnType("decimal(18,2)").IsRequired();
                    s.ToTable("OrdemDeServicoServicos"); // tabela separada
                });

                 entity.HasOne(o => o.Orcamento)
                    .WithOne(c => c.OrdemDeServico)
                    .HasForeignKey<Orcamento>(c => c.OrdemDeServicoId)
                    .IsRequired();
            });
               

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Servico>().HasData(
                new Servico("Troca de óleo", 100m) { Id = Guid.Parse("11111111-aaaa-aaaa-aaaa-111111111111"), DataCriacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), DataAtualizacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc) },
                new Servico("Alinhamento", 150m) { Id = Guid.Parse("22222222-bbbb-bbbb-bbbb-222222222222"), DataCriacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), DataAtualizacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc) },
                new Servico("Balanceamento", 120m) { Id = Guid.Parse("33333333-cccc-cccc-cccc-333333333333"), DataCriacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), DataAtualizacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc) },
                new Servico("Troca de filtro de ar", 80m) { Id = Guid.Parse("44444444-dddd-dddd-dddd-444444444444"), DataCriacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), DataAtualizacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc) },
                new Servico("Troca de pastilha de freio", 200m) { Id = Guid.Parse("55555555-eeee-eeee-eeee-555555555555"), DataCriacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), DataAtualizacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc) },
                new Servico("Revisão completa", 350m) { Id = Guid.Parse("66666666-ffff-ffff-ffff-666666666666"), DataCriacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), DataAtualizacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
