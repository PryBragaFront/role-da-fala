using RoleDaFala.Dominio.Avaliacao;
using RoleDaFala.Dominio.Excecoes;
using Xunit;

namespace RoleDaFala.Testes.Dominio;

public class AvaliadorTestes
{
    private readonly IAvaliador _avaliador = new AvaliadorPorSemelhanca();

    [Fact]
    public void Palavra_exata_vale_cem()
    {
        var resultado = _avaliador.Avaliar("apple", "apple");

        Assert.Equal(100, resultado.Nota);
        Assert.True(resultado.FoiOtimo);
    }

    [Fact]
    public void Maiusculas_e_espacos_nao_contam()
    {
        var resultado = _avaliador.Avaliar("apple", "  APPLE  ");

        Assert.Equal(100, resultado.Nota);
    }

    [Fact]
    public void Resposta_vazia_vale_zero()
    {
        var resultado = _avaliador.Avaliar("apple", "");

        Assert.Equal(0, resultado.Nota);
        Assert.False(resultado.Acertou);
    }

    [Fact]
    public void Palavra_muito_diferente_pontua_baixo()
    {
        var resultado = _avaliador.Avaliar("apple", "banana");

        Assert.True(resultado.Nota < 40);
    }

    [Fact]
    public void Nota_fora_da_faixa_e_recusada()
    {
        Assert.Throws<DominioException>(() => new ResultadoAvaliacao(101, "x"));
        Assert.Throws<DominioException>(() => new ResultadoAvaliacao(-1, "x"));
    }

    [Theory]
    [InlineData(100, "Muito bem!")]
    [InlineData(85, "Muito bem!")]
    [InlineData(70, "Quase lá. Tente de novo.")]
    [InlineData(10, "Vamos tentar de novo, com calma.")]
    public void Mensagem_acompanha_a_nota(int nota, string esperada)
    {
        var resultado = ResultadoAvaliacao.APartirDaNota(nota);

        Assert.Equal(esperada, resultado.Mensagem);
    }

    // O decorator entra na frente só nos casos que conhece.
    [Fact]
    public void Avaliador_com_sotaque_perdoa_erro_conhecido()
    {
        var comSotaque = new AvaliadorComSotaque(new AvaliadorPorSemelhanca());

        var resultado = comSotaque.Avaliar("apple", "apol");

        Assert.Equal(80, resultado.Nota);
        Assert.True(resultado.Acertou);
    }

    [Fact]
    public void Avaliador_com_sotaque_repassa_o_que_nao_conhece()
    {
        var comSotaque = new AvaliadorComSotaque(new AvaliadorPorSemelhanca());

        var doDecorator = comSotaque.Avaliar("apple", "banana");
        var doInterno = _avaliador.Avaliar("apple", "banana");

        Assert.Equal(doInterno.Nota, doDecorator.Nota);
    }
}
