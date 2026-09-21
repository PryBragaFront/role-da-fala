namespace RoleDaFala.Dominio.Excecoes;

/// <summary>
/// Erro de regra de negócio. Existe para separar o que é falha de uso
/// do sistema (nome vazio, nota inválida) do que é falha técnica.
/// </summary>
public class DominioException : Exception
{
    public DominioException(string mensagem) : base(mensagem) { }
}
