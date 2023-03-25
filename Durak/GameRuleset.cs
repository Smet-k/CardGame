using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak
{
    public class GameRuleset
    {
        public int attackerID;
        public int defendedID;
        public GameRuleset()
        {
            attackerID = id;
            defendedID = id + 1;
        
        }
        public static Random rng = new Random();

        public static int startingHand = 6;
        public static List<string> possibleSuits = new List<string> { "clubs", "spades", "hearts", "diamond" };
        public static List<string> symbol = new List<string> { "♣", "♠", "♥", "♦" };
        public static List<string> possibleValues = new List<string> { "6", "7", "8", "9", "10", "J", "Q", "K", "T" };

        public int playerAmount = 4;

        public bool singleplayer = false;

        public int id = 0;
        public int singlePlayerId = 0;
        public bool gameFinished = false;



        
        public int deckSize = possibleValues.Count * possibleSuits.Count;

        public int trumpCard = rng.Next(0, possibleSuits.Count);

        public List<Player> players = new List<Player>();

        public List<Card> banned = new List<Card>();

        public List<Card> MultPlayedCards = new List<Card>();

        public List<Card> playedCards = new List<Card>();

        public void setup()
        {
            while (true)
            {
                Console.WriteLine("Enter the player amount(2-4):");
                playerAmount = Convert.ToInt32(Console.ReadLine());
                if(playerAmount > 1 && playerAmount < 5) { break; }
                else { Console.WriteLine("Given number is out of range"); }
            }

            while (true) 
            {
                Console.WriteLine("Do you want to play with bots? \n [1] - Yes \n [2] - No");
                var choice = Console.ReadLine();
                if(choice == "1") 
                {
                    singleplayer = true; 
                    break; 
                }
                else if(choice == "2") 
                {
                    singleplayer = false; 
                    break;
                }
                else 
                {
                    Console.WriteLine("Index out of bounds");
                }

            }

            var loop = true;
            while (loop) 
            {
                Console.WriteLine("Choose the ruleset \n [1] - 24 cards \n [2] - 36 cards \n [3] - 52 cards");
                switch (Console.ReadLine()) 
                {
                    case "1": possibleValues = new List<string> {"9", "10", "J", "Q", "K", "T" }; deckSize = possibleValues.Count * possibleSuits.Count; loop = false; break;
                    case "2": possibleValues = new List<string> { "6", "7", "8", "9", "10", "J", "Q", "K", "T" }; deckSize = possibleValues.Count * possibleSuits.Count; loop = false; break;
                    case "3": possibleValues = new List<string> { "2","3","4","5","6", "7", "8", "9", "10", "J", "Q", "K", "T" }; deckSize = possibleValues.Count * possibleSuits.Count; loop = false; break;
                    default: Console.WriteLine("Given index out of bounds");break;
                }
            }
        }
    }
}
