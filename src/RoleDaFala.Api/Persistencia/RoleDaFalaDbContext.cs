using Microsoft.EntityFrameworkCore;
using RoleDaFala.Dominio.Entidades;

namespace RoleDaFala.Api.Persistencia;

/// <summary>
/// Única parte do projeto que fala com o banco de verdade (SQLite).
///
/// Conta é mapeada direto (é uma entidade simples). Participante e Atividade
/// guardam Xp, DiasSeguidos e afins encapsulados atrás de métodos — de propósito,
/// é o núcleo de POO do projeto — então em vez de forçar o EF Core a violar
/// esse encapsulamento, eles são salvos como registros "achatados"
/// (<see cref="ParticipanteRegistro"/> e <see cref="AtividadeRegistro"/>) e os
/// repositórios (ver ParticipanteRepositorioSqlite e AtividadeRepositorioSqlite)
/// convertem de um lado para o outro. O domínio continua sem conhecer o EF Core.
/// </summary>
public class RoleDaFalaDbContext(DbContextOptions<RoleDaFalaDbContext> opcoes) : DbContext(opcoes)
{
    public DbSet<Conta> Contas => Set<Conta>();
    public DbSet<ParticipanteRegistro> Participantes => Set<ParticipanteRegistro>();
    public DbSet<AtividadeRegistro> Atividades => Set<AtividadeRegistro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conta>(conta =>
        {
            conta.ToTable("Contas");
            conta.HasKey(c => c.Id);
            conta.Property(c => c.Email).IsRequired().HasMaxLength(200);
            conta.Property(c => c.SenhaHash).IsRequired();
            conta.HasIndex(c => c.Email).IsUnique();
        });

        modelBuilder.Entity<ParticipanteRegistro>(p =>
        {
            p.ToTable("Participantes");
            p.HasKey(x => x.Id);
            p.Property(x => x.Tipo).IsRequired().HasMaxLength(20);
            p.Property(x => x.Nome).IsRequired().HasMaxLength(60);
        });

        modelBuilder.Entity<AtividadeRegistro>(a =>
        {
            a.ToTable("Atividades");
            a.HasKey(x => x.Id);
            a.Property(x => x.Tipo).IsRequired().HasMaxLength(20);
        });
    }
}
