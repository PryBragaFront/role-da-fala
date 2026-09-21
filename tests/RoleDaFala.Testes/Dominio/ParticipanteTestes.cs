using RoleDaFala.Dominio.Acessibilidade;
using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Excecoes;
using Xunit;

namespace RoleDaFala.Testes.Dominio;

public class ParticipanteTestes
{
    [Fact]
    public void Aluno_novo_comeca_no_nivel_zero_sem_xp()
    {
        var aluno = new Aluno("Bia");

        Assert.Equal(Nivel.Zero, aluno.Nivel);
        Assert.Equal(0, aluno.Xp);
        Assert.Equal(0, aluno.DiasSeguidos);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void Nome_invalido_e_recusado(string nome)
    {
        Assert.Throws<DominioException>(() => new Aluno(nome));
    }

    [Fact]
    public void Nome_e_gravado_sem_espacos_nas_pontas()
    {
        var aluno = new Aluno("  Bia  ");

        Assert.Equal("Bia", aluno.Nome);
    }

    [Fact]
    public void Xp_negativo_e_recusado()
    {
        var aluno = new Aluno("Bia");

        Assert.Throws<DominioException>(() => aluno.GanharXp(-1));
    }

    [Fact]
    public void Aluno_sobe_de_nivel_ao_acumular_xp()
    {
        var aluno = new Aluno("Bia");

        aluno.GanharXp(60);

        Assert.Equal(Nivel.Iniciante, aluno.Nivel);
    }

    [Fact]
    public void Nivel_nunca_desce()
    {
        var aluno = new Aluno("Bia", Nivel.Avancado);

        aluno.GanharXp(5);

        Assert.Equal(Nivel.Avancado, aluno.Nivel);
    }

    // POLIMORFISMO: o mesmo método responde diferente em cada filha.
    [Fact]
    public void Aluno_iniciante_nao_corrige_colega()
    {
        var aluno = new Aluno("Bia", Nivel.Iniciante);

        Assert.False(aluno.PodeCorrigirOutros());
    }

    [Fact]
    public void Aluno_intermediario_ja_corrige_colega()
    {
        var aluno = new Aluno("Bia", Nivel.Intermediario);

        Assert.True(aluno.PodeCorrigirOutros());
    }

    [Fact]
    public void Monitor_sempre_corrige_e_nao_acumula_xp()
    {
        var monitor = new Monitor("Davi", DateTime.UtcNow.AddMonths(-6));

        Assert.True(monitor.PodeCorrigirOutros());
        Assert.Equal(0, monitor.XpPorAcerto());
    }

    [Fact]
    public void Monitor_formado_no_futuro_e_recusado()
    {
        Assert.Throws<DominioException>(() =>
            new Monitor("Davi", DateTime.UtcNow.AddDays(1)));
    }

    // A lista é do tipo da mãe e guarda objetos das duas filhas.
    [Fact]
    public void Descrever_responde_conforme_o_tipo_real()
    {
        var participantes = new List<Participante>
        {
            new Aluno("Bia"),
            new Monitor("Davi", DateTime.UtcNow.AddYears(-1))
        };

        var descricoes = participantes.Select(p => p.Descrever()).ToList();

        Assert.StartsWith("Aluno", descricoes[0]);
        Assert.StartsWith("Monitor", descricoes[1]);
    }

    [Fact]
    public void Presenca_soma_e_reinicio_zera_a_sequencia()
    {
        var aluno = new Aluno("Bia");

        aluno.RegistrarPresenca();
        aluno.RegistrarPresenca();
        Assert.Equal(2, aluno.DiasSeguidos);

        aluno.ReiniciarSequencia();
        Assert.Equal(0, aluno.DiasSeguidos);
    }
}
