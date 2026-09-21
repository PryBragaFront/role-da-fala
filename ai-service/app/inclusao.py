"""Adapta o conteúdo de uma atividade ao perfil de acessibilidade da pessoa.

Assim como em pronuncia.py e texto.py, isto é uma simulação por regras, não um
modelo de linguagem: cobre os apoios que já existem no cadastro (ver
RoleDaFala.Dominio/Acessibilidade/TipoApoio.cs) e decide o que muda no
conteúdo antes de chegar à tela. Trocar por um modelo real de IA é trabalho
para depois da reunião de alinhamento — a interface (entrada e saída) já fica
pronta para isso.
"""

from __future__ import annotations

import re

APOIOS_CONHECIDOS = {"nenhum", "cegueira", "baixavisao", "dislexia", "tdah", "autismo", "surdez"}

# Palavras que sinalizam urgência ou pressa — suavizadas para quem tem autismo,
# já que uma das regras do grupo é evitar quebra brusca de rotina.
PALAVRAS_DE_PRESSA = {
    "rápido": "com calma",
    "rapido": "com calma",
    "agora": "quando puder",
    "urgente": "importante",
}


def _normalizar_apoios(apoios: list[str]) -> set[str]:
    normalizados = {a.strip().lower() for a in apoios if a and a.strip()}
    desconhecidos = normalizados - APOIOS_CONHECIDOS
    if desconhecidos:
        raise ValueError(f"Apoio desconhecido: {', '.join(sorted(desconhecidos))}")
    return normalizados


def _quebrar_em_frases_curtas(texto: str, max_palavras: int = 8) -> str:
    """Corta frases longas em pontos de vírgula ou 'e'/'and', uma ideia por linha."""
    partes = re.split(r",\s*| e | and ", texto)
    linhas = [p.strip() for p in partes if p.strip()]
    return "\n".join(linhas) if len(texto.split()) > max_palavras else texto


def _suavizar_urgencia(texto: str) -> str:
    resultado = texto
    for termo, troca in PALAVRAS_DE_PRESSA.items():
        resultado = re.sub(rf"\b{termo}\b", troca, resultado, flags=re.IGNORECASE)
    return resultado


def adaptar(texto: str, apoios: list[str]) -> dict[str, object]:
    """Devolve o texto adaptado e os sinais que o front usa para ajustar a tela."""
    apoios_normalizados = _normalizar_apoios(apoios)
    adaptado = texto.strip()
    dicas: list[str] = []

    if "dislexia" in apoios_normalizados or "tdah" in apoios_normalizados:
        antes = adaptado
        adaptado = _quebrar_em_frases_curtas(adaptado)
        if adaptado != antes:
            dicas.append("Frase dividida em partes menores para facilitar a leitura.")

    if "autismo" in apoios_normalizados:
        antes = adaptado
        adaptado = _suavizar_urgencia(adaptado)
        if adaptado != antes:
            dicas.append("Linguagem de urgência suavizada para manter um ritmo previsível.")

    prioridade_audio = "cegueira" in apoios_normalizados
    precisa_alternativa_visual = "surdez" in apoios_normalizados
    sessao_curta = "tdah" in apoios_normalizados

    if prioridade_audio:
        dicas.append("Priorize o áudio: leia esta tela em voz alta antes de mostrar o texto.")
    if precisa_alternativa_visual:
        dicas.append("Mostre uma dica visual equivalente: esta pessoa não usa áudio.")
    if sessao_curta:
        dicas.append("Ofereça uma pausa após esta atividade.")

    return {
        "texto_original": texto,
        "texto_adaptado": adaptado,
        "prioridade_audio": prioridade_audio,
        "precisa_alternativa_visual": precisa_alternativa_visual,
        "sessao_curta": sessao_curta,
        "dicas": dicas,
    }
