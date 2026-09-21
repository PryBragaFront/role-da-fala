using RoleDaFala.Api.Dtos;

namespace RoleDaFala.Api.Servicos;

/// <summary>Cadastro (com conta de login) e autenticação de participantes.</summary>
public interface IContaServico
{
    Task<LoginResponse> RegistrarAsync(RegistrarContaRequest requisicao);
    Task<LoginResponse?> LoginAsync(LoginRequest requisicao);
}
