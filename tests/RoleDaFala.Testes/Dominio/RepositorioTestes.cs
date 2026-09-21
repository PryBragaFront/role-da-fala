using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Repositorios;
using Xunit;

namespace RoleDaFala.Testes.Dominio;

public class RepositorioTestes
{
    private readonly IParticipanteRepositorio _repositorio = new ParticipanteRepositorioEmMemoria();

    [Fact]
    public async Task Adicionar_atribui_id_sequencial()
    {
        var primeiro = await _repositorio.AdicionarAsync(new Aluno("Bia"));
        var segundo = await _repositorio.AdicionarAsync(new Aluno("Davi"));

        Assert.Equal(1, primeiro.Id);
        Assert.Equal(2, segundo.Id);
    }

    [Fact]
    public async Task Buscar_por_nome_ignora_maiusculas()
    {
        await _repositorio.AdicionarAsync(new Aluno("Glendha"));

        var encontrado = await _repositorio.ObterPorNomeAsync("glendha");

        Assert.NotNull(encontrado);
    }

    [Fact]
    public async Task Buscar_por_nivel_filtra_certo()
    {
        await _repositorio.AdicionarAsync(new Aluno("Bia"));
        await _repositorio.AdicionarAsync(new Aluno("Davi", Nivel.Avancado));

        var avancados = await _repositorio.ListarPorNivelAsync(Nivel.Avancado);

        Assert.Single(avancados);
        Assert.Equal("Davi", avancados[0].Nome);
    }

    [Fact]
    public async Task Remover_devolve_falso_quando_nao_existe()
    {
        Assert.False(await _repositorio.RemoverAsync(99));
    }

    // O repositório guarda Aluno e Monitor na mesma coleção de Participante.
    [Fact]
    public async Task Guarda_tipos_diferentes_na_mesma_colecao()
    {
        await _repositorio.AdicionarAsync(new Aluno("Bia"));
        await _repositorio.AdicionarAsync(new Monitor("Davi", DateTime.UtcNow.AddYears(-1)));

        var todos = await _repositorio.ListarAsync();

        Assert.Equal(2, todos.Count);
        Assert.Contains(todos, p => p is Aluno);
        Assert.Contains(todos, p => p is Monitor);
    }
}
