using GestaoEventos.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GestaoEventos.API.Data
{
    /// <summary>
    /// Contexto do Entity Framework para o banco de dados
    /// </summary>
    public class EventosContext : DbContext
    {
        public EventosContext(DbContextOptions<EventosContext> options) : base(options)
        {
        }

        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Ingresso> Ingressos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Evento
            modelBuilder.Entity<Evento>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nome)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.Descricao)
                      .HasMaxLength(1000);

                entity.Property(e => e.Local)
                      .IsRequired()
                      .HasMaxLength(300);

                entity.Property(e => e.PrecoIngresso)
                      .HasPrecision(10, 2)
                      .IsRequired();

                entity.Property(e => e.DataCriacao)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.DataEvento);
                entity.HasIndex(e => e.Nome);
            });

            // Configuração da entidade Ingresso
            modelBuilder.Entity<Ingresso>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.NomeComprador)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(i => i.EmailComprador)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(i => i.TelefoneComprador)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(i => i.ValorPago)
                      .HasPrecision(10, 2)
                      .IsRequired();

                entity.Property(i => i.MotivoDevolucao)
                      .HasMaxLength(500);

                entity.Property(i => i.DataCriacao)
                      .HasDefaultValueSql("GETDATE()");

                // Relacionamento com Evento
                entity.HasOne(i => i.Evento)
                      .WithMany(e => e.Ingressos)
                      .HasForeignKey(i => i.EventoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(i => i.EmailComprador);
                entity.HasIndex(i => new { i.EventoId, i.Ativo });
            });

            // Dados iniciais (Seed)
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Evento>().HasData(
                new Evento
                {
                    Id = 1,
                    Nome = "Conferência de Tecnologia 2024",
                    Descricao = "Evento sobre as últimas tendências em tecnologia e inovação",
                    DataEvento = DateTime.Now.AddDays(30),
                    Local = "Centro de Convenções - São Paulo, SP",
                    PrecoIngresso = 150.00m,
                    CapacidadeMaxima = 500,
                    IngressosVendidos = 0,
                    Ativo = true,
                    DataCriacao = DateTime.Now
                },
                new Evento
                {
                    Id = 2,
                    Nome = "Workshop de Angular Avançado",
                    Descricao = "Workshop prático sobre desenvolvimento avançado com Angular",
                    DataEvento = DateTime.Now.AddDays(15),
                    Local = "Auditório Tech Hub - Rio de Janeiro, RJ",
                    PrecoIngresso = 80.00m,
                    CapacidadeMaxima = 100,
                    IngressosVendidos = 0,
                    Ativo = true,
                    DataCriacao = DateTime.Now
                },
                new Evento
                {
                    Id = 3,
                    Nome = "Meetup de .NET",
                    Descricao = "Encontro da comunidade .NET para networking e aprendizado",
                    DataEvento = DateTime.Now.AddDays(7),
                    Local = "Coworking Innovation - Belo Horizonte, MG",
                    PrecoIngresso = 25.00m,
                    CapacidadeMaxima = 50,
                    IngressosVendidos = 0,
                    Ativo = true,
                    DataCriacao = DateTime.Now
                }
            );
        }
    }
}