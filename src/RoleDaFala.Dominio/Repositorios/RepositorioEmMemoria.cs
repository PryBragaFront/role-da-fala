using RoleDaFala.Dominio.Entidades;

namespace RoleDaFala.Dominio.Repositorios;

/// <summary>
/// Implementação em memória, usada nos testes e na demonstração sem banco.
///
/// Mostra que a interface funciona com mais de uma implementação: esta guarda
/// num dicionário, a do projeto da API guarda no SQLite, e o domínio não vê diferença.
/// </summary>
public class RepositorioEmMemoria<T> : IRepositorio<T> where T : Entidade
{
    private readonly Dictionary<int, T> _itens = new();
    private int _proximoId = 1;

    public Task<T?> ObterPorIdAsync(int id) =>
        Task.FromResult(_itens.TryGetValue(id, out var item) ? item : null);

    public Task<IReadOnlyList<T>> ListarAsync() =>
        Task.FromResult<IReadOnlyList<T>>(_itens.Values.ToList());

    public Task<T> AdicionarAsync(T entidade)
    {
        ArgumentNullException.ThrowIfNull(entidade);

        if (entidade.Id == 0)
        {
            entidade.DefinirId(_proximoId++);
        }

        _itens[entidade.Id] = entidade;
        return Task.FromResult(entidade);
    }

    public Task AtualizarAsync(T entidade)
    {
        _itens[entidade.Id] = entidade;
        return Task.CompletedTask;
    }

    public Task<bool> RemoverAsync(int id) => Task.FromResult(_itens.Remove(id));
}
