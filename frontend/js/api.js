/**
 * Cliente da API do Rolê da Fala.
 *
 * O front conversa apenas com a API em C#. Se ela estiver fora do ar,
 * o app continua funcionando com os dados que já estão na tela.
 */
(function (global) {
  'use strict';

  var BASE_URL = 'http://localhost:5080';

  function url(caminho) {
    return BASE_URL.replace(/\/$/, '') + caminho;
  }

  async function pedir(caminho, opcoes) {
    var config = Object.assign(
      { headers: { 'Content-Type': 'application/json' } },
      opcoes || {}
    );

    var resposta = await fetch(url(caminho), config);

    if (!resposta.ok) {
      var corpo = null;
      try { corpo = await resposta.json(); } catch (e) { /* sem corpo */ }

      var erro = new Error((corpo && corpo.erro) || 'A API respondeu ' + resposta.status);
      erro.status = resposta.status;
      throw erro;
    }

    return resposta.status === 204 ? null : resposta.json();
  }

  var Api = {
    /** Troque a URL se a API rodar em outro endereço. */
    configurar: function (baseUrl) {
      BASE_URL = baseUrl;
    },

    estaOnline: async function () {
      try {
        await pedir('/health');
        return true;
      } catch (e) {
        return false;
      }
    },

    /** tipo: "aluno" ou "monitor". apoios: ["Dislexia", "Tdah", ...] */
    criarParticipante: function (nome, tipo, nivel, apoios) {
      return pedir('/participantes', {
        method: 'POST',
        body: JSON.stringify({
          nome: nome,
          tipo: tipo || 'aluno',
          nivel: nivel || 'Zero',
          apoios: apoios || []
        })
      });
    },

    buscarParticipante: function (id) {
      return pedir('/participantes/' + id);
    },

    atualizarApoios: function (id, apoios) {
      return pedir('/participantes/' + id + '/apoios', {
        method: 'PUT',
        body: JSON.stringify(apoios)
      });
    },

    registrarPresenca: function (id) {
      return pedir('/participantes/' + id + '/presenca', { method: 'POST' });
    },

    /** Sem participanteId lista tudo; com ele, só o que a pessoa pode fazer. */
    listarAtividades: function (participanteId) {
      var q = participanteId ? '?participanteId=' + participanteId : '';
      return pedir('/atividades' + q);
    },

    /** Manda o que o reconhecimento de voz entendeu e recebe a nota. */
    responder: function (atividadeId, participanteId, resposta) {
      return pedir('/atividades/' + atividadeId + '/responder', {
        method: 'POST',
        body: JSON.stringify({ participanteId: participanteId, resposta: resposta })
      });
    }
  };

  global.Api = Api;
})(window);
