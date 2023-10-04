using System;
using System.Collections.Generic;
using System.Data;

namespace Blackjack1
{
    class Program
    {
        static int playerCardsTotal = 0;
        static int dealerCardsTotal = 0;
        const double initialMoney = 100.00;
        static double playerMoney = initialMoney;
        static int betAmount = 0;

        static string[] playingCards = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        static List<int> playerCardScores = new List<int>();
        static List<int> dealerCardScores = new List<int>();

        static bool isActive = true;

        static void Main(string[] args)
        {
            Console.Title = "Blackjack";
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Let's play Blackjack!\n");

            while (isActive)
            {
                PrintMenu();
                GameRun();

                Console.Clear();
            }
        }

        private static void GameRun()
        {
            Console.WriteLine("\nPlease type option number and press Enter.");
            string menuOption = Console.ReadLine();

            switch (menuOption)
            {
                case "1":
                    Console.WriteLine("A participant attempts to beat the dealer by getting a count as close to 21 as possible, " +
                        "without going over 21. An ace may be worth 1 or 11. Face cards are 10 and any other card is its pip value.");
                    Console.WriteLine("Press Enter to return to the menu.");
                    Console.ReadLine();
                    break;
                case "2":

                    PlaceBet();

                    if (!IsBetSufficient())
                    {
                        PrintInvalidBet();
                        break;
                    }

                    NewDeal();

                    HitCard();
                    HitCard("Dealer");
                    HitCard();
                    HitCard("Dealer", faceDown: true);

                    bool canHit = true;

                    while (canHit)
                    {
                        canHit = HitAgain();
                        PrintTotal();
                    }

                    while(dealerCardsTotal < 17)
                    {
                        HitCard("Dealer");
                    }

                    PrintTotal("Dealer");

                    CalculateResult();

                    break;
                case "3":
                    Console.WriteLine("Exiting Blackjack.");
                    isActive = false;
                    break;
            }
        }

        private static bool HitAgain()
        {
            if(playerCardsTotal < 21)
            {
                Console.ForegroundColor = ConsoleColor.Black;   
                Console.WriteLine("Do you want to hit again?\n1. Hit 2. Stay");
                var hitAgain = Console.ReadLine();

                if(hitAgain == "1")
                {
                    HitCard();
                    return true;
                }
            }

            return false;
        }

        private static void PrintInvalidBet()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\nInsufficient bet...");
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        }
        private static bool IsBetSufficient()
        {
            bool isSufficient = false;

            if (betAmount <= playerMoney)
            {
                isSufficient = true;
            }

            return isSufficient;
        }
        private static void PlaceBet()
        {
            Console.WriteLine("What is your bet amount?");
            Console.WriteLine($"(You have: {playerMoney})");
            betAmount = int.Parse(Console.ReadLine());
        }
        private static void CalculateResult()
        {
            if (playerCardsTotal > 21 || (dealerCardsTotal <= 21 && dealerCardsTotal >= playerCardsTotal))
            {
                playerMoney -= betAmount; 
                PrintLoss();
            }
            else
            {
                playerMoney += betAmount;
                PrintWin();
            }
        }

        private static void PrintWin()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"You won!\nYour total money is now: {playerMoney}$\nPress Enter to return to menu.");
            Console.ReadKey();
        }

        private static void PrintLoss()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"Dealer won. Your total money is now: {playerMoney}$\nPress Enter to return to menu.");
            Console.ReadKey();
        }

        private static void PrintTotal(string role = "Player")
        {
            if (role == "Player")
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"{role} total card score: {playerCardsTotal}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{role} total card score: {dealerCardsTotal}");
            }

            Console.ForegroundColor = ConsoleColor.Black;
        }

        private static void NewDeal()
        {
            playerCardScores.Clear();
            dealerCardScores.Clear();
            playerCardsTotal = 0;
            dealerCardsTotal = 0;

            PrintDeal();
        }

        private static void PrintDeal()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Shuffling the deck...");
            Console.WriteLine("Dealing cards...");
            Console.ForegroundColor = ConsoleColor.Black;
        }

        private static int HitCard(string role = "Player", bool faceDown = false)
        {
            var randomGenerater = new Random();
            var playingCardHand = randomGenerater.Next(playingCards.Length);
            var playingCard = playingCards[playingCardHand];
            int card;

            if(playingCardHand == 0)
            {
                card = 11;
            }
            else if(playingCardHand < 9)
            {
                card = playingCardHand + 1;
            }
            else
            {
                card = 10;
            }

            if(role == "Player")
            {
                playerCardScores.Add(card);
                Console.ForegroundColor = ConsoleColor.Black;
                CalculateHit("Player");
            }
            else
            {
                dealerCardScores.Add(card);
                Console.ForegroundColor = ConsoleColor.DarkRed;

                if (!faceDown)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    CalculateHit("Dealer");
                }
            }

            if (!faceDown)
            {
                Console.WriteLine($"{role} is drawing a card... Card is: {card}");
            }
            else
            {
                Console.WriteLine($"Dealer is drawing a face-down card...");
            }

            Console.ForegroundColor = ConsoleColor.DarkRed;

            return card;
        }

        private static void CalculateHit(string role = "Player")
        {
            if (role == "Player")
            {
                playerCardsTotal = CalculateTotalScore(playerCardScores); 
            }
            else
            {
                dealerCardsTotal = CalculateTotalScore(dealerCardScores);

            }
        }

        private static int CalculateTotalScore(List<int> card)
        {
            var totalScore = card.Sum();

            if(totalScore > 21)
            {
                var ace = card.FirstOrDefault(cs => cs == 11);
                card.Remove(ace);
                card.Add(1);
                totalScore = card.Sum();
            }

            return totalScore;
        }

        private static void PrintMenu()
        {
            Console.WriteLine("1. Rules");
            Console.WriteLine("2. Play Game");
            Console.WriteLine("3. Exit");
        }
    }
}