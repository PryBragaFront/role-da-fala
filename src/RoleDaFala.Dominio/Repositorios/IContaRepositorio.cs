using RoleDaFala.Dominio.Entidades;

namespace RoleDaFala.Dominio.Repositorios;

/// <summary>
/// Única implementação é em SQLite (ver RoleDaFala.Api/Persistencia): diferente dos
/// outros repositórios, login precisa sobreviver a reiniciar a API.
/// </summary>
public interface IContaRepositorio : IRepositorio<Conta>
{
    Task<Conta?> ObterPorEmailAsync(string email);
}
