using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoleDaFala.Api.Persistencia;
using RoleDaFala.Api.Seguranca;
using RoleDaFala.Api.Servicos;
using RoleDaFala.Dominio.Atividades;
using RoleDaFala.Dominio.Avaliacao;
using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Repositorios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.AddSecurityDefinition("Bearer", new()
{
    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
    Scheme = "bearer",
    BearerFormat = "JWT",
}));

// INJEÇÃO DE DEPENDÊNCIA: a API registra qual implementação usar.
// Os controllers e os serviços pedem a interface, nunca a classe concreta.
builder.Services.AddSingleton<IParticipanteRepositorio, ParticipanteRepositorioEmMemoria>();
builder.Services.AddSingleton<IRepositorio<Atividade>, RepositorioEmMemoria<Atividade>>();

// Conta é a primeira entidade a sair da memória: login precisa sobreviver
// a reiniciar a API. Participante e Atividade continuam em memória por
// enquanto (ver "Próximos passos" em docs/2-Arquitetura.md).
builder.Services.AddDbContext<RoleDaFalaDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IContaRepositorio, ContaRepositorioSqlite>();
builder.Services.AddScoped<TokenServico>();
builder.Services.AddScoped<IContaServico, ContaServico>();

// COMPOSIÇÃO: o avaliador com sotaque embrulha o de semelhança.
// Trocar por um serviço de fala real muda só esta linha.
builder.Services.AddSingleton<IAvaliador>(_ =>
    new AvaliadorComSotaque(new AvaliadorPorSemelhanca()));

builder.Services.AddScoped<IParticipanteServico, ParticipanteServico>();
builder.Services.AddScoped<IAtividadeServico, AtividadeServico>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
    });
builder.Services.AddAuthorization();

const string CorsFront = "front";
var origensPermitidas = builder.Configuration.GetSection("Cors:OrigensPermitidas").Get<string[]>()
    ?? ["http://localhost:8000", "http://127.0.0.1:8000"];
builder.Services.AddCors(o => o.AddPolicy(CorsFront, p => p
    .WithOrigins(origensPermitidas)
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

// Cria o banco (Contas) se ainda não existir. Um projeto real usaria
// migrations (dotnet ef migrations); aqui isso ainda seria cedo demais.
using (var escopo = app.Services.CreateScope())
{
    var db = escopo.ServiceProvider.GetRequiredService<RoleDaFalaDbContext>();
    await db.Database.EnsureCreatedAsync();
}

// Conteúdo inicial: as 12 figuras do protótipo.
await CargaInicial.ExecutarAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsFront);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

/// <summary>Torna a classe visível para o projeto de testes.</summary>
public partial class Program { }
