using Microsoft.EntityFrameworkCore;
using GestaoEventos.API.Models;
using GestaoEventos.API.Helpers; // Garanta que este using está aqui para o PasswordHasher
using System;

namespace GestaoEventos.API.Data
{
    public class EventosContext : DbContext
    {
        public EventosContext(DbContextOptions<EventosContext> options) : base(options)
        {
        }

        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Ingresso> Ingressos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Evento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Descricao).HasMaxLength(1000);
                entity.Property(e => e.Local).IsRequired().HasMaxLength(300);
                entity.Property(e => e.PrecoIngresso).HasPrecision(10, 2).IsRequired();
                entity.Property(e => e.DataCriacao).IsRequired();
                entity.HasIndex(e => e.DataEvento);
                entity.HasIndex(e => e.Nome);
            });

            modelBuilder.Entity<Ingresso>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.NomeComprador).IsRequired().HasMaxLength(100);
                entity.Property(i => i.EmailComprador).IsRequired().HasMaxLength(150);
                entity.Property(i => i.TelefoneComprador).IsRequired().HasMaxLength(20);
                entity.Property(i => i.ValorPago).HasPrecision(10, 2).IsRequired();
                entity.Property(i => i.MotivoDevolucao).HasMaxLength(500);
                entity.Property(i => i.DataCriacao).IsRequired();
                entity.HasOne(i => i.Evento)
                      .WithMany(e => e.Ingressos)
                      .HasForeignKey(i => i.EventoId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(i => i.EmailComprador);
                entity.HasIndex(i => new { i.EventoId, i.Ativo });
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.PasswordSalt).IsRequired();
                entity.Property(u => u.Papel).IsRequired().HasMaxLength(50);
                // --- ADICIONADO NomeUsuario À CONFIGURAÇÃO DA ENTIDADE ---
                entity.Property(u => u.NomeUsuario).IsRequired().HasMaxLength(100);
                entity.HasIndex(u => u.NomeUsuario).IsUnique(); // Nome de usuário também deve ser único
            });

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            var dataAtual = DateTime.UtcNow;

            modelBuilder.Entity<Evento>().HasData(
                new Evento
                {
                    Id = 1,
                    Nome = "Conferência de Tecnologia 2025",
                    Descricao = "Evento sobre as últimas tendências em tecnologia e inovação",
                    DataEvento = dataAtual.AddMonths(2),
                    Local = "Centro de Convenções - São Paulo, SP",
                    PrecoIngresso = 150.00m,
                    CapacidadeMaxima = 500,
                    IngressosVendidos = 0,
                    Ativo = true,
                    DataCriacao = dataAtual
                },
                new Evento
                {
                    Id = 2,
                    Nome = "Workshop de Angular Avançado",
                    Descricao = "Workshop prático sobre desenvolvimento avançado com Angular",
                    DataEvento = dataAtual.AddMonths(1).AddDays(15),
                    Local = "Auditório Tech Hub - Rio de Janeiro, RJ",
                    PrecoIngresso = 80.00m,
                    CapacidadeMaxima = 100,
                    IngressosVendidos = 0,
                    Ativo = true,
                    DataCriacao = dataAtual
                },
                new Evento
                {
                    Id = 3,
                    Nome = "Meetup de .NET",
                    Descricao = "Encontro da comunidade .NET para networking e aprendizado",
                    DataEvento = dataAtual.AddMonths(1).AddDays(7),
                    Local = "Coworking Innovation - Belo Horizonte, MG",
                    PrecoIngresso = 25.00m,
                    CapacidadeMaxima = 50,
                    IngressosVendidos = 0,
                    Ativo = true,
                    DataCriacao = dataAtual
                }
            );

            PasswordHasher.CreatePasswordHash("admin123", out byte[] adminPasswordHash, out byte[] adminPasswordSalt);

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Email = "admin@eventhub.com",
                    PasswordHash = adminPasswordHash,
                    PasswordSalt = adminPasswordSalt,
                    Papel = "Admin",
                    NomeUsuario = "AdminEventHub" // <<< ADICIONADO NomeUsuario PARA O ADMIN
                }
                // O usuário comum será criado via tela de registro
            );
        }
    }
}