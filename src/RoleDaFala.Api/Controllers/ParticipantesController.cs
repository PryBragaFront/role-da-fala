using Microsoft.AspNetCore.Mvc;
using RoleDaFala.Api.Dtos;
using RoleDaFala.Api.Servicos;
using RoleDaFala.Dominio.Excecoes;

namespace RoleDaFala.Api.Controllers;

[ApiController]
[Route("participantes")]
public class ParticipantesController : ControllerBase
{
    private readonly IParticipanteServico _servico;

    public ParticipantesController(IParticipanteServico servico)
    {
        _servico = servico;
    }

    /// <summary>Cria um aluno ou um monitor.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ParticipanteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarParticipanteRequest requisicao)
    {
        try
        {
            var criado = await _servico.CriarAsync(requisicao);
            return CreatedAtAction(nameof(Obter), new { id = criado.Id }, criado);
        }
        catch (DominioException ex)
        {
            // A exceção do domínio vira 400. Erro de regra não é erro de servidor.
            return BadRequest(new ErroResponse(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ParticipanteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(int id)
    {
        var participante = await _servico.ObterAsync(id);
        return participante is null ? NotFound() : Ok(participante);
    }

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servico.ListarAsync());

    [HttpPut("{id:int}/apoios")]
    public async Task<IActionResult> AtualizarApoios(int id, [FromBody] string[] apoios)
    {
        try
        {
            var atualizado = await _servico.AtualizarApoiosAsync(id, apoios);
            return atualizado is null ? NotFound() : Ok(atualizado);
        }
        catch (DominioException ex)
        {
            return BadRequest(new ErroResponse(ex.Message));
        }
    }

    [HttpPost("{id:int}/presenca")]
    public async Task<IActionResult> RegistrarPresenca(int id) =>
        await _servico.RegistrarPresencaAsync(id) ? NoContent() : NotFound();
}
