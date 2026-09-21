from app.pronuncia import avaliar, levenshtein, normalizar, semelhanca


def test_normalizar_tira_acento_e_espaco():
    assert normalizar("  Maçã  ") == "maca"


def test_levenshtein_conta_trocas():
    assert levenshtein("cat", "cat") == 0
    assert levenshtein("cat", "bat") == 1
    assert levenshtein("cat", "") == 3


def test_palavra_exata_vale_cem():
    assert semelhanca("apple", "apple") == 100
    assert semelhanca("apple", "  APPLE ") == 100


def test_quase_acerto_conhecido_pontua_bem():
    assert semelhanca("apple", "apol") == 80
    assert semelhanca("cat", "ket") == 80


def test_palavra_errada_pontua_baixo():
    assert semelhanca("apple", "banana") < 40


def test_silencio_vale_zero():
    assert semelhanca("apple", "") == 0


def test_avaliar_devolve_mensagem_e_acerto():
    resultado = avaliar("apple", "apple")
    assert resultado["nota"] == 100
    assert resultado["acertou"] is True
    assert resultado["mensagem"] == "Muito bem!"

    ruim = avaliar("apple", "xyz")
    assert ruim["acertou"] is False
