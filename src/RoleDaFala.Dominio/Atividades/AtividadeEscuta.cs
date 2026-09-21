using RoleDaFala.Dominio.Avaliacao;
using RoleDaFala.Dominio.Entidades;

namespace RoleDaFala.Dominio.Atividades;

/// <summary>
/// A pessoa ouve uma frase e escreve o que entendeu.
/// É a única atividade que depende mesmo de áudio, e por isso
/// não aparece para quem marcou surdez no cadastro.
/// </summary>
public class AtividadeEscuta : Atividade
{
    public AtividadeEscuta(string fraseFalada, Nivel nivelMinimo = Nivel.Iniciante)
        : base("Ouça e escreva o que você entendeu", nivelMinimo)
    {
        FraseFalada = fraseFalada;
    }

    public string FraseFalada { get; }

    public override string Titulo => "Escuta";

    public override bool DependeDeAudio => true;

    protected override ResultadoAvaliacao ExecutarAvaliacao(string resposta, IAvaliador avaliador) =>
        avaliador.Avaliar(FraseFalada, resposta);
}
