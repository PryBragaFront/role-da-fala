using Microsoft.EntityFrameworkCore;
using RoleDaFala.Dominio.Entidades;

namespace RoleDaFala.Api.Persistencia;

/// <summary>
/// Única parte do projeto que fala com o banco de verdade (SQLite).
///
/// Só mapeia Conta por enquanto: Participante e Atividade continuam em memória
/// até a próxima etapa (ver docs/2-Arquitetura.md). O domínio não conhece o
/// EF Core — só esta classe, na camada de infraestrutura, sabe que existe.
/// </summary>
public class RoleDaFalaDbContext(DbContextOptions<RoleDaFalaDbContext> opcoes) : DbContext(opcoes)
{
    public DbSet<Conta> Contas => Set<Conta>();

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
    }
}
