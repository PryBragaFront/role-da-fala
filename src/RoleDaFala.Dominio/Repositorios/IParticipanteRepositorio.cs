using RoleDaFala.Dominio.Entidades;

namespace RoleDaFala.Dominio.Repositorios;

/// <summary>
/// HERANÇA DE INTERFACE: aproveita tudo do repositório genérico
/// e acrescenta as buscas que só fazem sentido para participantes.
/// </summary>
public interface IParticipanteRepositorio : IRepositorio<Participante>
{
    Task<Participante?> ObterPorNomeAsync(string nome);
    Task<IReadOnlyList<Participante>> ListarPorNivelAsync(Nivel nivel);
}
