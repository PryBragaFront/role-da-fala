using RoleDaFala.Dominio.Excecoes;

namespace RoleDaFala.Dominio.Acessibilidade;

/// <summary>
/// Os apoios escolhidos por uma pessoa e o que eles mudam no app.
///
/// ENCAPSULAMENTO: a lista interna é privada. Quem usa a classe não consegue
/// adicionar um apoio direto na lista, tem que passar por Adicionar(), que
/// aplica as regras. A propriedade pública devolve uma cópia somente leitura.
/// </summary>
public class PerfilAcessibilidade
{
    private readonly HashSet<TipoApoio> _apoios = new();

    public IReadOnlyCollection<TipoApoio> Apoios => _apoios.ToList().AsReadOnly();

    public bool Possui(TipoApoio apoio) => _apoios.Contains(apoio);

    public void Adicionar(TipoApoio apoio)
    {
        if (apoio == TipoApoio.Nenhum)
        {
            _apoios.Clear();
            return;
        }

        // Regra: cegueira e baixa visão não convivem. Quem é cego usa o modo por voz,
        // que substitui os ajustes visuais.
        if (apoio == TipoApoio.Cegueira)
        {
            _apoios.Remove(TipoApoio.BaixaVisao);
        }

        if (apoio == TipoApoio.BaixaVisao && _apoios.Contains(TipoApoio.Cegueira))
        {
            throw new DominioException("Baixa visão não se aplica junto com cegueira.");
        }

        _apoios.Add(apoio);
    }

    public void Remover(TipoApoio apoio) => _apoios.Remove(apoio);

    /// <summary>O app lê as telas em voz alta.</summary>
    public bool PrecisaDeVoz() => Possui(TipoApoio.Cegueira);

    /// <summary>Nenhuma atividade pode depender só de áudio.</summary>
    public bool PrecisaDeAlternativaVisual() => Possui(TipoApoio.Surdez);

    /// <summary>Sessões curtas, uma tarefa por vez.</summary>
    public bool PrecisaDeSessaoCurta() => Possui(TipoApoio.Tdah);

    /// <summary>Classes de CSS que o front aplica.</summary>
    public IEnumerable<string> ClassesDeInterface()
    {
        if (Possui(TipoApoio.Cegueira) || Possui(TipoApoio.BaixaVisao)) yield return "big";
        if (Possui(TipoApoio.BaixaVisao)) yield return "hc";
        if (Possui(TipoApoio.Dislexia)) yield return "dys";
        if (Possui(TipoApoio.Autismo)) yield return "calm";
        if (Possui(TipoApoio.Tdah)) yield return "focus";
        if (Possui(TipoApoio.Surdez)) yield return "deaf";
    }
}
