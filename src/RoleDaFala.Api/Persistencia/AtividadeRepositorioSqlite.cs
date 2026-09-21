using Microsoft.EntityFrameworkCore;
using RoleDaFala.Dominio.Atividades;
using RoleDaFala.Dominio.Repositorios;

namespace RoleDaFala.Api.Persistencia;

/// <summary>
/// Mesma ideia de <see cref="ParticipanteRepositorioSqlite"/>: converte entre a
/// entidade de domínio (Figura, Conversa ou Escuta) e o registro achatado.
/// </summary>
public class AtividadeRepositorioSqlite : IRepositorio<Atividade>
{
    private readonly RoleDaFalaDbContext _db;

    public AtividadeRepositorioSqlite(RoleDaFalaDbContext db)
    {
        _db = db;
    }

    public async Task<Atividade?> ObterPorIdAsync(int id)
    {
        var registro = await _db.Atividades.FirstOrDefaultAsync(a => a.Id == id);
        return registro is null ? null : ParaDominio(registro);
    }

    public async Task<IReadOnlyList<Atividade>> ListarAsync()
    {
        var registros = await _db.Atividades.ToListAsync();
        return registros.Select(ParaDominio).ToList();
    }

    public async Task<Atividade> AdicionarAsync(Atividade entidade)
    {
        var registro = ParaRegistro(entidade);
        _db.Atividades.Add(registro);
        await _db.SaveChangesAsync();

        entidade.DefinirId(registro.Id);
        entidade.DefinirCriadoEm(registro.CriadoEm);
        return entidade;
    }

    public Task AtualizarAsync(Atividade entidade) =>
        // Nenhum campo de Atividade muda depois de criada (Enunciado, NivelMinimo
        // e os dados de cada tipo são todos só-leitura no domínio), então não há
        // o que sincronizar de volta ao banco.
        Task.CompletedTask;

    public async Task<bool> RemoverAsync(int id)
    {
        var registro = await _db.Atividades.FirstOrDefaultAsync(a => a.Id == id);

        if (registro is null)
        {
            return false;
        }

        _db.Atividades.Remove(registro);
        await _db.SaveChangesAsync();
        return true;
    }

    private static AtividadeRegistro ParaRegistro(Atividade a)
    {
        var registro = new AtividadeRegistro
        {
            CriadoEm = a.CriadoEm,
            NivelMinimo = a.NivelMinimo,
        };

        switch (a)
        {
            case AtividadeFigura figura:
                registro.Tipo = "Figura";
                registro.Palavra = figura.Palavra;
                registro.Traducao = figura.Traducao;
                registro.Pronuncia = figura.Pronuncia;
                registro.Dica = figura.Dica;
                break;

            case AtividadeConversa conversa:
                registro.Tipo = "Conversa";
                registro.Cenario = conversa.Cenario;
                registro.Fala = conversa.Enunciado;
                registro.RespostasAceitas = string.Join('|', conversa.RespostasAceitas);
                break;

            case AtividadeEscuta escuta:
                registro.Tipo = "Escuta";
                registro.FraseFalada = escuta.FraseFalada;
                break;

            default:
                throw new InvalidOperationException($"Tipo de atividade desconhecido: {a.GetType().Name}.");
        }

        return registro;
    }

    private static Atividade ParaDominio(AtividadeRegistro r)
    {
        Atividade atividade = r.Tipo switch
        {
            "Figura" => new AtividadeFigura(r.Palavra!, r.Traducao!, r.Pronuncia!, r.Dica!),
            "Conversa" => new AtividadeConversa(
                r.Cenario!,
                r.Fala!,
                (r.RespostasAceitas ?? string.Empty).Split('|', StringSplitOptions.RemoveEmptyEntries),
                r.NivelMinimo),
            "Escuta" => new AtividadeEscuta(r.FraseFalada!, r.NivelMinimo),
            _ => throw new InvalidOperationException($"Tipo de atividade desconhecido: {r.Tipo}."),
        };

        atividade.DefinirId(r.Id);
        atividade.DefinirCriadoEm(r.CriadoEm);
        return atividade;
    }
}
