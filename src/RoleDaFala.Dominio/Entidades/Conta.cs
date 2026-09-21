using System.Text.RegularExpressions;
using RoleDaFala.Dominio.Excecoes;

namespace RoleDaFala.Dominio.Entidades;

/// <summary>
/// Credenciais de login de um participante: e-mail e hash de senha.
///
/// Fica separada de Participante de propósito: uma é "quem a pessoa é" no app
/// (nome, nível, apoios), a outra é "como ela entra" (e-mail, senha). Assim dá
/// para trocar a forma de login (ex.: entrar com Google) sem mexer no domínio
/// de aprendizagem, e vice-versa.
/// </summary>
public partial class Conta : Entidade
{
    private string _email = string.Empty;

    public Conta(string email, string senhaHash, int participanteId)
    {
        Email = email;
        SenhaHash = senhaHash;

        if (participanteId <= 0)
        {
            throw new DominioException("A conta precisa estar ligada a um participante existente.");
        }

        ParticipanteId = participanteId;
    }

    public string Email
    {
        get => _email;
        private set
        {
            var limpo = (value ?? string.Empty).Trim().ToLowerInvariant();

            if (!PadraoEmail().IsMatch(limpo))
            {
                throw new DominioException("O e-mail informado não é válido.");
            }

            _email = limpo;
        }
    }

    public string SenhaHash { get; private set; } = string.Empty;

    public int ParticipanteId { get; private set; }

    public void TrocarSenha(string novoHash) => SenhaHash = novoHash;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex PadraoEmail();
}
