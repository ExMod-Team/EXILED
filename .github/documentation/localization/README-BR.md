<h1 align="center">EXILED - EXtended In-runtime Library for External Development</h1>
<div align="center">
    
[<img src="https://img.shields.io/github/actions/workflow/status/ExMod-Team/EXILED/main.yml?style=for-the-badge&logo=githubactions&label=build" alt="CI"/>](https://github.com/ExMod-Team/EXILED/actions/workflows/main.yml/badge.svg?branch=master)
<a href="https://github.com/ExMod-Team/EXILED/releases"><img src="https://img.shields.io/github/v/release/ExMod-Team/EXILED?display_name=tag&style=for-the-badge&logo=gitbook&label=Release" href="https://github.com/ExMod-Team/EXILED/releases" alt="GitHub Releases"></a>
<img src="https://img.shields.io/github/downloads/ExMod-Team/EXILED/total?style=for-the-badge&logo=github" alt="Downloads">
![Github Commits](https://img.shields.io/github/commit-activity/w/ExMod-Team/EXILED/apis-rework?style=for-the-badge&logo=git)
<a href="https://discord.gg/PyUkWTg">
    <img src="https://img.shields.io/discord/656673194693885975?style=for-the-badge&logo=discord" alt="Chat on Discord">
</a>    

</div>

EXILED é um Framework de alto nível para a criação de plug-ins direcionado a servidores de SCP: Secret Laboratory. Ele oferece um sistema de eventos para os desenvolvedores, com o objetivo de manipular, alterar ou implementar suas próprias funcionalidades no jogo.
Todos os eventos do EXILED são feitos com [Harmony](https://harmony.pardeike.net/articles/intro.html), o que significa que não requerem edição direta dos Assemblies/Código Base do servidor para funcionar, permitindo dois benefícios:

 - Todo o código do Framework pode ser publicado e compartilhado livremente, permitindo que os desenvolvedores entendam melhor *como* funciona, além de poderem sugerir adições ou alterações.
 - Todo o código relacionado ao framework é executado fora do assembly do servidor, significando que pequenas atualizações do jogo provavelmente não causarão efeitos colaterais. Isso torna o projeto mais compatível, além de facilitar quando for necessário atualizá-lo.

# Instalação
A instalação do EXILED é bem simples e você pode escolher entre dois tipos: ``Automática`` e ``Manual``.

Na instalação automática, o instalador cuidará de baixar todos os recursos e arquivos para que o EXILED funcione.

Já na manual, você faz o download do ``Exiled.tar.gz`` nos arquivos do release, e há duas pastas dentro. 
``SCP Secret Laboratory`` contém os arquivos necessários para carregar os recursos do EXILED de dentro da pasta ``EXILED``. Com isso em mente, tudo o que você precisa fazer é mover essas duas para o caminho adequado e pronto!

Abaixo entraremos em mais detalhes...

# Windows
> [!IMPORTANT]  
> Verifique se você está conectado no mesmo usuário do Windows que está executando o servidor ou se possui privilégios de administrador antes de executar o Instalador.

### Instalação automática ([mais informações](https://github.com/ExMod-Team/EXILED/blob/master/EXILED/Exiled.Installer/README.md))
  - Baixe **`Exiled.Installer-Win.exe` [aqui](https://github.com/ExMod-Team/EXILED/releases)** (clique em Assets -> clique no instalador)
  - Coloque-o na pasta do seu servidor (Ele precisa estar dentro da pasta de um servidor "dedicado", caso não tenha siga [esse guia](https://techwiki.scpslgame.com/books/server-guides/page/1-how-to-create-a-dedicated-server))
  - Clique duas vezes em **`Exiled.Installer.exe`** ou **[baixe este .bat](https://www.dropbox.com/scl/fi/7yh0r3q0vdn6ic4rhuu3l/install-prerelease.bat?rlkey=99fwjbwy1xg61qgtak0qzb9rd&st=8xs4xks8&dl=1)** e coloque-o na pasta do servidor para instalar o pré-lançamento mais recente
  - Para instalar e obter plug-ins, confira a secção [Instalando plug-ins](https://github.com/ExMod-Team/EXILED/edit/master/.github/documentation/localization/README-BR.md#instala%C3%A7%C3%A3o-manual).

### Instalação manual
  - Baixe o **`Exiled.tar.gz` [aqui](https://github.com/ExMod-Team/EXILED/releases)**
  - Extraia o conteúdo com [7Zip](https://www.7-zip.org/) ou [WinRar](https://www.win-rar.com/download.html?&L=6)
  > [!CAUTION]
  > As pastas a seguir precisam estar em ``C:\Users\%NomeDoUsuário%\AppData\Roaming``, e ***NÃO*** ``C:\Users\%NomeDoUsuário%\AppData\Roaming\SCP Secret Laboratory``.
  - Mova a pasta **``EXILED``** para **`%appdata%`** 
  - Mova **``SCP Secret Laboratory``** para **`%appdata%`**.
    - **Windows 10 e 11**:
      Escreva `%appdata%` na Cortana, no ícone de pesquisa ou na barra do Windows Explorer
    - **Outras versões do Windows**:
      Pressione Win + R e digite `%appdata%`

### Instalando plug-ins
O EXILED agora deve estar instalado e ativo na próxima vez que você iniciar o seu servidor. Observe que o EXILED sozinho não fará quase nada, portanto, certifique-se de obter novos plug-ins em **[nosso servidor do Discord](https://discord.gg/PyUkWTg)!**
- Para instalar um plug-in, basta:
  - Baixar um plug-in da [página de lançamentos *deles*](https://i.imgur.com/u34wgPD.jpg) (**PRECISA ser um `.dll`!**)
  - Mova-o para: ``C:\Users\%NomeDoUsuário%\AppData\Roaming\EXILED\Plugins``

# Linux
> [!IMPORTANT]  
> Certifique-se de executar o instalador como o mesmo usuário (ou root) que executa seus servidores de SCP:SL.

### Instalação automática ([mais informações](https://github.com/ExMod-Team/EXILED/blob/master/EXILED/Exiled.Installer/README.md))
  - Baixe o **`Exiled.Installer-Linux` [aqui](https://github.com/ExMod-Team/EXILED/releases)** (clique em Assets -> baixe o Instalador)
  - Instale-o digitando **`./Exiled.Installer-Linux --path /path/to/server`** ou mova-o diretamente para dentro da pasta do servidor, e digite: **`./Exiled.Installer-Linux`**.
  
> [!NOTE]  
> Se você quiser o último pré-lançamento, adicione a flag **`--pre-releases`**. Exemplo: **`./Exiled.Installer-Linux --pre-releases`**

  - Para instalar e obter plug-ins, confira a secção [Instalando plug-ins](https://github.com/ExMod-Team/EXILED/edit/master/.github/documentation/localization/README-BR.md#instalando-plug-ins-1).

### Instalação manual
  - Baixe o **`Exiled.tar.gz` [aqui](https://github.com/ExMod-Team/EXILED/releases)** (SSH: clique com o botão direito do mouse para copiar o link do `Exiled.tar.gz` e então digite: **`wget (link_para_baixar)`**)
  - Para extraí-lo à sua pasta atual, digite **``tar -xzvf EXILED.tar.gz``**
  
> [!CAUTION]
> As pastas precisam ir para o diretório ``~/.config``, e ***NÃO*** ``~/.config/SCP Secret Laboratory``* 

  - Mova a pasta **`EXILED`** para **``~/.config``**. (SSH: **`mv EXILED ~/.config/`**)
  - Mova a pasta **`SCP Secret Laboratory`** para **``~/.config``**. (SSH: **`mv "SCP Secret Laboratory" ~/.config/`**)

### Instalando plug-ins
O EXILED agora deve estar instalado e ativo na próxima vez que você inicializar seu servidor. Observe que o EXILED sozinho não fará quase nada, portanto, certifique-se de obter novos plug-ins em **[nosso servidor do Discord](https://discord.gg/PyUkWTg)!**
- Para instalar um plug-in, basta:
  - Baixar um plug-in da [página de lançamento *deles*](https://i.imgur.com/u34wgPD.jpg) (**DEVE ser um `.dll`!**)
  - Mova-o para: ``~/.config/EXILED/Plugins`` (se você utiliza SSH como root, procure pela `.config` correta, que estará dentro de `/home/(Usuário do Servidor de SCP)`)

# Configuração
O EXILED por si só oferece algumas opções de configuração.
Todas elas são geradas automaticamente na inicialização do servidor e estão localizadas no arquivo ``~/.config/EXILED/Configs/(PortaDoServidorAqui)-config.yml`` (``%AppData%\EXILED\Configs\(PortaDoServidorAqui)-config.yml`` no Windows).

As configurações dos plug-ins ***NÃO*** estarão no arquivo ``config_gameplay.txt``! 
Em vez disso, você encontrará no arquivo ``~/.config/EXILED/Configs/(porta_do_servidor)-config.yml`` (``%AppData%\EXILED\Configs\(porta_do_servidor)-config.yml`` no Windows).

> [!NOTE]  
> Em versões mais recentes do EXILED, as configs dos plug-ins foram movidas para pastas próprias: ``EXILED\Configs\(nome_do_plugin)``. Você pode mudar esse comportamento 
> editando a configuração do Loader em: ``SCP Secret Laboratory\LabAPI\configs\global\Exiled.Loader`` (ou ``SCP Secret Laboratory\LabAPI\configs\(porta_do_servidor)\Exiled.Loader``)

No entanto, alguns plug-ins podem gerar suas configurações em outros locais por conta própria. Este é simplesmente o local padrão do EXILED para esses arquivos, portanto, consulte o criador do plug-in se houver problemas.

# Para Desenvolvedores

Se você deseja fazer um plug-in com o EXILED, é bem simples. Caso queira ver um tutorial, visite nosso [Manual de Instruções.](GettingStarted-BR.md)

Para tutoriais mais abrangentes e ativamente atualizados, consulte [o site da EXILED](https://exmod-team.github.io/EXILED/).

Mas certifique-se de seguir estas regras ao publicar seus plug-ins:

 - Seu plug-in deve conter uma classe herdada de ``Exiled.API.Features.Plugin<>``, caso contrário, o EXILED não carregará seu plug-in quando o servidor iniciar.
 - Quando um plug-in é carregado, o código dentro do método ``OnEnabled()`` da classe é chamado imediatamente (Dependendo do ``Exiled.API.Features.Plugin<>::PluginPriority``)
 - Se você precisar acessar algo que ainda não foi inicializado antes do carregamento do plug-in, recomendamos simplesmente ouvir o evento ``WaitingForPlayers``. Se por algum motivo você precisar fazer isso antes, coloque o código dentro de um loop ```while (!x)``` onde verifica se a variável/objeto que você precisa não é mais *null* antes de continuar.
 - O EXILED suporta o recarregamento dinâmico de Assemblies de plug-ins no meio da execução. Isso significa que, se você precisar atualizar um plug-in, isso pode ser feito sem reiniciar o servidor, no entanto, se você estiver atualizando um plug-in no meio da execução, o plug-in precisa ser configurado corretamente para suportá-lo, ou você terá um sério problema. Consulte a seção ``Atualizações Dinâmicas`` para mais informações e orientações a seguir.
 - **NÃO** há evento OnUpdate, OnFixedUpdate ou OnLateUpdate no EXILED. Se você precisar, por algum motivo, executar o código com frequência, poderá usar uma corrotina MEC que espera por um quadro, 0.01f, ou usar um segmento de Timing como ``Timing.FixedUpdate``.

### Desativando patches de evento do EXILED
***Atualmente, esta função não está mais implementada.***

### Corrotinas do MEC
Se você não estiver familiarizado com o MEC, este será um guia muito breve e simples para você começar.
As corrotinas do MEC são basicamente métodos temporizados que suportam períodos de espera antes de continuar a execução, sem interromper/suspender a thread principal do jogo.
Elas são seguras para usar com o Unity, ao contrário do threading tradicional. ***NÃO tente criar NOVAS THREADS para interagir com o Unity, isso irá travar o servidor!!!***

Para usar o MEC, você precisará referenciar ``Assembly-CSharp-firstpass.dll`` dos arquivos do servidor e incluir ``using MEC;``.
Exemplo de criação de uma corrotina simples, que se repete com um atraso a cada ciclo:
```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}

public IEnumerator<float> MyCoroutine()
{
    for (;;) //Repete o evento seguinte por tempo indefinido
    {
        Log.Info("Ei, eu sou um ciclo infinito!"); // Usado para reproduzir uma linha nos registros do console/servidor do jogo.
        yield return Timing.WaitForSeconds(5f); //Diz à corrotina para esperar 5 segundos antes de continuar, e quando está no final do ciclo, efetivamente interrompe a repetição do ciclo por 5 segundos.
    }
}
```

É **altamente** recomendável que você pesquise no Google ou pergunte no Discord se não estiver familiarizado com o MEC e quiser aprender mais, obter conselhos ou precisar de ajuda. As perguntas, não importa o quão 'estúpidas' sejam, sempre serão respondidas da maneira mais útil e clara possível. Um bom código é melhor para todos.

### Atualizações Dinâmicas
O EXILED como uma estrutura suporta o recarregamento dinâmico de Assemblies de plug-ins sem precisar reiniciar o servidor.
Por exemplo, apenas com `Exiled.Events` como o único plug-in e depois você deseja adicionar um novo, não será necessário reiniciar o servidor. Você pode simplesmente usar o comando do RemoteAdmin/ServerConsole `reload plugins` para recarregar todos os plug-ins do EXILED, incluindo os novos que não foram carregados antes.

Isso também significa que você pode *atualizar* os plug-ins sem precisar reinicializar totalmente o servidor. No entanto, existem algumas diretrizes que devem ser seguidas pelo desenvolvedor do plug-in para que isso seja realizado corretamente:

***Para Hosters***
 - Se você estiver atualizando um plug-in, certifique-se de que o nome do Assembly não seja o mesmo da versão atual que você instalou (se houver uma). O plug-in deve ser construído pelo desenvolvedor com atualizações dinâmicas em mente para que isso funcione, simplesmente renomear o arquivo não basta.
 - Se o plug-in suporta Atualizações Dinâmicas, certifique-se de que, ao colocar a versão mais recente do plug-in na pasta "Plugins", você também remova a versão mais antiga da pasta, antes de recarregar o EXILED; a falha em garantir isso resultará em muitos problemas indesejados.
 - Quaisquer problemas decorrentes da Atualização Dinâmica de um plug-in são de sua exclusiva responsabilidade e do desenvolvedor do plug-in em questão. Embora o EXILED suporte e incentive totalmente as Atualizações Dinâmicas, a única maneira de isso falhar ou dar errado é se o dono do servidor ou o desenvolvedor do plug-in fizer algo errado. Verifique três vezes se tudo foi feito corretamente por ambas as partes antes de relatar um erro aos desenvolvedores da EXILED em relação às Atualizações Dinâmicas.

 ***Para Desenvolvedores***

 - Os plug-ins que desejam oferecer suporte à Atualização Dinâmica precisam cancelar a assinatura de todos os eventos aos quais estão conectados quando são desativados ou recarregados.
 - Os plug-ins que possuem patches personalizados do Harmony devem usar algum tipo de variável mutável no nome da instância do Harmony e devem usar UnPatchAll() em sua instância do Harmony quando o plug-in for desativado ou recarregado.
 - Quaisquer corrotinas iniciadas pelo plug-in em ``OnEnabled()`` também devem ser eliminadas quando o plug-in for desativado ou recarregado.

Tudo isso pode ser realizado nos métodos ``OnReloaded()`` ou ``OnDisabled()`` na classe do plug-in. Quando o EXILED recarrega os plug-ins, ele designa OnDisabled(), então ``OnReloaded()``, então ele carregará nos novos Assemblies, e então executará ``OnEnabled()``.

Observe que eu disse *novos* Assemblies. Se você substituir um Assembly por outro com o mesmo nome, ele ***NÃO*** será atualizado. Isso se deve ao GAC (Global Assembly Cache), se você tentar 'carregar' um Assembly que já está no cache, ele sempre usará o Assembly em cache.
Por esse motivo, se o seu plug-in oferecer suporte a Atualizações Dinâmicas, você deverá criar cada versão com um nome de Assembly diferente nas opções de compilação (renomear o arquivo não funcionará). Além disso, como o Assembly antigo não é "destruído" quando não é mais necessário, se você não cancelar a assinatura de eventos, desfazer o patch de sua instância de Harmony, eliminar corrotinas, etc., esse código continuará a ser executado, bem como o código da nova versão.
Esta é uma situação muito ruim para se deixar acontecer.

Como tal, os plug-ins que oferecem suporte a Atualizações Dinâmicas ***DEVEM*** seguir estas diretrizes ou serão removidos do servidor do Discord devido ao risco potencial para os donos de servidor.

Mas nem todo plug-in tem de oferecer suporte a Atualizações Dinâmicas. Se você não pretende oferecer suporte a Atualizações Dinâmicas, tudo bem, simplesmente não altere o nome do Assembly do seu plug-in ao criar uma nova versão e não precisará se preocupar com nada disso, apenas certifique-se de que os donos de servidor saibam que eles precisarão reinicializar completamente seus servidores para atualizar seu plug-in.

**Tradução para o português feita por**: *Unbistrackted* e *Firething*
