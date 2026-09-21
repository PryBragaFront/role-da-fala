using RoleDaFala.Dominio.Entidades;

namespace RoleDaFala.Dominio.Repositorios;

/// <summary>
/// HERANÇA DE CLASSE: reaproveita o repositório genérico e só acrescenta
/// as duas buscas específicas. Nenhuma linha do CRUD é reescrita.
/// </summary>
public class ParticipanteRepositorioEmMemoria
    : RepositorioEmMemoria<Participante>, IParticipanteRepositorio
{
    public async Task<Participante?> ObterPorNomeAsync(string nome)
    {
        var todos = await ListarAsync();
        return todos.FirstOrDefault(p =>
            string.Equals(p.Nome, nome?.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IReadOnlyList<Participante>> ListarPorNivelAsync(Nivel nivel)
    {
        var todos = await ListarAsync();
        return todos.Where(p => p.Nivel == nivel).ToList();
    }
}
