using RoleDaFala.Dominio.Acessibilidade;
using RoleDaFala.Dominio.Atividades;
using RoleDaFala.Dominio.Avaliacao;
using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Excecoes;
using Xunit;

namespace RoleDaFala.Testes.Dominio;

public class AtividadeTestes
{
    private readonly IAvaliador _avaliador = new AvaliadorComSotaque(new AvaliadorPorSemelhanca());

    [Fact]
    public void Figura_sem_palavra_e_recusada()
    {
        Assert.Throws<DominioException>(() =>
            new AtividadeFigura("", "maçã", "épol", "dica"));
    }

    [Fact]
    public void Acerto_na_figura_da_xp_ao_aluno()
    {
        var aluno = new Aluno("Bia");
        var figura = new AtividadeFigura("apple", "maçã", "épol", "O \"a\" é aberto.");

        var resultado = figura.Responder(aluno, "apple", _avaliador);

        Assert.True(resultado.Acertou);
        Assert.Equal(5, aluno.Xp);
        Assert.Equal("O \"a\" é aberto.", resultado.Dica);
    }

    [Fact]
    public void Erro_na_figura_nao_da_xp()
    {
        var aluno = new Aluno("Bia");
        var figura = new AtividadeFigura("apple", "maçã", "épol", "dica");

        figura.Responder(aluno, "xyz", _avaliador);

        Assert.Equal(0, aluno.Xp);
    }

    [Fact]
    public void Monitor_acerta_mas_nao_acumula_xp()
    {
        var monitor = new Monitor("Davi", DateTime.UtcNow.AddYears(-1));
        var figura = new AtividadeFigura("apple", "maçã", "épol", "dica");

        var resultado = figura.Responder(monitor, "apple", _avaliador);

        Assert.True(resultado.Acertou);
        Assert.Equal(0, monitor.Xp);
    }

    [Fact]
    public void Conversa_aceita_mais_de_uma_resposta_certa()
    {
        var aluno = new Aluno("Bia", Nivel.Iniciante);
        var conversa = new AtividadeConversa("Cumprimentos", "How are you?",
            new[] { "I am fine", "I am good" });

        var resultado = conversa.Responder(aluno, "I am good", _avaliador);

        Assert.Equal(100, resultado.Nota);
    }

    [Fact]
    public void Atividade_acima_do_nivel_nao_esta_disponivel()
    {
        var aluno = new Aluno("Bia");
        var conversa = new AtividadeConversa("Debate", "What do you think?",
            new[] { "I think so" }, Nivel.Avancado);

        Assert.False(conversa.EstaDisponivelPara(aluno));
        Assert.Throws<DominioException>(() => conversa.Responder(aluno, "I think so", _avaliador));
    }

    [Fact]
    public void Escuta_nao_aparece_para_quem_e_surdo()
    {
        var aluno = new Aluno("Bia", Nivel.Iniciante);
        aluno.Acessibilidade.Adicionar(TipoApoio.Surdez);

        var escuta = new AtividadeEscuta("The bus arrives at eight");

        Assert.True(escuta.DependeDeAudio);
        Assert.False(escuta.EstaDisponivelPara(aluno));
    }

    [Fact]
    public void Figura_aparece_para_quem_e_surdo()
    {
        var aluno = new Aluno("Bia");
        aluno.Acessibilidade.Adicionar(TipoApoio.Surdez);

        var figura = new AtividadeFigura("cat", "gato", "két", "dica");

        Assert.True(figura.EstaDisponivelPara(aluno));
    }

    // POLIMORFISMO: uma lista de Atividade, três classes, o mesmo método.
    [Fact]
    public void Lista_polimorfica_responde_cada_uma_do_seu_jeito()
    {
        var aluno = new Aluno("Bia", Nivel.Intermediario);

        var atividades = new List<Atividade>
        {
            new AtividadeFigura("cat", "gato", "két", "dica"),
            new AtividadeConversa("Mercado", "How much?", new[] { "Two reais" }),
            new AtividadeEscuta("The bus arrives at eight")
        };

        var titulos = atividades.Select(a => a.Titulo).ToList();

        Assert.Contains("Figura: gato", titulos);
        Assert.Contains("Conversa: Mercado", titulos);
        Assert.Contains("Escuta", titulos);
        Assert.All(atividades, a => Assert.True(a.EstaDisponivelPara(aluno)));
    }
}
