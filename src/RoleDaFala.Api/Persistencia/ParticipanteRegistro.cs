using RoleDaFala.Dominio.Entidades;

namespace RoleDaFala.Api.Persistencia;

/// <summary>
/// Formato salvo no SQLite. Não é a entidade de domínio: Participante encapsula
/// Xp, DiasSeguidos etc. de propósito (só muda pelos métodos que aplicam as
/// regras), então o registro guarda os dados "achatados" e o repositório
/// (<see cref="ParticipanteRepositorioSqlite"/>) sabe converter de um lado
/// para o outro sem que o domínio precise abrir mão do encapsulamento.
/// </summary>
public class ParticipanteRegistro
{
    public int Id { get; set; }
    public DateTime CriadoEm { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public Nivel Nivel { get; set; }
    public int Xp { get; set; }
    public int DiasSeguidos { get; set; }

    /// <summary>Apoios separados por vírgula (ex.: "Dislexia,Tdah").</summary>
    public string Apoios { get; set; } = string.Empty;

    /// <summary>Só preenchido quando Tipo = "Monitor".</summary>
    public DateTime? FormadoEm { get; set; }

    public int SalasConduzidas { get; set; }
}
