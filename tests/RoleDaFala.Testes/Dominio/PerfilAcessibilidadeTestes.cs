using RoleDaFala.Dominio.Acessibilidade;
using RoleDaFala.Dominio.Excecoes;
using Xunit;

namespace RoleDaFala.Testes.Dominio;

public class PerfilAcessibilidadeTestes
{
    [Fact]
    public void Perfil_novo_nao_tem_apoio()
    {
        var perfil = new PerfilAcessibilidade();

        Assert.Empty(perfil.Apoios);
    }

    [Fact]
    public void Apoio_repetido_nao_duplica()
    {
        var perfil = new PerfilAcessibilidade();

        perfil.Adicionar(TipoApoio.Dislexia);
        perfil.Adicionar(TipoApoio.Dislexia);

        Assert.Single(perfil.Apoios);
    }

    [Fact]
    public void Cegueira_substitui_baixa_visao()
    {
        var perfil = new PerfilAcessibilidade();

        perfil.Adicionar(TipoApoio.BaixaVisao);
        perfil.Adicionar(TipoApoio.Cegueira);

        Assert.True(perfil.Possui(TipoApoio.Cegueira));
        Assert.False(perfil.Possui(TipoApoio.BaixaVisao));
    }

    [Fact]
    public void Baixa_visao_depois_de_cegueira_e_recusada()
    {
        var perfil = new PerfilAcessibilidade();
        perfil.Adicionar(TipoApoio.Cegueira);

        Assert.Throws<DominioException>(() => perfil.Adicionar(TipoApoio.BaixaVisao));
    }

    [Fact]
    public void Nenhum_limpa_os_apoios()
    {
        var perfil = new PerfilAcessibilidade();
        perfil.Adicionar(TipoApoio.Tdah);

        perfil.Adicionar(TipoApoio.Nenhum);

        Assert.Empty(perfil.Apoios);
    }

    [Fact]
    public void Classes_de_interface_saem_conforme_os_apoios()
    {
        var perfil = new PerfilAcessibilidade();
        perfil.Adicionar(TipoApoio.Dislexia);
        perfil.Adicionar(TipoApoio.Tdah);

        var classes = perfil.ClassesDeInterface().ToList();

        Assert.Contains("dys", classes);
        Assert.Contains("focus", classes);
        Assert.DoesNotContain("deaf", classes);
    }

    // ENCAPSULAMENTO: a coleção devolvida é somente leitura.
    [Fact]
    public void Colecao_exposta_e_somente_leitura()
    {
        var perfil = new PerfilAcessibilidade();
        perfil.Adicionar(TipoApoio.Surdez);

        Assert.IsAssignableFrom<IReadOnlyCollection<TipoApoio>>(perfil.Apoios);
        Assert.True(perfil.PrecisaDeAlternativaVisual());
    }
}
