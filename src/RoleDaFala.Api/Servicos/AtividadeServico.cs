using RoleDaFala.Api.Dtos;
using RoleDaFala.Dominio.Atividades;
using RoleDaFala.Dominio.Avaliacao;
using RoleDaFala.Dominio.Repositorios;

namespace RoleDaFala.Api.Servicos;

public class AtividadeServico : IAtividadeServico
{
    private readonly IRepositorio<Atividade> _atividades;
    private readonly IParticipanteRepositorio _participantes;
    private readonly IAvaliador _avaliador;

    public AtividadeServico(
        IRepositorio<Atividade> atividades,
        IParticipanteRepositorio participantes,
        IAvaliador avaliador)
    {
        _atividades = atividades;
        _participantes = participantes;
        _avaliador = avaliador;
    }

    public async Task<IReadOnlyList<AtividadeResponse>> ListarAsync()
    {
        var todas = await _atividades.ListarAsync();
        return todas.Select(Mapear).ToList();
    }

    /// <summary>
    /// Filtra pelo que a pessoa pode fazer: nível alcançado e sem depender
    /// de áudio para quem é surdo. A regra está na entidade, não aqui.
    /// </summary>
    public async Task<IReadOnlyList<AtividadeResponse>> ListarDisponiveisParaAsync(int participanteId)
    {
        var participante = await _participantes.ObterPorIdAsync(participanteId);

        if (participante is null)
        {
            return Array.Empty<AtividadeResponse>();
        }

        var todas = await _atividades.ListarAsync();

        return todas
            .Where(a => a.EstaDisponivelPara(participante))
            .Select(Mapear)
            .ToList();
    }

    public async Task<ResultadoResponse?> ResponderAsync(int atividadeId, ResponderRequest requisicao)
    {
        var atividade = await _atividades.ObterPorIdAsync(atividadeId);
        var participante = await _participantes.ObterPorIdAsync(requisicao.ParticipanteId);

        if (atividade is null || participante is null)
        {
            return null;
        }

        // Uma linha só, e ela serve para figura, conversa e escuta.
        var resultado = atividade.Responder(participante, requisicao.Resposta, _avaliador);

        await _participantes.AtualizarAsync(participante);

        return new ResultadoResponse(
            resultado.Nota,
            resultado.Mensagem,
            resultado.Dica,
            resultado.Acertou,
            participante.Xp,
            participante.Nivel.ToString());
    }

    private static AtividadeResponse Mapear(Atividade a) =>
        new(a.Id, a.Titulo, a.Enunciado, a.NivelMinimo.ToString(), a.DependeDeAudio);
}
