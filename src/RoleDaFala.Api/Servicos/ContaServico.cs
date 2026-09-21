using RoleDaFala.Api.Dtos;
using RoleDaFala.Api.Seguranca;
using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Excecoes;
using RoleDaFala.Dominio.Repositorios;
using RoleDaFala.Dominio.Seguranca;

namespace RoleDaFala.Api.Servicos;

/// <summary>
/// Junta duas coisas que o cadastro faz de uma vez: cria o participante
/// (nome, nível, apoios — regra que já existia) e cria a conta de login
/// (e-mail, senha — a parte nova, gravada no SQLite).
/// </summary>
public class ContaServico : IContaServico
{
    private readonly IParticipanteServico _participanteServico;
    private readonly IContaRepositorio _contaRepositorio;
    private readonly TokenServico _tokenServico;

    public ContaServico(
        IParticipanteServico participanteServico,
        IContaRepositorio contaRepositorio,
        TokenServico tokenServico)
    {
        _participanteServico = participanteServico;
        _contaRepositorio = contaRepositorio;
        _tokenServico = tokenServico;
    }

    public async Task<LoginResponse> RegistrarAsync(RegistrarContaRequest requisicao)
    {
        if (await _contaRepositorio.ObterPorEmailAsync(requisicao.Email) is not null)
        {
            throw new DominioException("Já existe uma conta com este e-mail.");
        }

        var participante = await _participanteServico.CriarAsync(
            new CriarParticipanteRequest(requisicao.Nome, requisicao.Tipo, requisicao.Nivel, requisicao.Apoios));

        var conta = new Conta(requisicao.Email, HashDeSenha.Gerar(requisicao.Senha), participante.Id);
        await _contaRepositorio.AdicionarAsync(conta);

        return new LoginResponse(_tokenServico.GerarToken(conta), participante);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest requisicao)
    {
        var conta = await _contaRepositorio.ObterPorEmailAsync(requisicao.Email);

        if (conta is null || !HashDeSenha.Confere(requisicao.Senha, conta.SenhaHash))
        {
            return null;
        }

        var participante = await _participanteServico.ObterAsync(conta.ParticipanteId);

        if (participante is null)
        {
            // Não deveria acontecer: participante e conta são criados juntos em
            // RegistrarAsync, e os dois agora são persistidos. Só chegaria aqui
            // se o participante fosse removido diretamente do banco.
            throw new DominioException("A conta existe, mas o participante ligado a ela não foi encontrado.");
        }

        return new LoginResponse(_tokenServico.GerarToken(conta), participante);
    }
}
