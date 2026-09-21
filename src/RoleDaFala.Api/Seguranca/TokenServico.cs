using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RoleDaFala.Dominio.Entidades;

namespace RoleDaFala.Api.Seguranca;

/// <summary>
/// Gera o token que o app (web ou, no futuro, Android) manda em cada
/// requisição depois do login, no cabeçalho "Authorization: Bearer ...".
/// </summary>
public class TokenServico
{
    private readonly IConfiguration _config;

    public TokenServico(IConfiguration config)
    {
        _config = config;
    }

    public string GerarToken(Conta conta)
    {
        var chave = _config["Jwt:Key"]
            ?? throw new InvalidOperationException("Configuração 'Jwt:Key' não encontrada.");

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, conta.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, conta.Email),
            new Claim("participanteId", conta.ParticipanteId.ToString()),
        };

        var minutos = int.TryParse(_config["Jwt:ExpiraMinutos"], out var m) ? m : 120;

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutos),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
