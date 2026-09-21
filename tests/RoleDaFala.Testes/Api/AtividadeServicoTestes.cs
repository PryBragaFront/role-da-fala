using RoleDaFala.Api.Dtos;
using RoleDaFala.Api.Servicos;
using RoleDaFala.Dominio.Atividades;
using RoleDaFala.Dominio.Avaliacao;
using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Repositorios;
using Xunit;

namespace RoleDaFala.Testes.Api;

public class AtividadeServicoTestes
{
    private readonly IRepositorio<Atividade> _atividades = new RepositorioEmMemoria<Atividade>();
    private readonly IParticipanteRepositorio _participantes = new ParticipanteRepositorioEmMemoria();
    private readonly IAtividadeServico _servico;

    public AtividadeServicoTestes()
    {
        _servico = new AtividadeServico(
            _atividades,
            _participantes,
            new AvaliadorComSotaque(new AvaliadorPorSemelhanca()));
    }

    [Fact]
    public async Task Responder_com_acerto_devolve_nota_e_xp_atualizado()
    {
        var aluno = await _participantes.AdicionarAsync(new Aluno("Bia"));
        var figura = await _atividades.AdicionarAsync(
            new AtividadeFigura("apple", "maçã", "épol", "O \"a\" é aberto."));

        var resultado = await _servico.ResponderAsync(figura.Id,
            new ResponderRequest(aluno.Id, "apple"));

        Assert.NotNull(resultado);
        Assert.Equal(100, resultado!.Nota);
        Assert.True(resultado.Acertou);
        Assert.Equal(5, resultado.XpAtual);
    }

    [Fact]
    public async Task Responder_atividade_inexistente_devolve_nulo()
    {
        var aluno = await _participantes.AdicionarAsync(new Aluno("Bia"));

        var resultado = await _servico.ResponderAsync(99, new ResponderRequest(aluno.Id, "apple"));

        Assert.Null(resultado);
    }

    [Fact]
    public async Task Responder_participante_inexistente_devolve_nulo()
    {
        var figura = await _atividades.AdicionarAsync(
            new AtividadeFigura("apple", "maçã", "épol", "dica"));

        var resultado = await _servico.ResponderAsync(figura.Id, new ResponderRequest(99, "apple"));

        Assert.Null(resultado);
    }

    [Fact]
    public async Task Listar_disponiveis_esconde_escuta_de_quem_e_surdo()
    {
        var aluno = new Aluno("Bia", Nivel.Iniciante);
        aluno.Acessibilidade.Adicionar(RoleDaFala.Dominio.Acessibilidade.TipoApoio.Surdez);
        await _participantes.AdicionarAsync(aluno);

        await _atividades.AdicionarAsync(new AtividadeFigura("cat", "gato", "két", "dica"));
        await _atividades.AdicionarAsync(new AtividadeEscuta("The bus arrives at eight"));

        var disponiveis = await _servico.ListarDisponiveisParaAsync(aluno.Id);

        Assert.Single(disponiveis);
        Assert.Equal("Figura: gato", disponiveis[0].Titulo);
    }

    [Fact]
    public async Task Listar_disponiveis_de_quem_nao_existe_vem_vazio()
    {
        var disponiveis = await _servico.ListarDisponiveisParaAsync(99);

        Assert.Empty(disponiveis);
    }
}
