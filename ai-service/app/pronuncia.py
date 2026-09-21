"""Comparação entre a palavra esperada e o que a pessoa falou.

A nota não é uma medida fonética de verdade: é uma aproximação por semelhança
de escrita, feita sobre o texto que o reconhecimento de voz devolveu. Serve para
dar retorno imediato sem custo de servidor. Trocar por um serviço de fala real
está na fase 1 do roadmap.
"""

from __future__ import annotations

import unicodedata

# Notas a partir das quais a mensagem muda.
NOTA_OTIMA = 85
NOTA_BOA = 60

# Erros comuns de quem fala português aprendendo inglês.
# A chave é a palavra esperada; o valor, as formas que quase acertam.
QUASE_ACERTOS: dict[str, set[str]] = {
    "apple": {"apel", "epol", "apol", "aple"},
    "ball": {"bol", "bou", "bal"},
    "sun": {"san", "son", "sam"},
    "house": {"raus", "haus", "rauz"},
    "book": {"buk", "buque", "buc"},
    "cup": {"cap", "kap", "capi"},
    "banana": {"banana", "banena"},
    "tree": {"tri", "tree", "tchri"},
    "bus": {"bas", "baz", "bus"},
    "dog": {"dog", "dogui", "dag"},
    "cat": {"ket", "cat", "quet"},
    "fish": {"fich", "fixe", "fis"},
}


def normalizar(texto: str) -> str:
    """Tira acentos, espaços das pontas e deixa em minúsculas."""
    texto = texto.strip().lower()
    sem_acento = unicodedata.normalize("NFD", texto)
    return "".join(c for c in sem_acento if unicodedata.category(c) != "Mn")


def levenshtein(a: str, b: str) -> int:
    """Número mínimo de letras a trocar, inserir ou apagar para ir de a até b."""
    if a == b:
        return 0
    if not a:
        return len(b)
    if not b:
        return len(a)

    anterior = list(range(len(b) + 1))
    for i, ca in enumerate(a, start=1):
        atual = [i]
        for j, cb in enumerate(b, start=1):
            custo = 0 if ca == cb else 1
            atual.append(min(atual[j - 1] + 1, anterior[j] + 1, anterior[j - 1] + custo))
        anterior = atual

    return anterior[-1]


def semelhanca(esperado: str, reconhecido: str) -> int:
    """Nota de 0 a 100 pela proximidade entre as duas palavras."""
    a, b = normalizar(esperado), normalizar(reconhecido)

    if not a:
        return 0
    if not b:
        return 0
    if a == b:
        return 100

    if b in QUASE_ACERTOS.get(a, set()):
        return 80

    distancia = levenshtein(a, b)
    nota = round(100 * (1 - distancia / max(len(a), len(b))))
    return max(0, min(100, nota))


def mensagem_para(nota: int) -> str:
    """Retorno em português, no tom das regras do grupo: corrigir sem humilhar."""
    if nota >= NOTA_OTIMA:
        return "Muito bem!"
    if nota >= NOTA_BOA:
        return "Quase lá. Tente de novo."
    return "Vamos tentar de novo, com calma."


def avaliar(esperado: str, reconhecido: str) -> dict[str, object]:
    """Avalia uma tentativa e devolve nota, mensagem e se conta como acerto."""
    nota = semelhanca(esperado, reconhecido)
    return {
        "nota": nota,
        "mensagem": mensagem_para(nota),
        "acertou": nota >= NOTA_BOA,
    }
