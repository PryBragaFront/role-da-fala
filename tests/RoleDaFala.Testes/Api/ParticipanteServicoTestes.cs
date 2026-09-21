using RoleDaFala.Api.Dtos;
using RoleDaFala.Api.Servicos;
using RoleDaFala.Dominio.Repositorios;
using Xunit;

namespace RoleDaFala.Testes.Api;

/// <summary>
/// Testa a camada de serviço da API isoladamente, com o repositório em memória.
/// Não sobe servidor: é teste unitário, rápido.
/// </summary>
public class ParticipanteServicoTestes
{
    private readonly IParticipanteServico _servico =
        new ParticipanteServico(new ParticipanteRepositorioEmMemoria());

    [Fact]
    public async Task Criar_sem_tipo_gera_um_aluno()
    {
        var criado = await _servico.CriarAsync(new CriarParticipanteRequest("Bia", null, null, null));

        Assert.Equal("Aluno", criado.Tipo);
        Assert.Equal("Zero", criado.Nivel);
        Assert.False(criado.PodeCorrigirOutros());
    }

    [Fact]
    public async Task Criar_com_tipo_monitor_gera_um_monitor()
    {
        var criado = await _servico.CriarAsync(
            new CriarParticipanteRequest("Davi", "monitor", null, null));

        Assert.Equal("Monitor", criado.Tipo);
        Assert.True(criado.PodeCorrigirOutros());
    }

    [Fact]
    public async Task Criar_com_apoios_devolve_as_classes_de_interface()
    {
        var criado = await _servico.CriarAsync(
            new CriarParticipanteRequest("Bia", null, null, new[] { "Dislexia", "Tdah" }));

        Assert.Contains("dys", criado.ClassesDeInterface);
        Assert.Contains("focus", criado.ClassesDeInterface);
    }

    [Fact]
    public async Task Apoio_desconhecido_vira_erro_de_dominio()
    {
        await Assert.ThrowsAsync<RoleDaFala.Dominio.Excecoes.DominioException>(() =>
            _servico.CriarAsync(new CriarParticipanteRequest("Bia", null, null, new[] { "voar" })));
    }

    [Fact]
    public async Task Obter_id_inexistente_devolve_nulo()
    {
        Assert.Null(await _servico.ObterAsync(99));
    }

    [Fact]
    public async Task Atualizar_apoios_substitui_os_antigos()
    {
        var criado = await _servico.CriarAsync(
            new CriarParticipanteRequest("Bia", null, null, new[] { "Dislexia" }));

        var atualizado = await _servico.AtualizarApoiosAsync(criado.Id, new[] { "Surdez" });

        Assert.NotNull(atualizado);
        Assert.Contains("Surdez", atualizado!.Apoios);
        Assert.DoesNotContain("Dislexia", atualizado.Apoios);
    }

    [Fact]
    public async Task Registrar_presenca_soma_um_dia()
    {
        var criado = await _servico.CriarAsync(new CriarParticipanteRequest("Bia", null, null, null));

        await _servico.RegistrarPresencaAsync(criado.Id);
        var depois = await _servico.ObterAsync(criado.Id);

        Assert.Equal(1, depois!.DiasSeguidos);
    }

    [Fact]
    public async Task Registrar_presenca_de_quem_nao_existe_devolve_falso()
    {
        Assert.False(await _servico.RegistrarPresencaAsync(99));
    }
}
