using RoleDaFala.Dominio.Acessibilidade;
using RoleDaFala.Dominio.Excecoes;

namespace RoleDaFala.Dominio.Entidades;

/// <summary>
/// Pessoa que participa do Rolê da Fala.
///
/// HERANÇA: Aluno e Monitor herdam daqui. O que é comum a todos fica nesta classe.
/// POLIMORFISMO: PodeCorrigirOutros() e Descrever() são virtuais; cada filha responde
/// de um jeito, e quem chama não precisa saber qual é qual.
/// ENCAPSULAMENTO: Xp e DiasSeguidos só mudam pelos métodos, que aplicam as regras.
/// </summary>
public abstract class Participante : Entidade
{
    private string _nome = string.Empty;

    protected Participante(string nome, Nivel nivel = Nivel.Zero)
    {
        Nome = nome;
        Nivel = nivel;
        Acessibilidade = new PerfilAcessibilidade();
    }

    /// <summary>
    /// A validação mora no 'set'. Não existe Participante com nome vazio,
    /// nem na criação nem depois.
    /// </summary>
    public string Nome
    {
        get => _nome;
        set
        {
            var limpo = (value ?? string.Empty).Trim();

            if (limpo.Length < 2)
            {
                throw new DominioException("O nome precisa ter ao menos 2 letras.");
            }

            if (limpo.Length > 60)
            {
                throw new DominioException("O nome pode ter no máximo 60 letras.");
            }

            _nome = limpo;
        }
    }

    public Nivel Nivel { get; private set; }
    public int Xp { get; private set; }
    public int DiasSeguidos { get; private set; }
    public PerfilAcessibilidade Acessibilidade { get; }

    /// <summary>Quem pode corrigir o texto de outra pessoa. Cada tipo responde diferente.</summary>
    public abstract bool PodeCorrigirOutros();

    /// <summary>Quanto XP o participante ganha por acerto. O monitor não acumula.</summary>
    public virtual int XpPorAcerto() => 5;

    public virtual string Descrever() => $"{Nome}, nível {Nivel}";

    public void GanharXp(int quantidade)
    {
        if (quantidade < 0)
        {
            throw new DominioException("Não é possível ganhar XP negativo.");
        }

        Xp += quantidade;
        AtualizarNivel();
    }

    public void RegistrarPresenca() => DiasSeguidos++;

    public void ReiniciarSequencia() => DiasSeguidos = 0;

    /// <summary>Sobe de nível conforme o XP. Nunca desce: aprendizado não é apagado.</summary>
    private void AtualizarNivel()
    {
        var novo = Xp switch
        {
            >= 600 => Nivel.Avancado,
            >= 250 => Nivel.Intermediario,
            >= 60 => Nivel.Iniciante,
            _ => Nivel.Zero
        };

        if (novo > Nivel)
        {
            Nivel = novo;
        }
    }
}
