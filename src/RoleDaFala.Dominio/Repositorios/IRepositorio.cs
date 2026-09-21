using RoleDaFala.Dominio.Entidades;

namespace RoleDaFala.Dominio.Repositorios;

/// <summary>
/// Contrato de armazenamento, igual para qualquer entidade.
///
/// GENÉRICOS: um só contrato serve para Participante, Atividade e Progresso.
/// A restrição 'where T : Entidade' garante que só entra o que tem Id.
/// A API depende desta interface, não do banco. Trocar SQLite por outro
/// banco não muda nada no domínio.
/// </summary>
public interface IRepositorio<T> where T : Entidade
{
    Task<T?> ObterPorIdAsync(int id);
    Task<IReadOnlyList<T>> ListarAsync();
    Task<T> AdicionarAsync(T entidade);
    Task AtualizarAsync(T entidade);
    Task<bool> RemoverAsync(int id);
}
