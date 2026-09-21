using Microsoft.AspNetCore.Mvc;
using RoleDaFala.Api.Dtos;
using RoleDaFala.Api.Servicos;
using RoleDaFala.Dominio.Excecoes;

namespace RoleDaFala.Api.Controllers;

[ApiController]
[Route("contas")]
public class ContasController : ControllerBase
{
    private readonly IContaServico _servico;

    public ContasController(IContaServico servico)
    {
        _servico = servico;
    }

    /// <summary>Cria o participante e a conta de login numa só chamada.</summary>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarContaRequest requisicao)
    {
        try
        {
            var resultado = await _servico.RegistrarAsync(requisicao);
            return CreatedAtAction(nameof(Registrar), resultado);
        }
        catch (DominioException ex)
        {
            return BadRequest(new ErroResponse(ex.Message));
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest requisicao)
    {
        try
        {
            var resultado = await _servico.LoginAsync(requisicao);
            return resultado is null
                ? Unauthorized(new ErroResponse("E-mail ou senha inválidos."))
                : Ok(resultado);
        }
        catch (DominioException ex)
        {
            return BadRequest(new ErroResponse(ex.Message));
        }
    }
}
