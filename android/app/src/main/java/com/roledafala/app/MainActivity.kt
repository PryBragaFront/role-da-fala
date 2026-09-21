package com.roledafala.app

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Button
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.roledafala.app.network.ApiClient
import com.roledafala.app.ui.theme.RoleDaFalaTheme
import kotlinx.coroutines.launch

/**
 * Esqueleto: só esta tela existe. Nenhuma funcionalidade do app (cadastro,
 * atividades, salas...) foi construída ainda — o objetivo aqui é só provar
 * que o projeto compila, abre e consegue falar com a API em C#.
 */
class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            RoleDaFalaTheme {
                Surface(modifier = Modifier.fillMaxSize()) {
                    TelaInicial()
                }
            }
        }
    }
}

@Composable
private fun TelaInicial() {
    var resultadoConexao by remember { mutableStateOf<String?>(null) }
    var verificando by remember { mutableStateOf(false) }
    val escopo = rememberCoroutineScope()

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(24.dp),
        verticalArrangement = Arrangement.spacedBy(16.dp, Alignment.CenterVertically),
    ) {
        Text(text = "Rolê da Fala", style = MaterialTheme.typography.headlineMedium)
        Text(text = "Esqueleto do app Android — em construção após a reunião de alinhamento do time.")

        Button(
            enabled = !verificando,
            onClick = {
                verificando = true
                resultadoConexao = null
                escopo.launch {
                    resultadoConexao = try {
                        "Conectado à API (status: ${ApiClient.verificarSaude()})"
                    } catch (erro: Exception) {
                        "Não foi possível falar com a API: ${erro.message}. " +
                            "Ela está rodando (dotnet run --project src/RoleDaFala.Api)?"
                    }
                    verificando = false
                }
            },
        ) {
            Text(if (verificando) "Verificando..." else "Testar conexão com a API")
        }

        resultadoConexao?.let { Text(text = it) }
    }
}
