using Microsoft.EntityFrameworkCore;
using RoleDaFala.Dominio.Acessibilidade;
using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Repositorios;

namespace RoleDaFala.Api.Persistencia;

/// <summary>
/// Mesma interface que a versão em memória (<see cref="ParticipanteRepositorioEmMemoria"/>),
/// agora gravando no SQLite. A troca no <c>Program.cs</c> é só isto: nenhuma
/// outra classe do projeto precisou mudar.
/// </summary>
public class ParticipanteRepositorioSqlite : IParticipanteRepositorio
{
    private readonly RoleDaFalaDbContext _db;

    public ParticipanteRepositorioSqlite(RoleDaFalaDbContext db)
    {
        _db = db;
    }

    public async Task<Participante?> ObterPorIdAsync(int id)
    {
        var registro = await _db.Participantes.FirstOrDefaultAsync(p => p.Id == id);
        return registro is null ? null : ParaDominio(registro);
    }

    public async Task<IReadOnlyList<Participante>> ListarAsync()
    {
        var registros = await _db.Participantes.ToListAsync();
        return registros.Select(ParaDominio).ToList();
    }

    public async Task<Participante> AdicionarAsync(Participante entidade)
    {
        var registro = ParaRegistro(entidade);
        _db.Participantes.Add(registro);
        await _db.SaveChangesAsync();

        entidade.DefinirId(registro.Id);
        entidade.DefinirCriadoEm(registro.CriadoEm);
        return entidade;
    }

    public async Task AtualizarAsync(Participante entidade)
    {
        var registro = await _db.Participantes.FirstOrDefaultAsync(p => p.Id == entidade.Id)
            ?? throw new InvalidOperationException($"Participante {entidade.Id} não encontrado para atualizar.");

        CopiarPara(entidade, registro);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var registro = await _db.Participantes.FirstOrDefaultAsync(p => p.Id == id);

        if (registro is null)
        {
            return false;
        }

        _db.Participantes.Remove(registro);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Participante?> ObterPorNomeAsync(string nome)
    {
        var normalizado = nome?.Trim() ?? string.Empty;
        var registro = await _db.Participantes
            .FirstOrDefaultAsync(p => EF.Functions.Collate(p.Nome, "NOCASE") == normalizado);

        return registro is null ? null : ParaDominio(registro);
    }

    public async Task<IReadOnlyList<Participante>> ListarPorNivelAsync(Nivel nivel)
    {
        var registros = await _db.Participantes.Where(p => p.Nivel == nivel).ToListAsync();
        return registros.Select(ParaDominio).ToList();
    }

    private static ParticipanteRegistro ParaRegistro(Participante p)
    {
        var registro = new ParticipanteRegistro
        {
            CriadoEm = p.CriadoEm,
            Tipo = p is Monitor ? "Monitor" : "Aluno",
            Nome = p.Nome,
            Nivel = p.Nivel,
            Xp = p.Xp,
            DiasSeguidos = p.DiasSeguidos,
            Apoios = string.Join(',', p.Acessibilidade.Apoios),
        };

        if (p is Monitor monitor)
        {
            registro.FormadoEm = monitor.FormadoEm;
            registro.SalasConduzidas = monitor.SalasConduzidas;
        }

        return registro;
    }

    /// <summary>
    /// Reconstrói a entidade só pelos métodos públicos (GanharXp, RegistrarPresenca...),
    /// os mesmos que qualquer código fora do domínio usaria. Nenhuma regra é reescrita aqui.
    /// </summary>
    private static Participante ParaDominio(ParticipanteRegistro r)
    {
        Participante participante = r.Tipo == "Monitor"
            ? new Monitor(r.Nome, r.FormadoEm ?? r.CriadoEm, r.Nivel)
            : new Aluno(r.Nome, r.Nivel);

        foreach (var apoio in r.Apoios.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            participante.Acessibilidade.Adicionar(Enum.Parse<TipoApoio>(apoio));
        }

        if (r.Xp > 0)
        {
            participante.GanharXp(r.Xp);
        }

        for (var i = 0; i < r.DiasSeguidos; i++)
        {
            participante.RegistrarPresenca();
        }

        if (participante is Monitor monitor)
        {
            for (var i = 0; i < r.SalasConduzidas; i++)
            {
                monitor.RegistrarSalaConduzida();
            }
        }

        participante.DefinirId(r.Id);
        participante.DefinirCriadoEm(r.CriadoEm);
        return participante;
    }

    private static void CopiarPara(Participante p, ParticipanteRegistro registro)
    {
        registro.Nome = p.Nome;
        registro.Nivel = p.Nivel;
        registro.Xp = p.Xp;
        registro.DiasSeguidos = p.DiasSeguidos;
        registro.Apoios = string.Join(',', p.Acessibilidade.Apoios);

        if (p is Monitor monitor)
        {
            registro.FormadoEm = monitor.FormadoEm;
            registro.SalasConduzidas = monitor.SalasConduzidas;
        }
    }
}
