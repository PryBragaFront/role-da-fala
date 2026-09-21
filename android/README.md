# App Android (esqueleto)

Status: **estrutura inicial, sem funcionalidade do app ainda.** Existe uma única tela, que só serve para confirmar que o projeto compila e consegue falar com a API em C#. Cadastro, atividades, salas etc. ainda não foram construídos — isso fica para depois da reunião de alinhamento do time (ver [`../docs/2-Arquitetura.md`](../docs/2-Arquitetura.md#próximos-passos)).

> **Importante:** este esqueleto foi escrito sem Android Studio nem o SDK do Android instalados. `./gradlew :app:tasks` roda com sucesso (o Gradle wrapper, os plugins e as versões do Kotlin/AGP/Compose foram validados de verdade), mas a compilação em si (`assembleDebug`) não pôde ser testada — falta o SDK do Android, que só o Android Studio instala. Ou seja: a configuração do projeto está correta, mas o código Kotlin em si (`MainActivity.kt`, `ApiClient.kt`, `Theme.kt`) ainda não foi compilado nem rodado por ninguém. É bem possível que o primeiro `Sync` no Android Studio peça para atualizar alguma versão — normal, resolve com um clique ("Upgrade" / "Sync Now").

## O que já existe

- Projeto Gradle padrão (`build.gradle.kts`, `settings.gradle.kts`, wrapper) com um módulo `app`.
- Kotlin + [Jetpack Compose](https://developer.android.com/jetpack/compose) para a interface.
- Uma tela (`MainActivity.kt`) com o nome do app e um botão "Testar conexão com a API", que chama `GET /health` na API em C# usando OkHttp puro (`network/ApiClient.kt`) — nenhuma biblioteca maior de rede (Retrofit, Ktor...) foi escolhida ainda, de propósito.
- Cores (`ui/theme/Color.kt`) copiadas das mesmas variáveis usadas em `frontend/index.html` (`--brand` e `--accent`), para já nascer com a identidade visual do protótipo web.
- Ícone de app placeholder (um balão de fala simples, em vetor).

## Como abrir

1. Instale o [Android Studio](https://developer.android.com/studio) (ele já vem com o SDK do Android).
2. Abra a pasta `android/` como projeto.
3. Deixe o Gradle sincronizar (primeira vez demora, baixa dependências).
4. Rode num emulador ou celular físico (▶ no Android Studio).

## Testando a conexão com a API

1. Suba a API em C#: `dotnet run --project ../src/RoleDaFala.Api` (veja o [README principal](../README.md)).
2. Rode o app num **emulador** — `ApiClient.BASE_URL_EMULADOR` já aponta para `http://10.0.2.2:5080`, o endereço que o emulador usa para chegar ao `localhost` da máquina que roda ele.
3. Num **celular físico**, troque essa URL pelo IP da máquina na mesma rede Wi-Fi (ex.: `http://192.168.0.10:5080`).
4. Toque em "Testar conexão com a API". Deve aparecer "Conectado à API (status: ok)".

## O que falta (decisões do time, não deste esqueleto)

- Qual biblioteca de rede usar de verdade (OkHttp puro é só para não travar o esqueleto numa decisão que não é minha).
- Todas as telas do app (cadastro, tutor de IA, salas, desafios...) — hoje só existe a tela de teste de conexão.
- Guardar o token JWT devolvido por `/contas/login` e `/contas/registrar` (ver [`../docs/2-Arquitetura.md`](../docs/2-Arquitetura.md#contas-e-autenticação)) entre uma abertura do app e outra.
- Ícone e identidade visual de verdade, no lugar do placeholder.
- Testes (também zerados de propósito no resto do projeto agora — ver o Roadmap).
