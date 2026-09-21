namespace RoleDaFala.Dominio.Entidades;

/// <summary>
/// Quem está aprendendo. É o participante mais comum.
///
/// POLIMORFISMO: implementa PodeCorrigirOutros() à sua maneira.
/// </summary>
public class Aluno : Participante
{
    public Aluno(string nome, Nivel nivel = Nivel.Zero) : base(nome, nivel) { }

    /// <summary>
    /// Regra do grupo: aluno só corrige colega a partir do nível intermediário.
    /// Antes disso, corrigir sem saber atrapalha mais do que ajuda.
    /// </summary>
    public override bool PodeCorrigirOutros() => Nivel >= Nivel.Intermediario;

    public override string Descrever() => $"Aluno {base.Descrever()}";
}
