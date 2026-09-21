# Front (HTML, CSS e JavaScript)

A tela do app. Arquivo único, sem framework e sem build, porque precisa abrir rápido em celular simples.

## Rodando

```bash
cd frontend
python3 -m http.server 8000
# acesse http://localhost:8000
```

Ou abra o `index.html` direto no navegador, com dois cliques.

Use o Chrome e permita o microfone para testar a correção de pronúncia. Em navegadores sem reconhecimento de voz, o app mostra uma nota simulada e avisa isso na tela.

## Arquivos

- `index.html` — o app inteiro: telas, estilos e lógica
- `js/api.js` — cliente da API em C#, para quando o backend estiver no ar

## Conectando com a API

O `index.html` funciona sozinho, sem backend: é assim que ele é demonstrado hoje. Para ligá-lo à API, inclua o cliente antes do script principal:

```html
<script src="js/api.js"></script>
```

E, no lugar onde hoje a nota é calculada localmente, chame:

```js
var r = await Api.avaliarPronuncia(usuarioId, figuraId, textoReconhecido);
```

Se a URL da API mudar, ajuste com `Api.configurar('http://outro-endereco')`.

## Regras ao mexer aqui

- Todo texto novo entra nos dois idiomas, com `tr(pt, en)`.
- Nada de bibliotecas externas, além das fontes já usadas.
- Confira a lista de acessibilidade do [guia de contribuição](../CONTRIBUTING.md) antes de abrir o PR.
