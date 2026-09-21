using RoleDaFala.Dominio.Excecoes;

namespace RoleDaFala.Dominio.Avaliacao;

/// <summary>
/// O retorno de uma tentativa: nota, mensagem e dica.
///
/// Objeto de valor: não tem Id e é imutável. Dois resultados com os mesmos
/// dados são equivalentes. Por isso é 'record' e as propriedades são 'init'.
/// </summary>
public record ResultadoAvaliacao
{
    public const int NotaMinimaDeAcerto = 60;
    public const int NotaOtima = 85;

    public ResultadoAvaliacao(int nota, string mensagem, string dica = "")
    {
        if (nota is < 0 or > 100)
        {
            throw new DominioException("A nota precisa estar entre 0 e 100.");
        }

        Nota = nota;
        Mensagem = mensagem;
        Dica = dica;
    }

    public int Nota { get; init; }
    public string Mensagem { get; init; }
    public string Dica { get; init; }

    public bool Acertou => Nota >= NotaMinimaDeAcerto;
    public bool FoiOtimo => Nota >= NotaOtima;

    /// <summary>
    /// Mensagem no tom das regras do grupo: corrige sem humilhar.
    /// Método estático de fábrica: cria o objeto já com a mensagem certa.
    /// </summary>
    public static ResultadoAvaliacao APartirDaNota(int nota, string dica = "")
    {
        var mensagem = nota switch
        {
            >= NotaOtima => "Muito bem!",
            >= NotaMinimaDeAcerto => "Quase lá. Tente de novo.",
            _ => "Vamos tentar de novo, com calma."
        };

        return new ResultadoAvaliacao(nota, mensagem, dica);
    }
}
