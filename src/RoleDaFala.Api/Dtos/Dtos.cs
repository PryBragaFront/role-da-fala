namespace RoleDaFala.Api.Dtos;

/// <summary>
/// Os DTOs separam o que trafega na API do que existe no domínio.
/// Assim a entidade pode mudar sem quebrar o contrato do front, e dados
/// internos não vazam para fora.
/// </summary>
public record CriarParticipanteRequest(string Nome, string? Tipo, string? Nivel, string[]? Apoios);

public record ParticipanteResponse(
    int Id,
    string Nome,
    string Tipo,
    string Nivel,
    int Xp,
    int DiasSeguidos,
    string[] Apoios,
    string[] ClassesDeInterface,
    bool PodeCorrigirOutros,
    string Descricao);

public record AtividadeResponse(int Id, string Titulo, string Enunciado, string NivelMinimo, bool DependeDeAudio);

public record ResponderRequest(int ParticipanteId, string Resposta);

public record ResultadoResponse(int Nota, string Mensagem, string Dica, bool Acertou, int XpAtual, string Nivel);

public record ErroResponse(string Erro);

public record RegistrarContaRequest(
    string Nome,
    string? Tipo,
    string? Nivel,
    string[]? Apoios,
    string Email,
    string Senha);

public record LoginRequest(string Email, string Senha);

public record LoginResponse(string Token, ParticipanteResponse Participante);
