from app.texto import corrigir


def test_pronome_i_vira_maiusculo():
    r = corrigir("i am fine")
    assert r["corrigido"] == "I am fine."
    assert r["tem_correcao"] is True
    assert any("maiúsculo" in e for e in r["explicacoes"])


def test_concordancia_he_have():
    r = corrigir("he have a dog")
    assert "he has" in r["corrigido"].lower()


def test_frase_correta_ganha_so_ponto_final():
    r = corrigir("She is my friend")
    assert r["corrigido"] == "She is my friend."


def test_frase_ja_correta_nao_muda():
    r = corrigir("She is my friend.")
    assert r["tem_correcao"] is False
