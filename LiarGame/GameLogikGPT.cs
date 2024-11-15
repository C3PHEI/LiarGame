using System;
using System.Collections.Generic;

namespace LiarGame
{
    public class Player
    {
        public int Health { get; set; }
        public int HitNumber { get; set; }
        public List<string> Cards { get; set; }

        public Player()
        {
            Health = 0;
            Cards = new List<string>();
        }

        public override string ToString()
        {
            return $"Player with Health: {Health}, HitNumber: {HitNumber}";
        }
    }

    public class GameLogic
    {
        private readonly List<string> _cardStack = new();
        private string _gameCardIcon;
        private Player _player1;
        private Player _player2;

        public void StartGame()
        {
            int cardCount = GetCardCount();

            _player1 = new Player();
            _player2 = new Player();

            _player1.Cards = DistributeCards(cardCount);
            _player2.Cards = DistributeCards(cardCount);

            SetPlayerHitNumbers(_player1, _player2);

            _gameCardIcon = GetRandomCardIcon();
            Console.WriteLine($"\nDas ist ein {_gameCardIcon}-Spiel!");

            PlayGameRounds(cardCount);

            Console.ReadKey();
        }

        private void PlayGameRounds(int cardCount)
        {
            for (int i = 0; i < cardCount; i++)
            {
                // Zug von Spieler 1
                if (i >= 1)
                {
                    Console.WriteLine("\nSpieler 1, denkst du, dass Spieler 2 gelogen hat? (j/n)");
                    bool accusation = GetPlayerAccusation();
                    bool playerLied = CheckIfPlayerLied();
                    ManageLieInteraction(_player1, _player2, accusation, playerLied);
                }

                DisplayPlayerStats();

                Console.WriteLine("\nSpieler 1, welche Karte möchtest du spielen?");
                DisplayPlayerCards(_player1);
                int cardChosen = GetCardChosen(_player1);
                _cardStack.Add(_player1.Cards[cardChosen]);
                _player1.Cards.RemoveAt(cardChosen);

                // Zug von Spieler 2
                Console.WriteLine("\nSpieler 2, denkst du, dass Spieler 1 gelogen hat? (j/n)");
                bool accusation2 = GetPlayerAccusation();
                bool playerLied2 = CheckIfPlayerLied();
                ManageLieInteraction(_player2, _player1, accusation2, playerLied2);

                DisplayPlayerStats();

                Console.WriteLine("\nSpieler 2, welche Karte möchtest du spielen?");
                DisplayPlayerCards(_player2);
                int cardChosen2 = GetCardChosen(_player2);
                _cardStack.Add(_player2.Cards[cardChosen2]);
                _player2.Cards.RemoveAt(cardChosen2);
            }

            Console.WriteLine("\nAktueller Stapel:");
            Console.WriteLine(string.Join(" - ", _cardStack));
        }

        private void DisplayPlayerStats()
        {
            Console.WriteLine($"\nSpieler 1 - Gesundheit: {_player1.Health}, Trefferzahl: {_player1.HitNumber}");
            Console.WriteLine($"Spieler 2 - Gesundheit: {_player2.Health}, Trefferzahl: {_player2.HitNumber}");
        }

        private void DisplayPlayerCards(Player player)
        {
            Console.WriteLine("Deine Karten:");
            for (int i = 0; i < player.Cards.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {player.Cards[i]}");
            }
        }

        private int GetCardChosen(Player player)
        {
            int cardChosen = 0;
            bool validChoice = false;

            while (!validChoice)
            {
                Console.Write("Wähle eine Karte aus (Nummer): ");
                string? input = Console.ReadLine();
                if (int.TryParse(input, out cardChosen) && cardChosen >= 1 && cardChosen <= player.Cards.Count)
                {
                    validChoice = true;
                }
                else
                {
                    Console.WriteLine("Ungültige Eingabe. Bitte wähle eine gültige Kartennummer.");
                }
            }
            return cardChosen - 1; // Anpassung für nullbasierten Index
        }

        private bool GetPlayerAccusation()
        {
            while (true)
            {
                Console.Write("Deine Wahl (j/n): ");
                var input = Console.ReadLine()?.ToLower();

                if (input == "j")
                {
                    return true;
                }
                else if (input == "n")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Ungültige Eingabe. Bitte gib 'j' für Ja oder 'n' für Nein ein.");
                }
            }
        }

        private bool CheckIfPlayerLied()
        {
            string lastCard = _cardStack[^1];
            return _gameCardIcon != lastCard && lastCard != "J";
        }

        private void ManageLieInteraction(Player accuser, Player accused, bool accuserThinksLied, bool actualLie)
        {
            if (!accuserThinksLied)
            {
                Console.WriteLine("\nDu hast dich entschieden, den anderen Spieler nicht zu beschuldigen.");
                return;
            }

            if (actualLie)
            {
                Console.WriteLine("\nDu hast korrekt beschuldigt! Der beschuldigte Spieler erhält einen Gesundheitspunkt.");
                accused.Health++;
            }
            else
            {
                Console.WriteLine("\nFalsche Beschuldigung! Du erhältst einen Gesundheitspunkt.");
                accuser.Health++;
            }
        }

        private void SetPlayerHitNumbers(Player player1, Player player2)
        {
            Random random = new Random();
            player1.HitNumber = random.Next(1, 7);
            player2.HitNumber = random.Next(1, 7);
        }

        private int GetCardCount()
        {
            int countCard = 0;
            bool isValidInput = false;

            while (!isValidInput)
            {
                Console.Write("Wie viele Karten sollen die Spieler haben? (3-5): ");
                string? input = Console.ReadLine();

                if (int.TryParse(input, out countCard) && countCard >= 3 && countCard <= 5)
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

        private string GetRandomCardIcon()
        {
            Random random = new Random();
            int cCount = random.Next(1, 5);

            return cCount switch
            {
                1 => "B",
                2 => "Q",
                3 => "K",
                4 => "A",
                _ => "Unbekannt"
            };
        }

        private List<string> DistributeCards(int countCard)
        {
            List<string> cardShuffled = new();
            Random random = new Random();

            for (int i = 0; i < countCard; i++)
            {
                int rCount = random.Next(1, 6);
                string card = rCount switch
                {
                    1 => "B",
                    2 => "Q",
                    3 => "K",
                    4 => "A",
                    5 => "J",
                    _ => "Unbekannt"
                };
                cardShuffled.Add(card);
            }
            return cardShuffled;
        }
    }
}
