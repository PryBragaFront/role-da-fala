# Rolê da Fala

Protótipo de aplicativo para praticar inglês em grupo, pensado para uso em comunidades e favelas. Funciona no celular, dá prioridade ao áudio e usa o português como apoio.

> **Status:** protótipo navegável (alta fidelidade). Não é um produto em produção: os dados são de exemplo e a inteligência artificial é simulada. Veja [Limitações](#limitações).

## Sobre o nome

"Rolê" é encontro, não aula: o app nasceu da ideia de reunir pessoas para estudar juntas. "Fala" é o que se pratica ali, porque o áudio e a pronúncia vêm antes da leitura. O nome usa a linguagem do dia a dia, sem tom escolar, para que quem nunca estudou inglês não se sinta intimidado.

## Autoria

A ideia é da **Pricilla Lopes Braga**: reunir estudantes para estudar juntos e trocar conhecimento. O grupo evoluiu em conversa, escreveu o documento com as regras de convivência e, a partir dele, decidiu transformar a ideia em um app.

Participam do projeto, por enquanto, em ordem alfabética: **Davi Alves Silveira**, **Glendha Paulino Tolentino**, **Pricilla Lopes Braga** e **Welberthy Gustavo de Freitas Morais**, com orientação do **Professor Humberto Nigri**.

Este não é um projeto institucional de nenhuma universidade.

## Por que este projeto existe

Muita gente em comunidades e favelas quer aprender inglês, mas esbarra em internet fraca ou cara, celulares simples, pouco tempo, cursos caros e o receio de errar na frente dos outros. Boa parte dos apps de idiomas consome muitos dados e supõe leitura fluente, o que deixa essas pessoas de fora.

O Rolê da Fala parte de três escolhas: o áudio vem primeiro, o português é apoio permanente e a prática acontece em grupo, seguindo regras de convivência definidas pela própria comunidade.

## Funcionalidades

| Tela | O que faz |
| --- | --- |
| Cadastro | Nome, nível e necessidades de apoio. Botão para ativar o modo por voz. |
| Início | Dias seguidos, XP, palavras salvas e atalhos. Níveis: 0, Iniciante, Intermediário e Avançado. |
| Tutor IA – Figuras | Mostra uma imagem, a pessoa fala o nome em inglês e recebe nota de pronúncia e dica. São 12 figuras. |
| Tutor IA – Conversas | Cenários guiados: cumprimentos, mercado, consulta médica, entrevista de emprego e debate. |
| Tutor IA – Palavras | Cartões das palavras salvas para revisão. |
| Salas | Salas de prática e dicas da comunidade. |
| Feedback | Speaking (áudio) e Writing (texto), com comentários de colegas. |
| Desafios e Chamadas | Desafios, chamadas em dupla só por voz ou com vídeo, e rodas de conversa. |
| Regras | Regras de convivência do grupo e créditos do projeto. |

O nível 0, antes do iniciante, permite aprender ouvindo, vendo figuras e repetindo, sem precisar ler em inglês.

## Acessibilidade

No cadastro a pessoa escolhe, de forma opcional e privada, o apoio de que precisa. A interface muda na hora e pode ser ajustada depois no perfil.

| Apoio | O que muda |
| --- | --- |
| Cegueira | O app lê o nome de cada tela em voz alta, com botão "ler a tela" e botões grandes. |
| Baixa visão | Letras maiores, alto contraste e botões grandes. |
| Dislexia | Fonte Lexend, mais espaço entre letras e palavras e botão para ouvir os textos. |
| TDAH | Uma tarefa por vez, sessões de 5 minutos com cronômetro, pausas e menos itens na tela. |
| Autismo (TEA) | Rotina previsível, sem animações, cores suaves e aviso antes de trocar de atividade. |
| Surdez | Tudo em texto e com dicas visuais de pronúncia, sem depender de áudio. |

## Estrutura do repositório

O projeto usa três linguagens, uma por camada:

```
role-da-fala/
├── frontend/                    HTML, CSS e JavaScript — a tela do app
├── src/
│   ├── RoleDaFala.Dominio/      C# — as classes do domínio (o núcleo de POO)
│   └── RoleDaFala.Api/          C# — controllers, serviços e DTOs
├── tests/RoleDaFala.Testes/     C# — 58 testes com xUnit
├── ai-service/                  Python (FastAPI) — correção de texto
└── docs/                        documentação do projeto
```

O front fala apenas com a API em C#. A API não tem regra de negócio própria: ela traduz HTTP e delega ao domínio, que é onde vivem as classes. O detalhamento está em [`docs/arquitetura.md`](docs/arquitetura.md), e o mapa dos conceitos de POO em [`docs/poo.md`](docs/poo.md).

## Como executar

Cada parte roda sozinha. Para ver o protótipo funcionando, basta a primeira.

### 1. Front (não precisa das outras partes)

```bash
cd frontend
python3 -m http.server 8000
# acesse http://localhost:8000
```

Ou abra o `frontend/index.html` direto no navegador. Use o Chrome e permita o microfone para testar a pronúncia.

### 2. Serviço de IA (Python)

```bash
cd ai-service
python3 -m venv .venv
source .venv/bin/activate        # Windows: .venv\Scripts\activate
pip install -r requirements.txt
uvicorn app.main:app --reload --port 8001
```

### 3. API (C#)

Requer o SDK do .NET 8.

```bash
dotnet restore
dotnet run --project src/RoleDaFala.Api
# API em http://localhost:5080, Swagger em http://localhost:5080/swagger
```

As 15 atividades iniciais são criadas quando a API sobe.

### Testes

```bash
dotnet test                 # 58 testes do domínio e da API
cd ai-service && pip install -r requirements-dev.txt && pytest   # 11 testes
```

## Tecnologia

| Camada | Linguagem | O que usa |
| --- | --- | --- |
| Front | HTML, CSS e JavaScript | Arquivo único, sem framework e sem build. Fala do app pela `SpeechSynthesis` e reconhecimento pela `SpeechRecognition` do navegador. Figuras em SVG desenhadas em código. |
| Domínio | C# (.NET 8) | Biblioteca de classes pura, sem dependência de framework. Classes abstratas, herança, polimorfismo, interfaces e genéricos. |
| API | C# (.NET 8) | ASP.NET Core Web API com controllers, injeção de dependência, DTOs e Swagger. |
| Testes | C# e Python | xUnit no C# (58 testes) e pytest no Python (11 testes). |
| Serviço auxiliar | Python 3.11+ | FastAPI para a correção de texto. Sem estado e sem banco. |

O reconhecimento de voz acontece no navegador, não no servidor: economiza dados e evita enviar áudio pela rede, o que importa muito para o público do app.

## Limitações

- Nada é salvo: ao recarregar a página, o cadastro e o progresso se perdem.
- Não há contas, backend, chamadas reais entre pessoas nem moderação.
- O tutor de IA e as notas de pronúncia são simulados quando o navegador não oferece reconhecimento de voz.
- Os pacotes offline são uma proposta de design; o uso sem internet ainda precisa ser construído.
- Nomes, horários e comentários exibidos são fictícios.
- As figuras são ilustrações, não fotografias.

## Roadmap

1. **Validação.** Testar o protótipo com um grupo da comunidade e ajustar regras, conteúdo e moderação.
2. **Primeira versão real.** Contas, progresso salvo e pacote offline.
3. **Comunidade.** Salas, chamadas e moderação com pessoas reais.
4. **Expansão.** Outros idiomas além do inglês.

O backlog completo, com riscos e métricas, está em [`docs/roadmap.md`](docs/roadmap.md).

## Documentação

- [`docs/projeto.md`](docs/projeto.md) — visão geral, problema, público e objetivos
- [`docs/arquitetura.md`](docs/arquitetura.md) — como as camadas se dividem, com diagramas
- [`docs/poo.md`](docs/poo.md) — onde cada conceito de orientação a objetos está no código, com diagrama de classes
- [`docs/roadmap.md`](docs/roadmap.md) — fases, backlog, riscos e métricas
- [`docs/acessibilidade.md`](docs/acessibilidade.md) — decisões de acessibilidade
- [`docs/demonstracao.md`](docs/demonstracao.md) — roteiro de apresentação
- [`docs/regras-do-grupo.md`](docs/regras-do-grupo.md) — regras de convivência

Cada pasta de código tem o seu próprio README: [frontend](frontend/README.md) e [ai-service](ai-service/README.md).

## Como contribuir

Leia o [guia de contribuição](CONTRIBUTING.md) e o [código de conduta](CODE_OF_CONDUCT.md).

## Licença

[MIT](LICENSE). Copyright (c) 2026 Davi Alves Silveira, Glendha Paulino Tolentino, Pricilla Lopes Braga e Welberthy Gustavo de Freitas Morais.
