using RoleDaFala.Dominio.Atividades;
using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Repositorios;

namespace RoleDaFala.Api.Persistencia;

/// <summary>Cria as atividades iniciais quando a API sobe.</summary>
public static class CargaInicial
{
    public static async Task ExecutarAsync(IServiceProvider servicos)
    {
        var repositorio = servicos.GetRequiredService<IRepositorio<Atividade>>();

        if ((await repositorio.ListarAsync()).Count > 0)
        {
            return;
        }

        // Uma lista de Atividade recebe objetos de três classes diferentes.
        // É polimorfismo: o tipo declarado é o da mãe.
        var atividades = new List<Atividade>
        {
            new AtividadeFigura("apple", "maçã", "épol", "O \"a\" é aberto, como em \"até\"."),
            new AtividadeFigura("ball", "bola", "ból", "Alongue o \"o\" e termine com a língua no céu da boca."),
            new AtividadeFigura("sun", "sol", "sãn", "O \"u\" soa como o \"a\" de \"pá\", curto."),
            new AtividadeFigura("house", "casa", "ráus", "O \"h\" é um sopro, não um \"r\" forte."),
            new AtividadeFigura("book", "livro", "buk", "O \"oo\" é curto, quase um \"u\" rápido."),
            new AtividadeFigura("cup", "xícara", "cáp", "Termine com o lábio fechando, sem soltar \"pi\"."),
            new AtividadeFigura("banana", "banana", "banéna", "A força está na sílaba do meio."),
            new AtividadeFigura("tree", "árvore", "tri", "O \"ee\" é longo, como um \"i\" esticado."),
            new AtividadeFigura("bus", "ônibus", "bás", "Termine com um \"s\" seco."),
            new AtividadeFigura("dog", "cachorro", "dóg", "O \"g\" final é suave, não vira \"gui\"."),
            new AtividadeFigura("cat", "gato", "két", "Abra bem a boca no \"a\"."),
            new AtividadeFigura("fish", "peixe", "fich", "Termine com o som de \"ch\", não de \"s\"."),

            new AtividadeConversa("Cumprimentos", "Nice to meet you! How are you?",
                new[] { "I am fine", "I'm fine, thank you", "I am good" }, Nivel.Zero),

            new AtividadeConversa("Mercado", "How much is the bread?",
                new[] { "It is two reais", "Two reais" }),

            new AtividadeEscuta("The bus arrives at eight")
        };

        foreach (var atividade in atividades)
        {
            await repositorio.AdicionarAsync(atividade);
        }
    }
}
