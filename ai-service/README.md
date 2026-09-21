# Serviço auxiliar (Python)

Corrige frases escritas em inglês e adapta conteúdo ao perfil de acessibilidade. É opcional: a API em C# funciona sem ele.

A avaliação de pronúncia ficou no domínio em C# (`AvaliadorPorSemelhanca` e `AvaliadorComSotaque`), para ser testada junto com as regras de negócio. Este serviço permanece como demonstração de integração entre serviços e é onde, na etapa 10, entraria um modelo de linguagem de verdade.

## Rodando

```bash
cd ai-service
python3 -m venv .venv
source .venv/bin/activate        # Windows: .venv\Scripts\activate
pip install -r requirements.txt
uvicorn app.main:app --reload --port 8001
```

Documentação interativa em `http://localhost:8001/docs`.

## Endpoints

### POST /avaliar

```json
{ "esperado": "apple", "reconhecido": "apol" }
```

```json
{ "nota": 80, "mensagem": "Quase lá. Tente de novo.", "acertou": true }
```

### POST /corrigir

```json
{ "texto": "i am fine" }
```

```json
{
  "original": "i am fine",
  "corrigido": "I am fine.",
  "tem_correcao": true,
  "explicacoes": ["O pronome \"I\" é sempre maiúsculo.", "Falta o ponto final."]
}
```

### POST /adaptar

```json
{ "texto": "Responda rápido!", "apoios": ["Autismo"] }
```

```json
{
  "texto_original": "Responda rápido!",
  "texto_adaptado": "Responda com calma!",
  "prioridade_audio": false,
  "precisa_alternativa_visual": false,
  "sessao_curta": false,
  "dicas": ["Linguagem de urgência suavizada para manter um ritmo previsível."]
}
```

Regras por enquanto (ver `app/inclusao.py`), não um modelo de linguagem — os apoios aceitos são os mesmos do cadastro: `Cegueira`, `BaixaVisao`, `Dislexia`, `Tdah`, `Autismo`, `Surdez`.

## Testes

Não há testes por enquanto: foram removidos de propósito nesta etapa (ver [`docs/2-Arquitetura.md`](../docs/2-Arquitetura.md#próximos-passos)), para serem recriados do zero depois da mudança de arquitetura. Quando isso acontecer, `pytest` volta a ser a ferramenta (basta reinstalar como dependência de desenvolvimento).

## Limitação importante

A nota de pronúncia é uma aproximação por semelhança de escrita, calculada sobre o texto que o reconhecimento de voz do navegador devolveu. Não é uma análise fonética do áudio. Trocar por um serviço de fala real está na fase 1 do roadmap; quando isso acontecer, só este serviço muda.
