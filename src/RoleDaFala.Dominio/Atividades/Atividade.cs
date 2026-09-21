using RoleDaFala.Dominio.Acessibilidade;
using RoleDaFala.Dominio.Avaliacao;
using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Excecoes;

namespace RoleDaFala.Dominio.Atividades;

/// <summary>
/// Qualquer exercício do app.
///
/// CLASSE ABSTRATA: "atividade" não existe sozinha; existem figuras,
/// conversas e textos. Ela define o que toda atividade tem e obriga
/// as filhas a dizerem como se avaliam.
///
/// TEMPLATE METHOD: Responder() é o passo a passo fixo (validar, avaliar,
/// pontuar). O que muda entre as filhas é só ExecutarAvaliacao().
/// </summary>
public abstract class Atividade : Entidade
{
    protected Atividade(string enunciado, Nivel nivelMinimo)
    {
        if (string.IsNullOrWhiteSpace(enunciado))
        {
            throw new DominioException("A atividade precisa de um enunciado.");
        }

        Enunciado = enunciado.Trim();
        NivelMinimo = nivelMinimo;
    }

    public string Enunciado { get; }
    public Nivel NivelMinimo { get; }

    /// <summary>Nome do tipo de atividade, para exibir na tela.</summary>
    public abstract string Titulo { get; }

    /// <summary>A atividade depende de ouvir? Se sim, não serve para quem é surdo.</summary>
    public abstract bool DependeDeAudio { get; }

    /// <summary>
    /// Cada filha avalia de um jeito.
    /// POLIMORFISMO: quem chama Responder() não sabe qual atividade está usando.
    /// </summary>
    protected abstract ResultadoAvaliacao ExecutarAvaliacao(string resposta, IAvaliador avaliador);

    /// <summary>
    /// Passo a passo igual para toda atividade. As filhas não reescrevem isto,
    /// só a parte que muda. Por isso o método não é virtual.
    /// </summary>
    public ResultadoAvaliacao Responder(Participante participante, string resposta, IAvaliador avaliador)
    {
        ArgumentNullException.ThrowIfNull(participante);
        ArgumentNullException.ThrowIfNull(avaliador);

        if (!EstaDisponivelPara(participante))
        {
            throw new DominioException($"A atividade \"{Titulo}\" não está disponível para {participante.Nome}.");
        }

        var resultado = ExecutarAvaliacao(resposta ?? string.Empty, avaliador);

        if (resultado.Acertou)
        {
            participante.GanharXp(participante.XpPorAcerto());
        }

        return resultado;
    }

    /// <summary>
    /// Duas regras: o nível precisa alcançar, e a atividade não pode depender
    /// de áudio se a pessoa é surda.
    /// </summary>
    public virtual bool EstaDisponivelPara(Participante participante)
    {
        if (participante.Nivel < NivelMinimo)
        {
            return false;
        }

        if (DependeDeAudio && participante.Acessibilidade.Possui(TipoApoio.Surdez))
        {
            return false;
        }

        return true;
    }
}
