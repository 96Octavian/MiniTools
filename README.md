#MiniTools

L'obiettivo finale finalissimo, molto lontano, è una cosa tipo un Raspberry che appena avviato legga la configurazione data dall'utente su un file (su una chiavetta USB) che può contenere alcune delle seguenti cose: parametri per la connessione a Wi-Fi e server, playlist, canzoni. 
La configurazione andrebbe fornita su USB esterna dato che il SO del Raspberry sta su una schedina SD, che se spenta e accesa molte volte si corrompe velocemente: per evitare questo la schedina andrà messa in read-only, che significa che una volta impostata non può scrivere nulla. Perciò file da scrivere vanno su una memoria esterna. 
- Se non c’è né configurazione Wi-Fi né configurazione playlist, si spegne;
- Se c’è il Wi-Fi si connette, e fa partire anche un server locale: il server locale può o servire per mostrare una pagina di telecomando, o solo per esporre delle API che possono venire contattate da una app telecomando che in questo caso sarebbe separata. Se c'è il Wifi non si spegne mai, dato che col telecomando si possono fornire configurationi aggiuntive;
- Se c'è una playlist, questa può contenere sia percorsi di rete, che significa che bisogna riprodurre canzoni dal server, sia percorsi locali, che significa che bisogna riprodurre canzoni sulla chiavetta. Se c'è una playlist non si spegne, anche se nessun elemento è raggiungibile. Ovviamente si saltano gli elementi non raggiungibili senza problemi. In caso di fine della lista di canzoni, si ripete la playlist e si rimane in attesa magari monitorando cambi al file playlist periodicamente, per riprendere nel caso in cui venisse modificato qualcosa;

Bisogna decidere:
- se il player è un singolo programma monolitico che ingloba tutto (riproduttore, server, telecomando) oppure più pezzi separati;
- quale tecnologia usare in ogni caso;
- se fornire anche una interfaccia grafica di base integrata nel caso il Raspberry avesse uno schermo;

In ogni caso bisogna imparare, a step successivi, come implementare varie cose:
- come fare un server per l'app telecomando, sia esso API server o normale web server;
- eventualmente come sviluppare l'app telecomando esterna;
- eventualmente nel caso in cui il player fosse costituito da più programmi (server+player+telecomando) come gestire la comunicazione tra le varie parti;
- come salvare le configurazioni;
- come connettersi al Wi-Fi programmaticamente, se esistono API decenti ad esempio in Java (pare non esistano su Linux per .NET);

La struttura finale vedrà un server centrale di musica che è singolo, e comunica solo attraverso Web API su richiesta dei molteplici player, che sono controllati dal file di configurazione e da molteplici telecomandi (la possibilitò che un player si connetta a più server contemporaneamente non mi sembra interessante), e capaci di riprodurre musica solo localmente, leggendo file locali e/o remoti. 

Passi interessanti possono essere: 
- cominciare con un player che abbia hard-coded la connessione e dà per scontata la presenza del server, senza lettura di file di configurazione né tantomeno riproduzione di musica locale;
- una volta funzionante, lettura dei parametri di connessione da file (sostanzialmente indirizzo del server musicale, alla fine anche parametri del Wi-FI);
- una volta funzionante, lettura dei file musicali locali;
- parallelamente la creazine dell'interfaccia del telecomando, dopo aver deciso se servirlo come webapp dal player o come app separata;

Tutto questo senza curarsi della configurazione su un Raspebrry in modalità read-only e senza gestione della connessione Wi-Fi. Questi passaggi saranno implementati in ultima istanza, come esercizio per fornire un prodotto completo e autonomo
