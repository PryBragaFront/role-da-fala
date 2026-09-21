using RoleDaFala.Dominio.Avaliacao;
using RoleDaFala.Dominio.Entidades;

namespace RoleDaFala.Dominio.Atividades;

/// <summary>
/// Conversa guiada por cenário: mercado, consulta médica, entrevista.
/// Aceita mais de uma resposta certa, porque conversa não tem gabarito único.
/// </summary>
public class AtividadeConversa : Atividade
{
    private readonly List<string> _respostasAceitas;

    public AtividadeConversa(string cenario, string fala, IEnumerable<string> respostasAceitas,
        Nivel nivelMinimo = Nivel.Iniciante)
        : base(fala, nivelMinimo)
    {
        Cenario = cenario;
        _respostasAceitas = respostasAceitas?.ToList() ?? new List<string>();
    }

    public string Cenario { get; }
    public IReadOnlyCollection<string> RespostasAceitas => _respostasAceitas.AsReadOnly();

    public override string Titulo => $"Conversa: {Cenario}";

    public override bool DependeDeAudio => false;

    /// <summary>
    /// Avalia contra todas as respostas aceitas e fica com a melhor nota.
    /// SOBRESCRITA: mesmo contrato da mãe, cálculo diferente.
    /// </summary>
    protected override ResultadoAvaliacao ExecutarAvaliacao(string resposta, IAvaliador avaliador)
    {
        if (_respostasAceitas.Count == 0)
        {
            return ResultadoAvaliacao.APartirDaNota(0);
        }

        var melhor = _respostasAceitas
            .Select(aceita => avaliador.Avaliar(aceita, resposta))
            .MaxBy(r => r.Nota)!;

        return melhor;
    }
}
