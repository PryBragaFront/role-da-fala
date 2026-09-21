using RoleDaFala.Api.Dtos;

namespace RoleDaFala.Api.Servicos;

/// <summary>
/// Regras de aplicação sobre participantes. O controller só recebe a
/// requisição e devolve a resposta: quem decide é o serviço.
/// </summary>
public interface IParticipanteServico
{
    Task<ParticipanteResponse> CriarAsync(CriarParticipanteRequest requisicao);
    Task<ParticipanteResponse?> ObterAsync(int id);
    Task<IReadOnlyList<ParticipanteResponse>> ListarAsync();
    Task<ParticipanteResponse?> AtualizarApoiosAsync(int id, string[] apoios);
    Task<bool> RegistrarPresencaAsync(int id);
}
