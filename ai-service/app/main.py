"""Serviço de IA do Rolê da Fala.

Responde à API em C#, que é quem o front conhece. Não guarda nada:
recebe texto, devolve nota ou correção.
"""

from __future__ import annotations

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, Field

from app.inclusao import adaptar
from app.pronuncia import avaliar
from app.texto import corrigir

app = FastAPI(
    title="Rolê da Fala — serviço de IA",
    description="Avalia pronúncia e corrige frases em inglês.",
    version="0.1.0",
)

# Em produção, restrinja aos domínios da API.
app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:5080", "http://localhost:8000"],
    allow_methods=["*"],
    allow_headers=["*"],
)


class AvaliarRequest(BaseModel):
    esperado: str = Field(..., description="Palavra correta em inglês", examples=["apple"])
    reconhecido: str = Field(..., description="O que o reconhecimento de voz entendeu", examples=["apol"])


class AvaliarResponse(BaseModel):
    nota: int
    mensagem: str
    acertou: bool


class CorrigirRequest(BaseModel):
    texto: str = Field(..., description="Frase escrita pela pessoa", examples=["i am fine"])


class CorrigirResponse(BaseModel):
    original: str
    corrigido: str
    tem_correcao: bool
    explicacoes: list[str]


class AdaptarRequest(BaseModel):
    texto: str = Field(..., description="Conteúdo original da atividade", examples=["Responda rápido!"])
    apoios: list[str] = Field(
        default_factory=list,
        description="Apoios do perfil de acessibilidade (ex.: Dislexia, Tdah)",
        examples=[["Dislexia", "Tdah"]],
    )


class AdaptarResponse(BaseModel):
    texto_original: str
    texto_adaptado: str
    prioridade_audio: bool
    precisa_alternativa_visual: bool
    sessao_curta: bool
    dicas: list[str]


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}


@app.post("/avaliar", response_model=AvaliarResponse)
def avaliar_pronuncia(req: AvaliarRequest) -> dict[str, object]:
    """Compara o que a pessoa falou com a palavra esperada."""
    return avaliar(req.esperado, req.reconhecido)


@app.post("/corrigir", response_model=CorrigirResponse)
def corrigir_texto(req: CorrigirRequest) -> dict[str, object]:
    """Corrige erros comuns em uma frase curta."""
    return corrigir(req.texto)


@app.post("/adaptar", response_model=AdaptarResponse)
def adaptar_conteudo(req: AdaptarRequest) -> dict[str, object]:
    """Adapta um texto ao perfil de acessibilidade informado."""
    return adaptar(req.texto, req.apoios)
