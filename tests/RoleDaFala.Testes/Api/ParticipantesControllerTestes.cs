using Microsoft.AspNetCore.Mvc;
using RoleDaFala.Api.Controllers;
using RoleDaFala.Api.Dtos;
using RoleDaFala.Api.Servicos;
using RoleDaFala.Dominio.Repositorios;
using Xunit;

namespace RoleDaFala.Testes.Api;

/// <summary>Testa se o controller devolve o código HTTP certo em cada caso.</summary>
public class ParticipantesControllerTestes
{
    private readonly ParticipantesController _controller =
        new(new ParticipanteServico(new ParticipanteRepositorioEmMemoria()));

    [Fact]
    public async Task Criar_valido_devolve_201()
    {
        var resposta = await _controller.Criar(new CriarParticipanteRequest("Bia", null, null, null));

        var criado = Assert.IsType<CreatedAtActionResult>(resposta);
        Assert.Equal(StatusCodes.Status201Created, criado.StatusCode);
    }

    [Fact]
    public async Task Criar_com_nome_invalido_devolve_400()
    {
        var resposta = await _controller.Criar(new CriarParticipanteRequest("", null, null, null));

        var erro = Assert.IsType<BadRequestObjectResult>(resposta);
        Assert.IsType<ErroResponse>(erro.Value);
    }

    [Fact]
    public async Task Obter_inexistente_devolve_404()
    {
        var resposta = await _controller.Obter(99);

        Assert.IsType<NotFoundResult>(resposta);
    }

    [Fact]
    public async Task Presenca_de_quem_existe_devolve_204()
    {
        var criacao = await _controller.Criar(new CriarParticipanteRequest("Bia", null, null, null));
        var criado = (ParticipanteResponse)((CreatedAtActionResult)criacao).Value!;

        var resposta = await _controller.RegistrarPresenca(criado.Id);

        Assert.IsType<NoContentResult>(resposta);
    }
}
