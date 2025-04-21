# Exiled Low-Level Documentation
**(Scritto da [KadeDev](https://github.com/KadeDev) per la community) (tradotto da [Inzi](https://github.com/InziDeveloperMode))**

## Getting Started
### Intro
Exiled è un'API di basso livello, il che significa che è possibile richiamare funzioni dal gioco senza bisogno di un mucchio di API bloatware.

Ciò consente di aggiornare Exiled con estrema facilità e di aggiornarlo anche prima che l'aggiornamento arrivi al gioco.

Inoltre, consente agli sviluppatori di plugin di non dover modificare il proprio codice dopo ogni aggiornamento di Exiled o di SCP:SL. In effetti, non devono nemmeno aggiornare i loro plugin!

Questa documentazione vi mostrerà le basi della creazione di un plugin per Exiled. Da qui potrete iniziare a mostrare al mondo quali cose creative potete realizzare con questo framework!

### Plugin di esempio
Il [Plugin di esempio](https://github.com/ExMod-Team/EXILED/tree/master/EXILED/Exiled.Example) è un semplice plugin che mostra gli eventi e come realizzarli correttamente. L'uso di questo esempio vi aiuterà a imparare a usare correttamente Exiled. Ci sono un paio di cose importanti in questo plugin, e ne parliamo
#### On Enable + On Disable Dynamic Updates
Exiled è un framework che ha un comando **Reload** che può essere usato per ricaricare tutti i plugin e ottenerne di nuovi. Ciò significa che i plugin devono essere **aggiornabili dinamicamente**. Questo significa che ogni variabile, evento, coroutine, ecc. *deve* essere assegnata quando è abilitata e annullata quando è disabilitata. Il metodo **On Enable** dovrebbe abilitare tutto e il metodo **On Disable** dovrebbe disabilitare tutto. Ma ci si potrebbe chiedere: e **On Reload**? Questo void ha lo scopo di trasportare le variabili statiche, in quanto ogni costante statica creata non sarà cancellata. Quindi si potrebbe fare qualcosa del genere:
```csharp
public static int StaticCount = 0;
public int counter = 0;

public override void OnEnable()
{
    counter = StaticCount;
    counter++;
    Info(counter);
}

public override void OnDisable()
{
    counter++;
    Info(counter);
}

public override void OnReload()
{
    StaticCount = counter;
}
```

E l'output sarebbe:
```bash
# On enable fires
1
# Reload command
# On Disable fires
2
# On Reload fires
# On Enable fires again
3

```
(Ovviamente escludendo tutto ciò che non sia le risposte effettive).
Senza questo, si sarebbe semplicemente passati a 1 e poi di nuovo a 2.

### Giocatori + Eventi
Ora che abbiamo finito di rendere i nostri plugin **dinamicamente aggiornabili** possiamo concentrarci sul tentativo di interagire con i giocatori con gli eventi!

Un evento è piuttosto interessante: permette a SCP:SL di comunicare con Exiled e poi con Exiled a tutti i plugin!

Potete ascoltare gli eventi per il vostro plugin aggiungendo questo in cima al file sorgente del vostro plugin principale:

```csharp
using EXILED;
```
E poi si deve fare riferimento al file `Exiled.Events.dll` per ottenere effettivamente gli eventi.

Per fare riferimento a un evento, utilizzeremo una nuova classe creata, chiamata “EventHandlers”. Il gestore di eventi non è fornito di default; è necessario crearlo.


Possiamo fare riferimento ad esso nelle void OnEnable e OnDisable in questo modo:

`MainClass.cs`
```csharp
using Player = Exiled.Events.Handlers.Player;

public EventHandlers EventHandler;

public override OnEnable()
{
    // Registriamo la classe del gestore di eventi. E aggiungere l'evento,
    // all'ascoltatore di eventi EXILED_Events, in modo da ricevere l'evento.
    EventHandler = new EventHandlers();
    Player.Verified += EventHandler.PlayerVerified;
}

public override OnDisable()
{
    // Rendiamolo aggiornabile dinamicamente.
    // Lo facciamo rimuovendo l'ascoltatore dell'evento e poi annullando il gestore dell'evento.
    // Questo processo deve essere ripetuto per ogni evento.
    Player.Verified -= EventHandler.PlayerVerified;
    EventHandler = null;
}
```


E nella classe EventHandlers si fa:

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {

    }
}
```
Ora abbiamo preso un evento player verified, che si attiva quando un giocatore viene autenticato dopo essersi unito al server! È importante notare che ogni evento ha diversi argomenti, e ogni tipo di argomento ha diverse proprietà associate.

EXILED fornisce già una funzione di broadcast, quindi usiamola nel nostro evento:

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {
        ev.Player.Broadcast(5, "<color=lime>Benvenuti nel mio server cool!</color>");
    }
}
```

Come detto in precedenza, ogni evento ha argomenti diversi. Di seguito è riportato un evento diverso che disattiva i tesla gate per i giocatori della NineTailedFox.

`MainClass.cs`
```csharp
using Player = Exiled.Events.Handlers.Player;

public EventHandlers EventHandler;

public override OnEnable()
{
    EventHandler = new EventHandlers();
    Player.TriggeringTesla += EventHandler.TriggeringTesla;
}

public override OnDisable()
{
    // Non dimenticate che gli eventi devono essere disconnessi e annullati con il metodo disable.
    Player.TriggeringTesla -= EventHandler.TriggeringTesla;
    EventHandler = null;
}
```

E nella classe EventHandlers.

`EventHandlers.cs`
```csharp
public class EventHandlers
{
    public void TriggeringTesla(TriggeringTeslaEventArgs ev)
    {
        // Disattivare l'evento per i giocatori del personale della fondazione.
        // Questo può essere fatto controllando il lato del giocatore.
        if (ev.Player.Role.Side == Side.Mtf) {
            // Disattiva il trigger Tesla impostando ev.IsTriggerable su false.
            // I giocatori che faranno parte della MTF non attiveranno più i Tesla Gate.
            ev.IsTriggerable = false;
        }
    }
}
```


### Configs
La maggior parte dei plugin di Exiled contiene delle configurazioni. Le configurazioni permettono ai manutentori del server di modificare i plugin a loro piacimento, anche se questo è limitato alla configurazione fornita dallo sviluppatore del plugin.

Per prima cosa, creare una classe `config.cs` e cambiare l'ereditarietà dei plugin da `Plugin<>` a `Plugin<Config>`.

Ora è necessario fare in modo che la configurazione erediti da `IConfig`. Dopo aver ereditato da `IConfig`, aggiungere alla classe una proprietà intitolata `IsEnabled` e `Debug`. La classe Config dovrebbe ora avere questo aspetto:

```csharp
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
    }
```

È possibile aggiungere qualsiasi opzione di configurazione e fare riferimento ad essa in questo modo:

`Config.cs`
```csharp
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
        public string TextThatINeed { get; set; } = "this is the default";
    }
```

`MainClass.cs`
```csharp
   public override OnEnabled()
   {
        Log.Info(Config.TextThatINeed);
   }
```

E poi congratulazioni! Avete creato il vostro primo plugin Exiled! È importante notare che tutti i plugin **devono** avere una configurazione IsEnabled. Questa configurazione consente ai proprietari del server di attivare e disattivare il plugin a loro piacimento. La configurazione IsEnabled sarà letta dal Loader di Exiled (il vostro plugin non ha bisogno di controllare se `IsEnabled == true` o meno).

### E ora?
Se volete maggiori informazioni dovreste unirvi al nostro [discord!](https://discord.gg/PyUkWTg)

Abbiamo un canale #resources che potreste trovare utile, così come collaboratori di Exiled e sviluppatori di plugin che sarebbero disposti ad assistervi nella creazione dei vostri plugin.

Oppure puoi leggere tutti gli eventi che abbiamo! Se vuoi darci un'occhiata [qui!](https://github.com/ExMod-Team/EXILED/tree/master/EXILED/Exiled.Events/EventArgs)
