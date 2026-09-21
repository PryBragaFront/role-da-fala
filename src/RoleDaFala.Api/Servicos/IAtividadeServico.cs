using RoleDaFala.Api.Dtos;

namespace RoleDaFala.Api.Servicos;

public interface IAtividadeServico
{
    Task<IReadOnlyList<AtividadeResponse>> ListarAsync();
    Task<IReadOnlyList<AtividadeResponse>> ListarDisponiveisParaAsync(int participanteId);
    Task<ResultadoResponse?> ResponderAsync(int atividadeId, ResponderRequest requisicao);
}
