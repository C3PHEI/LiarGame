namespace LiarGame;

public class Gamelogik
{
    //Klassenvariable, kann überall verwendet werden 
    private static readonly List<string> CardStackList = new();
    private static string _gameCardIcone = null!;
    
    private static int _player1Health;
    private static int _player2Health;
    
    private static int _player1HitNumber;
    private static int _player2HitNumber;
    
    public static void DoGameLogik()
    {
        int countCard = GetCardCount();

        List<string> player1Cards = DoCardShuffleDistribution(countCard);
        List<string> player2Cards = DoCardShuffleDistribution(countCard);
        
        SetPlayerHitNumber(out _player1HitNumber, out _player2HitNumber);
        
        GetCardGame(out _gameCardIcone);
        Console.WriteLine($"\nDAS IST EIN {_gameCardIcone} SPIEL!!!");

        DoCardExchange(player1Cards, player2Cards, countCard);
        
        //Ausgabe des Stapels 
        Console.WriteLine("\nDas ist der aktuelle Stapel");
        Console.WriteLine(string.Join(" - ", CardStackList));
        
        //Zeigen der HitNumber und der aktuellen lebens nach 1 interation
        Console.WriteLine("\n");
        Console.WriteLine("p1 Number " + _player1HitNumber + " : p2 Number " + _player2HitNumber);
        Console.WriteLine("p1 Hits IG " + _player1Health + " : p2 Hits IG " + _player2Health);

        if (_player1Health > _player2Health)
        {
            Console.WriteLine("\nSpieler 2 hat gewonnen, ohne das einer gestorben ist");
        }
        else if (_player1Health < _player2Health)
        {
            Console.WriteLine("\nSpieler 1 hat gewonnen, ohne das einer gestorben ist");
        }
        else if (_player1Health == _player2Health)
        {
            Console.WriteLine($"\nEs steht gleichstand bei {_player1Health} Punkt/Punkten");
        }

        Console.ReadKey();
    }

    private static void DoCardExchange(List<string> player1Cards, List<string> player2Cards, int countCard)
    {
        for (int i = 0; i < countCard; i++)
        {
            //Player 1 moves 
            //Frage kann nicht beim ersten Zug gestellt werde
            var playerPlaying = 1;
            bool playerLie;
            int playerGainHealth;
            if (i >= 1)
            {
                //TODO Bei aller letzten karte wird nicht gefragt ob es eine lüge ist oder nicht
                //liegt daran das die frage am anfang der iteration anfängt 
                Console.WriteLine("\nSpieler 1, denkst du das Spieler 2 gelogen hat ?(y/n)");
                CheckPlayerLie(CardStackList, _gameCardIcone, out playerLie);
                playerGainHealth = ManageLieInteraction(playerLie, playerPlaying); //TODO Nachricht die diese Methode schreibt, wird mit zeile unten dran direkt gelöscht 
                
                Console.Clear();
                Console.WriteLine($"\nDAS IST EIN {_gameCardIcone} SPIEL!!!");
                
                CheckIfDeadOrNot(playerGainHealth);
            }
            Console.WriteLine("\nSpieler 1, welche von diesen Karten möchtest du setzen?");
            Console.WriteLine(string.Join(" - ", player1Cards));

            //Wählen einer karten aus dem deck von Player 1
            GetCardChosenP1(player1Cards, out var cardChosen);
            Console.Clear();
            Console.WriteLine($"DAS IST EIN {_gameCardIcone} SPIEL!!!");

            //Die gewählte Karte wird dem Stapel hinzugefügt
            AddCardStack(player1Cards[cardChosen], CardStackList);

            //Die gewählte Karte wird dem Player 1 entzogen 
            player1Cards.RemoveAt(cardChosen);
            //Console.WriteLine(string.Join(" - ", player1Cards));

            //Player 2 wird gefragt ob der der spiel zug vor ihm eine Lüge wahr 
            playerPlaying = 2;
            Console.WriteLine("\nSpieler 2, denkst du das spieler 1 gelogen hat ?(y/n)");
            CheckPlayerLie(CardStackList, _gameCardIcone, out playerLie);
            playerGainHealth = ManageLieInteraction(playerLie, playerPlaying); //TODO Nachricht die diese Methode schreibt, wird mit zeile unten dran direkt gelöscht  
            
            Console.Clear();
            Console.WriteLine($"\nDAS IST EIN {_gameCardIcone} SPIEL!!!");
            
            CheckIfDeadOrNot(playerGainHealth);

            Console.WriteLine("\nSpieler 2, welche von diesen Karten möchtest du setzen?");
            Console.WriteLine(string.Join(" - ", player2Cards));

            //Wählen einer karten aus dem deck von Player 2
            GetCardChosenP2(player2Cards, out cardChosen);
            Console.Clear();
            Console.WriteLine($"DAS IST EIN {_gameCardIcone} SPIEL!!!");

            //Die gewählte Karte wird dem Stapel hinzugefügt
            AddCardStack(player2Cards[cardChosen], CardStackList);

            //Die gewählte Karte wird dem Player 2 entzogen 
            player2Cards.RemoveAt(cardChosen);
            //Console.WriteLine(string.Join(" - ", player2Cards));
        }
    }

    //playerPlaying = 1 = player 1
    //playerPlaying = 2 = player 2
    
    //playerLie ist ein checker ob der Spieler davor wirklich gelogen hat (true = ja Spieler davor hat gelogen / false = nein Spieler davor hat nicht gelogen)
    //plaPlaying ist ein checker wer gerade Spielt (1 / 2)
    //inputLie ist die entscheidung ob der Spieler davor gelogen hat oder nicht (true = Spieler der spielt meint er lugt / false = Spieler lässt die karte durch gehen)
    private static int ManageLieInteraction(in bool playerLie, in int playerPlaying)
    {
        ValidatingInputLie(out var inputLie);
        if ((playerPlaying == 1 || playerPlaying == 2) && !inputLie)
        {
            Console.WriteLine("\nDu hast gesagt, dass er nicht lügt");
            return 0;
        }
        else if (playerPlaying == 1 && inputLie)
        {
            if (playerLie)
            {
                Console.WriteLine("\nDu hast gesagt, dass Spieler 2 lügt und es stimmt!");
                _player2Health++;
                return 2;
            }
            else
            {
                Console.WriteLine("\nDu hast gesagt, dass Spieler 2 lügt und es stimmt NICHT!");
                _player1Health++;
                return 1;
            }
        }
        else if (playerPlaying == 2 && inputLie)
        {
            if (playerLie)
            {
                Console.WriteLine("\nDu hast gesagt, dass Spieler 1 lügt und es stimmt!");
                _player1Health++;
                return 1;
            }
            else
            {
                Console.WriteLine("\nDu hast gesagt, dass Spieler 1 lügt und es stimmt NICHT!");
                _player2Health++;
                return 2;
            }
        }
        return 0;
    }


    //TODO
    private static void CheckIfDeadOrNot(in int playerGainHealth)
    {
        if (playerGainHealth == 1 && _player1Health >= _player1HitNumber)
        {
            Thread.Sleep(1000);
            Console.WriteLine("\nSpieler 1 muss sich jetzt anschiessen");
            Thread.Sleep(1000);
            Console.WriteLine("...");
            Thread.Sleep(1000);
            Console.WriteLine("..");
            Thread.Sleep(1000);
            Console.WriteLine(".");
            Thread.Sleep(1000);
            Console.WriteLine("Spieler 1 ist gestorben\n");
            Console.WriteLine("\nGut gespielt Spieler 2 DU HAST GEWONNEN!!!");
            Environment.Exit(0);
        }
        else if(playerGainHealth == 1)
        {
            Thread.Sleep(1000);
            Console.WriteLine("\nSpieler 1 muss sich jetzt anschiessen");
            Thread.Sleep(1000);
            Console.WriteLine("...");
            Thread.Sleep(1000);
            Console.WriteLine("..");
            Thread.Sleep(1000);
            Console.WriteLine(".");
            Thread.Sleep(1000);
            Console.WriteLine($"Spieler 1 ist bei {_player1Health} Schuss/Schüsse");
        }

        if (playerGainHealth == 2 && _player2Health >= _player2HitNumber)
        {
            Thread.Sleep(1000);
            Console.WriteLine("\nSpieler 2 muss sich jetzt anschiessen");
            Thread.Sleep(1000);
            Console.WriteLine("...");
            Thread.Sleep(1000);
            Console.WriteLine("..");
            Thread.Sleep(1000);
            Console.WriteLine(".");
            Thread.Sleep(1000);
            Console.WriteLine("Spieler 2 ist gestorben\n");
            Console.WriteLine("\nGut gespielt Spieler 1 DU HAST GEWONNEN!!!");
            Environment.Exit(0);
        }
        else if(playerGainHealth ==2)
        {
            Thread.Sleep(1000);
            Console.WriteLine("\nSpieler 2 muss sich jetzt anschiessen");
            Thread.Sleep(1000);
            Console.WriteLine("...");
            Thread.Sleep(1000);
            Console.WriteLine("..");
            Thread.Sleep(1000);
            Console.WriteLine(".");
            Thread.Sleep(1000);
            Console.WriteLine($"Spieler 2 ist bei {_player2Health} Schuss/Schüsse");
        }
    }

    private static void ValidatingInputLie(out bool lieOrNotAsk)
    {
        while (true)
        {
            var inputLieOrNot = Console.ReadLine()?.ToLower();

            if (inputLieOrNot == "y")
            {
                lieOrNotAsk = true;
                break; // Beende die Schleife
            }
            else if (inputLieOrNot == "n")
            {
                lieOrNotAsk = false;
                break; // Beende die Schleife
            }
            else
            {
                Console.WriteLine("Ungültige Eingabe. Bitte gib 'y' oder 'n' ein.");
            }
        }
    }


    private static void SetPlayerHitNumber(out int player1HitNumber, out int player2HitNumber)
    {
        Random random = new();
        player1HitNumber = random.Next(1, 7);
        player2HitNumber = random.Next(1, 7);
    }

    //Methode funktioniert nur wenn 1 Karte platziert wurde
    private static void CheckPlayerLie(List<string> cardStackList, string gameCardIcone, out bool playerLie)
    {
        string lastEntry = cardStackList[^1];
        if (gameCardIcone == lastEntry || lastEntry == "J")
        {
            //False er hat nicht gelogen
            playerLie = false;
        }
        else
        {
            //Er hat gelogen
            playerLie = true;
        }
    }

    private static void AddCardStack(string cardStackAdd, List<string> cardStackList)
    {
        cardStackList.Add(cardStackAdd);
    }

    private static void GetCardChosenP1(List<string> player1Cards, out int cardChosen)
    {
        cardChosen = 0;
        bool playerChoice = false;

        while (!playerChoice)
        {
            string? inputCardChosen = Console.ReadLine();

            if (int.TryParse(inputCardChosen, out cardChosen) && cardChosen >= 1 && cardChosen <= player1Cards.Count)
            {
                playerChoice = true;
            }
            else
            {
                Console.WriteLine("Die Eingabe ist ungültig.");
            }
        }
        cardChosen--;
    }

    private static void GetCardChosenP2(List<string> player2Cards, out int cardChosen)
    {
        cardChosen = 0;
        bool playerChoice = false;

        while (!playerChoice)
        {
            string? inputCardChosen = Console.ReadLine();

            if (int.TryParse(inputCardChosen, out cardChosen) && cardChosen >= 1 && cardChosen <= player2Cards.Count)
            {
                playerChoice = true;
            }
            else
            {
                Console.WriteLine("Die Eingabe ist ungültig.");
            }
        }
        cardChosen--;
    }

    private static int GetCardCount()
    {
        int countCard = 0;
        bool isValidInput = false;

        while (!isValidInput)
        {
            Console.WriteLine("Wie viele Karten sollen die 2 Spieler haben (3-5)?");
            string? inputNumberCard = Console.ReadLine();

            if (int.TryParse(inputNumberCard, out countCard) && countCard >= 3 && countCard <= 5)
            {
                isValidInput = true;
            }
            else
            {
                Console.WriteLine("Ungültige Eingabe. Bitte gib eine Zahl zwischen 3 und 5 ein.");
            }
        }
        return countCard;
    }

    private static void GetCardGame(out string cardGame)
    {
        cardGame = null!;
        Random random = new Random();
        int cCount = random.Next(1, 5);

        switch (cCount)
        {
            case 1:
                cardGame = "B";
                break;
            case 2:
                cardGame = "Q";
                break;
            case 3:
                cardGame = "K";
                break;
            case 4:
                cardGame = "A";
                break;
        }
    }

    private static List<string> DoCardShuffleDistribution(int countCard)
    {
        List<string> cardShuffled = new();
        Random random = new Random();

        for (int i = 0; i < countCard; i++)
        {
            int rCount = random.Next(1, 6);
            switch (rCount)
            {
                case 1:
                    cardShuffled.Add("B");
                    break;
                case 2:
                    cardShuffled.Add("Q");
                    break;
                case 3:
                    cardShuffled.Add("K");
                    break;
                case 4:
                    cardShuffled.Add("A");
                    break;
                case 5:
                    cardShuffled.Add("J");
                    break;
            }
        }
        return cardShuffled;
    }
}