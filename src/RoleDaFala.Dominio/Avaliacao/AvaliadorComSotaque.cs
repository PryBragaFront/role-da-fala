namespace RoleDaFala.Dominio.Avaliacao;

/// <summary>
/// Avaliador que perdoa erros típicos de quem fala português.
///
/// HERANÇA de comportamento por composição: em vez de copiar o cálculo,
/// recebe outro avaliador e só entra na frente quando reconhece um
/// "quase acerto" conhecido. É o padrão Decorator.
/// </summary>
public class AvaliadorComSotaque : IAvaliador
{
    private const int NotaDeQuaseAcerto = 80;

    private static readonly Dictionary<string, HashSet<string>> QuaseAcertos = new()
    {
        ["apple"] = new() { "apel", "epol", "apol", "aple" },
        ["ball"] = new() { "bol", "bou", "bal" },
        ["sun"] = new() { "san", "son", "sam" },
        ["house"] = new() { "raus", "haus", "rauz" },
        ["book"] = new() { "buk", "buque", "buc" },
        ["cup"] = new() { "cap", "kap", "capi" },
        ["tree"] = new() { "tri", "tchri" },
        ["bus"] = new() { "bas", "baz" },
        ["dog"] = new() { "dogui", "dag" },
        ["cat"] = new() { "ket", "quet" },
        ["fish"] = new() { "fich", "fixe", "fis" }
    };

    private readonly IAvaliador _interno;

    public AvaliadorComSotaque(IAvaliador interno)
    {
        _interno = interno;
    }

    public ResultadoAvaliacao Avaliar(string esperado, string recebido)
    {
        var a = (esperado ?? string.Empty).Trim().ToLowerInvariant();
        var b = (recebido ?? string.Empty).Trim().ToLowerInvariant();

        if (QuaseAcertos.TryGetValue(a, out var variacoes) && variacoes.Contains(b))
        {
            return ResultadoAvaliacao.APartirDaNota(NotaDeQuaseAcerto);
        }

        return _interno.Avaliar(esperado, recebido);
    }
}
