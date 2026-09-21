using Microsoft.EntityFrameworkCore;
using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Repositorios;

namespace RoleDaFala.Api.Persistencia;

/// <summary>
/// Mesma interface que a versão em memória dos outros repositórios,
/// mas gravando de verdade: o login precisa sobreviver a reiniciar a API.
/// </summary>
public class ContaRepositorioSqlite : IContaRepositorio
{
    private readonly RoleDaFalaDbContext _db;

    public ContaRepositorioSqlite(RoleDaFalaDbContext db)
    {
        _db = db;
    }

    public async Task<Conta?> ObterPorIdAsync(int id) =>
        await _db.Contas.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IReadOnlyList<Conta>> ListarAsync() =>
        await _db.Contas.ToListAsync();

    public async Task<Conta> AdicionarAsync(Conta entidade)
    {
        _db.Contas.Add(entidade);
        await _db.SaveChangesAsync();
        return entidade;
    }

    public async Task AtualizarAsync(Conta entidade)
    {
        _db.Contas.Update(entidade);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var conta = await ObterPorIdAsync(id);

        if (conta is null)
        {
            return false;
        }

        _db.Contas.Remove(conta);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Conta?> ObterPorEmailAsync(string email)
    {
        var normalizado = email.Trim().ToLowerInvariant();
        return await _db.Contas.FirstOrDefaultAsync(c => c.Email == normalizado);
    }
}
