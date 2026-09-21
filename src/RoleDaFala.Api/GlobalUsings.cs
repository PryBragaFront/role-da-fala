// O SDK Web traz "using System.Threading" implícito, que já tem uma classe
// Monitor — em conflito com RoleDaFala.Dominio.Entidades.Monitor. Resolvido
// uma vez só aqui, para todo o projeto, em vez de em cada arquivo que usa Monitor.
global using Monitor = RoleDaFala.Dominio.Entidades.Monitor;
