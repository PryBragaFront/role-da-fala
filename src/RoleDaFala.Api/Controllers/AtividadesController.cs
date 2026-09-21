using Microsoft.AspNetCore.Mvc;
using RoleDaFala.Api.Dtos;
using RoleDaFala.Api.Servicos;
using RoleDaFala.Dominio.Excecoes;

namespace RoleDaFala.Api.Controllers;

[ApiController]
[Route("atividades")]
public class AtividadesController : ControllerBase
{
    private readonly IAtividadeServico _servico;

    public AtividadesController(IAtividadeServico servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int? participanteId)
    {
        var lista = participanteId is null
            ? await _servico.ListarAsync()
            : await _servico.ListarDisponiveisParaAsync(participanteId.Value);

        return Ok(lista);
    }

    /// <summary>Envia a resposta de uma atividade e recebe a nota.</summary>
    [HttpPost("{id:int}/responder")]
    [ProducesResponseType(typeof(ResultadoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Responder(int id, [FromBody] ResponderRequest requisicao)
    {
        try
        {
            var resultado = await _servico.ResponderAsync(id, requisicao);
            return resultado is null ? NotFound() : Ok(resultado);
        }
        catch (DominioException ex)
        {
            return BadRequest(new ErroResponse(ex.Message));
        }
    }
}
