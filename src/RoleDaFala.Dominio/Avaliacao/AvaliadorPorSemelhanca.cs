namespace RoleDaFala.Dominio.Avaliacao;

/// <summary>
/// Avaliador padrão: compara as duas palavras letra a letra, pela distância
/// de Levenshtein. Não é análise fonética; é uma aproximação que funciona
/// offline e sem custo, o que atende à prioridade de pouca internet.
/// </summary>
public class AvaliadorPorSemelhanca : IAvaliador
{
    public ResultadoAvaliacao Avaliar(string esperado, string recebido)
    {
        var a = Normalizar(esperado);
        var b = Normalizar(recebido);

        if (a.Length == 0 || b.Length == 0)
        {
            return ResultadoAvaliacao.APartirDaNota(0);
        }

        if (a == b)
        {
            return ResultadoAvaliacao.APartirDaNota(100);
        }

        var distancia = Distancia(a, b);
        var nota = (int)Math.Round(100.0 * (1.0 - (double)distancia / Math.Max(a.Length, b.Length)));

        return ResultadoAvaliacao.APartirDaNota(Math.Clamp(nota, 0, 100));
    }

    private static string Normalizar(string texto) =>
        (texto ?? string.Empty).Trim().ToLowerInvariant();

    /// <summary>Quantas letras é preciso trocar, inserir ou apagar para ir de a até b.</summary>
    private static int Distancia(string a, string b)
    {
        var anterior = new int[b.Length + 1];
        var atual = new int[b.Length + 1];

        for (var j = 0; j <= b.Length; j++)
        {
            anterior[j] = j;
        }

        for (var i = 1; i <= a.Length; i++)
        {
            atual[0] = i;

            for (var j = 1; j <= b.Length; j++)
            {
                var custo = a[i - 1] == b[j - 1] ? 0 : 1;
                atual[j] = Math.Min(Math.Min(atual[j - 1] + 1, anterior[j] + 1), anterior[j - 1] + custo);
            }

            (anterior, atual) = (atual, anterior);
        }

        return anterior[b.Length];
    }
}
