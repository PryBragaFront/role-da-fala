using RoleDaFala.Dominio.Excecoes;

namespace RoleDaFala.Dominio.Entidades;

/// <summary>
/// Quem media as salas e responde pelas regras de convivência.
///
/// HERANÇA com acréscimo: além do que todo participante tem, o monitor
/// guarda a data da formação e a quantidade de salas que conduziu.
/// </summary>
public class Monitor : Participante
{
    public Monitor(string nome, DateTime formadoEm, Nivel nivel = Nivel.Intermediario)
        : base(nome, nivel)
    {
        if (formadoEm > DateTime.UtcNow)
        {
            throw new DominioException("A data de formação não pode estar no futuro.");
        }

        FormadoEm = formadoEm;
    }

    public DateTime FormadoEm { get; }
    public int SalasConduzidas { get; private set; }

    /// <summary>O monitor sempre pode corrigir: é para isso que ele foi formado.</summary>
    public override bool PodeCorrigirOutros() => true;

    /// <summary>O monitor não acumula XP: ele não está competindo com os alunos.</summary>
    public override int XpPorAcerto() => 0;

    public override string Descrever() => $"Monitor {Nome}, formado em {FormadoEm:dd/MM/yyyy}";

    public void RegistrarSalaConduzida() => SalasConduzidas++;
}
