# Tutorial do EXILED
*(Escrito por [KadeDev](https://github.com/KadeDev) para a comunidade, revisado e traduzido por [Unbistrackted](https://github.com/Unbistrackted) e [Firething](https://github.com/Firething))*

## Manual de Instruções
### Introdução
Como dito anteriormente, o EXILED é um framework de alto nível que nos permite chamar funções do jogo sem ter nenhum tipo de complicação ou quase nenhuma perda de performance.

Isso permite que o projeto seja atualizado de forma mais simples, sem precisar que desenvolvedores atualizem seus plugins toda vez que o jogo atualizar. (Isso se não houver códigos que foram alterados/tornados obsoletos em versões majors do EXILED)

O guia a seguir irá te ensinar o básico de como criar seu primeiro plugin!

### Guia
O [Plug-in de Exemplo](https://github.com/ExMod-Team/EXILED/tree/master/EXILED/Exiled.Example) mostra o que são eventos e como criar eles de forma correta. Usar esse exemplo ajudará você a aprender a como usar o Exiled apropriadamente. Dentro desse existem elementos que são importantes, portanto acompanhe o código durante o tutorial.

#### ``OnEnable`` e ``OnDisable``+ Atualizações Dinâmicas
O EXILED possuí um comando chamado **Reload**, que recarrega todos os plug-ins instalados. 

Ele funciona desativando o plugin e ativando-o novamente, além de chamar a função ``OnReload`` que entraremos em detalhes abaixo.

Lembrando que toda variável, evento, corrotina, etc. *deve* ser atribuído ou criado quando o plugin é ativado e anulada quando o mesmo é desativado.

> [!IMPORTANT]  
> Você **DEVE** usar o método ``OnEnable`` para ativar o Plug-in, e ``OnDisable`` desativa-lo. 

Mas talvez você deve estar se perguntando: "Mas então para que serve o ``OnReload``?" Essa função tem como objetivo recarregar as variáveis estáticas de dentro do seu plugin. Então você poderia fazer algo assim:
```csharp
public static int StaticCount = 0;
public int counter = 0;

public override void OnEnable()
{
    counter = StaticCount;
    counter++;
    Log.Info(counter);
}

public override void OnDisable()
{
    counter++;
    Log.Info(counter);
}

public override void OnReload()
{
    StaticCount = counter;
}
```

E o resultado seria:
```bash
# O servidor é iniciado...
# OnEnable é chamado.
1
# Comando Reload é executado por alguém...
# OnDisable é chamado.
2
# OnReload é chamado.
"counter" é guardado dentro de "StaticCount"
# E então OnEnabled é chamado novamente.
3

```
Sem fazer isso, teria apenas mostrado no console ``1`` e então para o ``2`` novamente.

### Jogadores + Eventos
Agora que entendemos como os métodos de entrada/inicializaçãos dos plug-ins funcionam, podemos focar em como interagir com jogadores por meio de eventos!

Um evento é uma forma do jogo notificar seu plug-in quando algo acontece, por exemplo quando um jogador entrar, tomar dano, morrer, etc.

> [!IMPORTANT]  
> Você **PRECISA** referenciar o arquivo `Exiled.Events.dll` para que você consiga usar os eventos. (Ou apenas baixe o pacote [Nuget do Exiled](https://www.nuget.org/packages/ExMod.Exiled)!)

Para começar a ouvir um evento, iremos utilizar uma nova classe chamada "EventHandlers", que irá gerenciar nossos eventos.

Na classe EventHandlers:

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {
        // Códigos 1
        // Códigos 2
        // Códigos 3
    }
}
```


E depois nós podemos referenciá-lo no ``OnEnable`` e ``OnDisable`` desse jeito:

`MainClass.cs`
```csharp
using Player = Exiled.Events.Handlers.Player;

public EventHandlers EventHandler;

public override void OnEnable()
{
    EventHandler = new EventHandlers();
    // += significa que você vai estar se atribuindo ao evento, que nesse caso você vai ouvir toda vez que ele for chamado.
    Player.Verified += EventHandler.PlayerVerified;
}

public override void OnDisable()
{
    // Precisamos desatribuir o evento e depois, anular o gerenciador de eventos.
    // A linha abaixo deve ser repetida para cada evento.
    Player.Verified -= EventHandler.PlayerVerified;
    EventHandler = null;
}
```

Agora toda vez que um jogador é autenticado após entrar no servidor podemos executar nosso código customizado! É importante destacar que todos eventos têm diferentes argumentos, e cada tipo tem propriedades diferentes associadas.

O EXILED já fornece uma função para enviar um broadcast, então a usaremos em nosso exemplo:

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {
        ev.Player.Broadcast(5, "<color=lime>Bem-vindo(a) ao meu servidor!</color>");
    }
}
```

Outro exemplo seria um evento que desliga as Teslas para todos os MTFs. (Incluindo guardas)

`MainClass.cs`
```csharp
using Player = Exiled.Events.Handlers.Player;

public EventHandlers EventHandler;

public override void OnEnable()
{
    EventHandler = new EventHandlers();
    Player.TriggeringTesla += EventHandler.TriggeringTesla;
}

public override void OnDisable()
{
    // Não se esqueça, eventos devem ser desatribuídos e anulados nesse metódo!
    Player.TriggeringTesla -= EventHandler.TriggeringTesla;
    EventHandler = null;
}
```

E na classe EventHandlers.

`EventHandlers.cs`
```csharp
public class EventHandlers
{
    public void TriggeringTesla(TriggeringTeslaEventArgs ev)
    {
        // Desativa o evento para jogadores da equipe da Fundação.
        // Isso pode ser feito ao verificar o lado da classe (Player::Role.Side) do jogador.
        if (ev.Player.Role.Side == Side.Mtf) {
            // Desative o acionamento da Tesla mudando o valor de 'ev.IsTriggerable' para 'false'.
            // Lembrando que isso desabilita para todos os MTFs, incluindo Guardas! 
            ev.IsTriggerable = false;
        }
    }
}
```


### Configurações
Grande partes dos plug-ins precisam de configurações, isso permite que os donos de servidores modifiquem-os livremente.

Primeiro crie uma classe chmada `Config` e mude a herança do seu plug-in de `Plugin<>` para `Plugin<Config>`

Agora você precisa fazer essa classe herdar `IConfig`, e depois implementar o contrato dela criando `IsEnabled` e `Debug`. Sua classe de Configuração agora deve se assemelhar a isso:

```csharp
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true; // Se você não colocar "= true", o seu plugin não sera habilitado quando o servidor iniciar!
        public bool Debug { get; set; }
    }
```

Você pode adicionar qualquer opção de configuração e referenciá-la assim: 

`Config.cs`
```csharp
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
        public string TextThatINeed { get; set; } = "Texto para testes";
    }
```

> [!NOTE] 
> Você não precisa verificar se `IsEnabled == true` ou não, o Loader do Exiled já faz isso automaticamente.

`MainClass.cs`

```csharp
   public override OnEnabled()
   {
        Log.Info(Config.TextThatINeed);
   }
```

Pronto, você está preparado para fazer Plug-ins usando o Exiled! 

### E agora?
Se você quiser mais informações, entre no nosso [Servidor do Discord!](https://discord.gg/PyUkWTg)

Nós temos um canal de recursos chamado ``#resources`` que você pode considerar útil, assim como vários outros desenvolvedores que iram te ajudar a desenvolver seus plug-ins!

Ou você poderia ler sobre todos os eventos que nós temos! Bem [aqui](https://github.com/ExMod-Team/EXILED/tree/master/EXILED/Exiled.Events/EventArgs)!
