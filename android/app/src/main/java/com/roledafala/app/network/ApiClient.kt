package com.roledafala.app.network

import kotlinx.coroutines.suspendCancellableCoroutine
import okhttp3.Call
import okhttp3.Callback
import okhttp3.OkHttpClient
import okhttp3.Request
import org.json.JSONObject
import java.io.IOException
import kotlin.coroutines.resume
import kotlin.coroutines.resumeWithException

/**
 * Cliente mínimo para a API em C# (src/RoleDaFala.Api). Só OkHttp por enquanto,
 * de propósito: qual biblioteca de rede usar (Retrofit, Ktor...) é uma decisão
 * do time, não algo para travar sozinho neste esqueleto.
 *
 * 10.0.2.2 é o endereço que o emulador Android usa para chegar ao "localhost"
 * da máquina que roda o emulador — não é o IP da API. Rodando num celular
 * físico, troque pelo IP da máquina na mesma rede.
 */
object ApiClient {
    const val BASE_URL_EMULADOR = "http://10.0.2.2:5080"

    private val client = OkHttpClient()

    /** Chama GET /health e devolve o texto do campo "status", ou lança em caso de erro. */
    suspend fun verificarSaude(baseUrl: String = BASE_URL_EMULADOR): String =
        suspendCancellableCoroutine { continuacao ->
            val request = Request.Builder().url("$baseUrl/health").build()

            val chamada = client.newCall(request)
            continuacao.invokeOnCancellation { chamada.cancel() }

            chamada.enqueue(object : Callback {
                override fun onFailure(call: Call, e: IOException) {
                    continuacao.resumeWithException(e)
                }

                override fun onResponse(call: Call, response: okhttp3.Response) {
                    response.use {
                        if (!it.isSuccessful) {
                            continuacao.resumeWithException(IOException("HTTP ${it.code}"))
                            return
                        }

                        val corpo = it.body?.string().orEmpty()
                        val status = JSONObject(corpo).optString("status", "desconhecido")
                        continuacao.resume(status)
                    }
                }
            })
        }
}
