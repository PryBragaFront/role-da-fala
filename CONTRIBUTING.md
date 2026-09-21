# Como contribuir

Obrigado pelo interesse no Rolê da Fala. Este é um projeto de estudantes, e contribuições de todos os níveis são bem-vindas.

## Antes de começar

Leia o [código de conduta](CODE_OF_CONDUCT.md) e o [README](README.md). Se for a sua primeira contribuição, procure issues marcadas com `good first issue`.

## Rodando o projeto

O repositório tem quatro partes: `frontend/` (HTML), `src/RoleDaFala.Dominio` e `src/RoleDaFala.Api` (C#), `tests/` (xUnit) e `ai-service/` (Python). Cada uma tem o seu README com as instruções. Para mexer só na tela, você precisa apenas do front.

Não há build nem dependências no front. Clone o repositório e abra o `frontend/index.html` no navegador, ou rode `cd frontend && python3 -m http.server 8000`.

## Fluxo de trabalho

1. Abra uma issue descrevendo o que pretende mudar, antes de escrever código.
2. Crie um branch a partir do `main`, com nome descritivo: `feat/pacote-offline`, `fix/botao-cortado`.
3. Faça commits pequenos, com mensagens em português no imperativo: `adiciona figura de ônibus`, `corrige contraste no modo escuro`.
4. Abra um pull request explicando o que mudou e por quê, com prints se a mudança for visual.

## Padrões de código

O front é um arquivo HTML único, sem framework, por escolha: precisa abrir rápido em celular simples e funcionar sem instalação. Mantenha isso ao contribuir.

**Front (HTML, CSS e JS)**
- Indentação de 2 espaços.
- JavaScript sem dependências externas.
- Nada de bibliotecas de fora, exceto as fontes do Google Fonts já usadas.
- Textos sempre nos dois idiomas, com a função `tr(pt, en)`.

**Domínio e API (C#)**
- Indentação de 4 espaços, padrão do .NET.
- Nomes de classes, propriedades e rotas em português, como já está no código.
- Regra de negócio fica no domínio (`src/RoleDaFala.Dominio`), nunca no controller nem no front.
- Uma classe por arquivo; controllers só traduzem HTTP.
- Toda classe ou regra nova entra com teste em `tests/RoleDaFala.Testes`.
- Validação dentro da entidade, lançando `DominioException`.

**IA (Python)**
- Indentação de 4 espaços, nomes em `snake_case`.
- Anotações de tipo em toda função pública.
- Docstring explicando o porquê, não só o quê.
- Todo comportamento novo entra com teste em `tests/`.

## Acessibilidade é requisito

Toda contribuição de interface precisa passar por esta lista:

- [ ] Funciona com teclado, sem mouse.
- [ ] Elementos interativos têm rótulo acessível (`aria-label` ou texto visível).
- [ ] Contraste suficiente nos temas claro e escuro.
- [ ] Continua legível com a fonte de dislexia e com letras grandes.
- [ ] Nada depende só de cor para transmitir informação.
- [ ] Nada depende só de áudio: há alternativa em texto.

## Reportando problemas

Use os modelos de issue. Inclua o navegador, o aparelho e o que você esperava que acontecesse.
