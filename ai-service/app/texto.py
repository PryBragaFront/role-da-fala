"""Correção simples de frases escritas em inglês.

Cobre erros frequentes de quem está começando. Não substitui um corretor real:
a fase 1 do roadmap prevê trocar isto por um modelo de linguagem.
"""

from __future__ import annotations

import re

# (padrão, correção, explicação em português)
REGRAS: list[tuple[str, str, str]] = [
    (r"\bi\b", "I", 'O pronome "I" é sempre maiúsculo.'),
    (r"\bi'm\b", "I'm", 'O pronome "I" é sempre maiúsculo.'),
    (r"\bim\b", "I'm", 'Falta o apóstrofo: "I am" vira "I\'m".'),
    (r"\bdont\b", "don't", "Falta o apóstrofo em \"don't\"."),
    (r"\bcant\b", "can't", "Falta o apóstrofo em \"can't\"."),
    (r"\bdoesnt\b", "doesn't", "Falta o apóstrofo em \"doesn't\"."),
    (r"\bhe have\b", "he has", 'Com "he", "she" e "it" o verbo vira "has".'),
    (r"\bshe have\b", "she has", 'Com "he", "she" e "it" o verbo vira "has".'),
    (r"\bi has\b", "I have", 'Com "I" o verbo é "have".'),
    (r"\byou is\b", "you are", 'Com "you" o verbo é "are".'),
    (r"\bi is\b", "I am", 'Com "I" o verbo é "am".'),
    (r"\bpeoples\b", "people", '"People" já é plural, não leva "s".'),
    (r"\binformations\b", "information", '"Information" não tem plural em inglês.'),
]


def corrigir(texto: str) -> dict[str, object]:
    """Aplica as regras e devolve o texto corrigido com as explicações."""
    original = texto.strip()
    corrigido = original
    explicacoes: list[str] = []

    for padrao, troca, explicacao in REGRAS:
        if re.search(padrao, corrigido, flags=re.IGNORECASE):
            corrigido = re.sub(padrao, troca, corrigido, flags=re.IGNORECASE)
            if explicacao not in explicacoes:
                explicacoes.append(explicacao)

    # Primeira letra maiúscula.
    if corrigido and corrigido[0].islower():
        corrigido = corrigido[0].upper() + corrigido[1:]
        explicacoes.append("A frase começa com letra maiúscula.")

    # Ponto final.
    if corrigido and corrigido[-1] not in ".!?":
        corrigido += "."
        explicacoes.append("Falta o ponto final.")

    return {
        "original": original,
        "corrigido": corrigido,
        "tem_correcao": corrigido != original,
        "explicacoes": explicacoes,
    }
