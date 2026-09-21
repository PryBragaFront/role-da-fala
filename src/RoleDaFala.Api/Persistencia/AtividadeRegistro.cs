using RoleDaFala.Dominio.Entidades;

namespace RoleDaFala.Api.Persistencia;

/// <summary>
/// Formato salvo no SQLite para as três atividades (Figura, Conversa, Escuta).
/// Uma tabela só, com colunas específicas de cada tipo em branco quando não
/// se aplicam — simples e suficiente para o tamanho atual do domínio.
/// </summary>
public class AtividadeRegistro
{
    public int Id { get; set; }
    public DateTime CriadoEm { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public Nivel NivelMinimo { get; set; }

    // AtividadeFigura
    public string? Palavra { get; set; }
    public string? Traducao { get; set; }
    public string? Pronuncia { get; set; }
    public string? Dica { get; set; }

    // AtividadeConversa
    public string? Cenario { get; set; }
    public string? Fala { get; set; }

    /// <summary>Respostas aceitas, separadas por "|".</summary>
    public string? RespostasAceitas { get; set; }

    // AtividadeEscuta
    public string? FraseFalada { get; set; }
}
