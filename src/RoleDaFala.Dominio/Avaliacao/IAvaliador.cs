namespace RoleDaFala.Dominio.Avaliacao;

/// <summary>
/// Contrato de quem sabe avaliar uma resposta.
///
/// INTERFACE: separa o "o quê" do "como". A atividade pede uma avaliação
/// sem saber se ela vem de uma comparação local, de um serviço de fala
/// ou de um modelo de linguagem. Trocar a implementação não mexe no resto.
/// </summary>
public interface IAvaliador
{
    ResultadoAvaliacao Avaliar(string esperado, string recebido);
}
