namespace RoleDaFala.Dominio.Entidades;

/// <summary>
/// Classe base de tudo que o sistema guarda.
///
/// ABSTRAÇÃO: nenhuma entidade "Entidade" existe no mundo real; ela só reúne
/// o que é comum a todas. Por isso é abstract e não pode ser instanciada.
///
/// ENCAPSULAMENTO: Id e CriadoEm têm 'private set'. Quem usa a classe lê,
/// mas não altera: a data de criação não muda, e o Id é definido uma vez só.
/// </summary>
public abstract class Entidade
{
    public int Id { get; private set; }
    public DateTime CriadoEm { get; private set; }

    protected Entidade()
    {
        CriadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Só o repositório atribui o Id, depois de gravar.
    /// 'internal' limita o acesso ao próprio projeto.
    /// </summary>
    internal void DefinirId(int id)
    {
        if (Id != 0)
        {
            throw new Excecoes.DominioException("O Id já foi definido e não pode mudar.");
        }

        Id = id;
    }

    /// <summary>
    /// Duas entidades com o mesmo Id são a mesma coisa, mesmo em objetos diferentes.
    /// SOBRESCRITA: muda o comportamento herdado de Object.
    /// </summary>
    public override bool Equals(object? obj) =>
        obj is Entidade outra && GetType() == outra.GetType() && Id != 0 && Id == outra.Id;

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
