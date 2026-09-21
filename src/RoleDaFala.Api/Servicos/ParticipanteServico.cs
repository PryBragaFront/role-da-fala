using RoleDaFala.Api.Dtos;
using RoleDaFala.Dominio.Acessibilidade;
using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Excecoes;
using RoleDaFala.Dominio.Repositorios;

namespace RoleDaFala.Api.Servicos;

public class ParticipanteServico : IParticipanteServico
{
    private readonly IParticipanteRepositorio _repositorio;

    /// <summary>Recebe a interface pelo construtor: é a injeção de dependência.</summary>
    public ParticipanteServico(IParticipanteRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ParticipanteResponse> CriarAsync(CriarParticipanteRequest requisicao)
    {
        var nivel = ConverterNivel(requisicao.Nivel);

        // POLIMORFISMO na prática: a variável é do tipo da mãe,
        // o objeto é de uma das filhas, e o resto do código não muda.
        Participante participante = requisicao.Tipo?.ToLowerInvariant() switch
        {
            "monitor" => new Monitor(requisicao.Nome, DateTime.UtcNow.Date, nivel),
            _ => new Aluno(requisicao.Nome, nivel)
        };

        foreach (var apoio in ConverterApoios(requisicao.Apoios))
        {
            participante.Acessibilidade.Adicionar(apoio);
        }

        await _repositorio.AdicionarAsync(participante);

        return Mapear(participante);
    }

    public async Task<ParticipanteResponse?> ObterAsync(int id)
    {
        var participante = await _repositorio.ObterPorIdAsync(id);
        return participante is null ? null : Mapear(participante);
    }

    public async Task<IReadOnlyList<ParticipanteResponse>> ListarAsync()
    {
        var todos = await _repositorio.ListarAsync();
        return todos.Select(Mapear).ToList();
    }

    public async Task<ParticipanteResponse?> AtualizarApoiosAsync(int id, string[] apoios)
    {
        var participante = await _repositorio.ObterPorIdAsync(id);

        if (participante is null)
        {
            return null;
        }

        participante.Acessibilidade.Adicionar(TipoApoio.Nenhum);

        foreach (var apoio in ConverterApoios(apoios))
        {
            participante.Acessibilidade.Adicionar(apoio);
        }

        await _repositorio.AtualizarAsync(participante);

        return Mapear(participante);
    }

    public async Task<bool> RegistrarPresencaAsync(int id)
    {
        var participante = await _repositorio.ObterPorIdAsync(id);

        if (participante is null)
        {
            return false;
        }

        participante.RegistrarPresenca();
        await _repositorio.AtualizarAsync(participante);

        return true;
    }

    private static Nivel ConverterNivel(string? texto) =>
        Enum.TryParse<Nivel>(texto, ignoreCase: true, out var nivel) ? nivel : Nivel.Zero;

    private static IEnumerable<TipoApoio> ConverterApoios(string[]? apoios)
    {
        foreach (var texto in apoios ?? Array.Empty<string>())
        {
            if (!Enum.TryParse<TipoApoio>(texto, ignoreCase: true, out var apoio))
            {
                throw new DominioException($"Apoio desconhecido: {texto}.");
            }

            yield return apoio;
        }
    }

    /// <summary>
    /// Converte a entidade em DTO. Repare que Descrever() e PodeCorrigirOutros()
    /// respondem diferente para Aluno e Monitor sem nenhum 'if' aqui.
    /// </summary>
    private static ParticipanteResponse Mapear(Participante p) => new(
        p.Id,
        p.Nome,
        p.GetType().Name,
        p.Nivel.ToString(),
        p.Xp,
        p.DiasSeguidos,
        p.Acessibilidade.Apoios.Select(a => a.ToString()).ToArray(),
        p.Acessibilidade.ClassesDeInterface().ToArray(),
        p.PodeCorrigirOutros(),
        p.Descrever());
}
