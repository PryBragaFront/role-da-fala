using System.Security.Cryptography;

namespace RoleDaFala.Dominio.Seguranca;

/// <summary>
/// Gera e confere o hash de uma senha com PBKDF2 (só a biblioteca padrão do .NET,
/// sem depender de nenhum pacote externo nem do ASP.NET).
///
/// A senha em si nunca é guardada: só o hash. "Gerar" cria um sal aleatório por
/// senha, para que duas pessoas com a mesma senha tenham hashes diferentes.
/// </summary>
public static class HashDeSenha
{
    private const int TamanhoSalEmBytes = 16;
    private const int TamanhoHashEmBytes = 32;
    private const int Iteracoes = 100_000;

    public static string Gerar(string senhaEmTexto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(senhaEmTexto);

        var sal = RandomNumberGenerator.GetBytes(TamanhoSalEmBytes);
        var hash = DerivarHash(senhaEmTexto, sal);

        return $"{Iteracoes}.{Convert.ToBase64String(sal)}.{Convert.ToBase64String(hash)}";
    }

    /// <summary>Confere se a senha digitada bate com o hash guardado, sem nunca descriptografar nada.</summary>
    public static bool Confere(string senhaEmTexto, string hashArmazenado)
    {
        var partes = hashArmazenado.Split('.', 3);

        if (partes.Length != 3 || !int.TryParse(partes[0], out var iteracoes))
        {
            return false;
        }

        var sal = Convert.FromBase64String(partes[1]);
        var hashEsperado = Convert.FromBase64String(partes[2]);
        var hashCalculado = DerivarHash(senhaEmTexto, sal, iteracoes);

        return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
    }

    private static byte[] DerivarHash(string senha, byte[] sal, int iteracoes = Iteracoes) =>
        Rfc2898DeriveBytes.Pbkdf2(senha, sal, iteracoes, HashAlgorithmName.SHA256, TamanhoHashEmBytes);
}
