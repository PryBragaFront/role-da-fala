using RoleDaFala.Api.Persistencia;
using RoleDaFala.Api.Servicos;
using RoleDaFala.Dominio.Atividades;
using RoleDaFala.Dominio.Avaliacao;
using RoleDaFala.Dominio.Entidades;
using RoleDaFala.Dominio.Repositorios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// INJEÇÃO DE DEPENDÊNCIA: a API registra qual implementação usar.
// Os controllers e os serviços pedem a interface, nunca a classe concreta.
builder.Services.AddSingleton<IParticipanteRepositorio, ParticipanteRepositorioEmMemoria>();
builder.Services.AddSingleton<IRepositorio<Atividade>, RepositorioEmMemoria<Atividade>>();

// COMPOSIÇÃO: o avaliador com sotaque embrulha o de semelhança.
// Trocar por um serviço de fala real muda só esta linha.
builder.Services.AddSingleton<IAvaliador>(_ =>
    new AvaliadorComSotaque(new AvaliadorPorSemelhanca()));

builder.Services.AddScoped<IParticipanteServico, ParticipanteServico>();
builder.Services.AddScoped<IAtividadeServico, AtividadeServico>();

const string CorsFront = "front";
builder.Services.AddCors(o => o.AddPolicy(CorsFront, p => p
    .WithOrigins("http://localhost:8000", "http://127.0.0.1:8000")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

// Conteúdo inicial: as 12 figuras do protótipo.
await CargaInicial.ExecutarAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsFront);
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

/// <summary>Torna a classe visível para o projeto de testes.</summary>
public partial class Program { }
