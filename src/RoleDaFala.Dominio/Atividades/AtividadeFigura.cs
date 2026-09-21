using RoleDaFala.Dominio.Avaliacao;
using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Excecoes;

namespace RoleDaFala.Dominio.Atividades;

/// <summary>
/// Mostra uma imagem, a pessoa fala o nome em inglês.
/// É a atividade do nível 0: não exige ler nada.
/// </summary>
public class AtividadeFigura : Atividade
{
    public AtividadeFigura(string palavra, string traducao, string pronuncia, string dica)
        : base("Diga o nome da figura em inglês", Nivel.Zero)
    {
        if (string.IsNullOrWhiteSpace(palavra))
        {
            throw new DominioException("A figura precisa de uma palavra em inglês.");
        }

        Palavra = palavra.Trim().ToLowerInvariant();
        Traducao = traducao;
        Pronuncia = pronuncia;
        Dica = dica;
    }

    public string Palavra { get; }
    public string Traducao { get; }
    public string Pronuncia { get; }
    public string Dica { get; }

    public override string Titulo => $"Figura: {Traducao}";

    /// <summary>
    /// Tem alternativa visual: quem é surdo vê a escrita e a pronúncia aproximada.
    /// Por isso não depende de áudio.
    /// </summary>
    public override bool DependeDeAudio => false;

    protected override ResultadoAvaliacao ExecutarAvaliacao(string resposta, IAvaliador avaliador)
    {
        var resultado = avaliador.Avaliar(Palavra, resposta);
        return resultado with { Dica = Dica };
    }
}
