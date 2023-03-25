using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Serialization;
using static Durak.Serializer;

namespace Durak
{
    public class Program 
    {
        #region Game rule set
 
        //Log
        static LogClass log = new LogClass();

        //Game rule set     
        static GameRuleset rules = new GameRuleset();

        #endregion


        public static void Main()
        {
            var lastPlayedCard = new Card("", "");

            #region save/load
            void SaveGame(string name)
            {
                CreateSave(name);
                WriteToFile<GameRuleset>(JsonConvert.SerializeObject(rules), rules);
                WriteToFile<List<Player>>(JsonConvert.SerializeObject(rules.players), rules.players);
                WriteToFile<Card>(JsonConvert.SerializeObject(new KeyValuePair<string, string>(lastPlayedCard.card.Key, lastPlayedCard.card.Value)), lastPlayedCard);
            }

            void LoadGame(string name)
            {
                lastPlayedCard = new Card(JsonConvert.DeserializeObject<KeyValuePair<string, string>>(File.ReadAllText(Path.Combine("..", "..", "..", $"{currentGameId}", "Card.json"))));
                rules = JsonConvert.DeserializeObject<GameRuleset>(File.ReadAllText(Path.Combine("..", "..", "..", $"{currentGameId}", "GameRuleset.json")));
                rules.players = JsonConvert.DeserializeObject<List<Player>>(File.ReadAllText(Path.Combine("..", "..", "..", $"{currentGameId}", "List`1.json")));
            }




            while (true)
            {
                Console.WriteLine("Do you want to load the save?\n[1] - Yes \n[2] - No");
                var choice = Console.ReadLine();
                if (choice == "1")
                {
                    Console.WriteLine("Enter a save name: ");
                    var name = Console.ReadLine();
                    if (Directory.Exists(Path.Combine("..", "..", "..", $"{name}")))
                    {
                        LoadGame(name);
                        Console.Clear();
                        break;
                    }
                    else { Console.WriteLine($"File - {name} does not exist"); }
                }
                else
                {
                    rules.setup();
                    log.Log($"   ------- Trump Card: {GameRuleset.symbol[rules.trumpCard]} {GameRuleset.possibleSuits[rules.trumpCard]}");
                    for (var i = 0; i < rules.playerAmount; i++)
                    {
                        rules.players.Add(GenerateDecks(GameRuleset.startingHand));
                    }
                    rules.id = FirstTurn();
                    rules.attackerID = rules.id;
                    rules.defendedID = rules.id + 1;
                    if(rules.defendedID >= rules.playerAmount) 
                    {
                        rules.defendedID = 0;
                    }
                    break;
                }
            }
            #endregion

            Console.Clear();
            log.Log("   ------- Game Started");


            while (true)
            {
                if (rules.gameFinished == false)
                {
                    #region turnStart
                    bool isAttacker = rules.id == rules.attackerID;
                    Console.WriteLine($"Player {rules.id + 1} turn:");
                    if (isAttacker) { Console.WriteLine("Role:Attacker"); }
                    else { Console.WriteLine("Role:Defender"); }
                    if (rules.id == rules.playerAmount) { rules.id = 0; }
                    Console.WriteLine($"Current Trump Card: {GameRuleset.symbol[rules.trumpCard]} {GameRuleset.possibleSuits[rules.trumpCard]}");
                    Console.WriteLine($"Cards left in deck: {rules.deckSize}");

                    if (rules.playedCards.Count > 0)
                    {
                        Console.WriteLine("Played Card History:");
                        rules.playedCards.ForEach(x => Console.Write($" - {x.card.Value} {GameRuleset.symbol[GameRuleset.possibleSuits.IndexOf(x.card.Key)]} {x.card.Key} "));
                        Console.WriteLine(" - ");
                    }

                    if (rules.MultPlayedCards.Count > 0)
                    {
                        Console.WriteLine("Played Cards:");
                        rules.MultPlayedCards.ForEach(x => { Console.Write($" {x.card.Value} {GameRuleset.symbol[GameRuleset.possibleSuits.IndexOf(x.card.Key)]} {x.card.Key} -"); });
                        Console.WriteLine("");
                        Console.WriteLine($"Defend against: {rules.MultPlayedCards[0].card.Value} {GameRuleset.symbol[GameRuleset.possibleSuits.IndexOf(rules.MultPlayedCards[0].card.Key)]} {rules.MultPlayedCards[0].card.Key}");
                    }

                    log.Log($"   ------- Player {rules.id + 1} turn");
                    if (rules.id == rules.singlePlayerId)
                    {
                        rules.players[rules.id].printHand();
                        Console.WriteLine($"[{rules.players[rules.id].hand.Count + 1}] - Quit -");
                        Console.WriteLine($"[{rules.players[rules.id].hand.Count + 2}] - Save -");
                    }
                    #endregion

                    #region Card Choice 
                    int index;
                    bool quit = false;
                    while (true)
                    {
                        while (true)
                        {
                            if (rules.id != rules.singlePlayerId && rules.singleplayer == true)
                            {
                                index = rules.players[rules.id].AIplay(lastPlayedCard, rules.trumpCard, rules.playedCards, isAttacker);
                                if (index == rules.players[rules.id].getHandSize()) { quit = true; }
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Choose card by id: ");
                                index = Convert.ToInt32(Console.ReadLine()) - 1;
                                if (index < rules.players[rules.id].getHandSize() && index >= 0)
                                {
                                    break;
                                }
                                else if (lastPlayedCard.card.Value != "" && index == rules.players[rules.id].getHandSize())
                                {
                                    quit = true;
                                    break;
                                }
                                else if (lastPlayedCard.card.Value != "" && index == rules.players[rules.id].getHandSize() + 1)
                                {
                                    quit = true;
                                    Console.WriteLine("Enter save name: ");
                                    var name = Console.ReadLine();
                                    SaveGame(name);
                                    break;

                                }
                                else if (lastPlayedCard.card.Value == "" && index == rules.players[rules.id].getHandSize())
                                {
                                    Console.WriteLine("Its too early to quit");
                                    log.Log($"   ------- Player {rules.id + 1} tried to quit too early");
                                }
                                else
                                {
                                    log.Log($"   ------- Player {rules.id + 1} chose card out of bounds");
                                    Console.WriteLine("Index out of bounds");
                                }
                            }
                        }

                        if (quit == false)
                        {
                            if (checkIfPossible(lastPlayedCard, rules.players[rules.id].hand[index], rules.id) && rules.MultPlayedCards.Count != 0)
                            {
                                rules.MultPlayedCards.Remove(lastPlayedCard);
                                break;
                            }
                            else if (checkIfPossible(lastPlayedCard, rules.players[rules.id].hand[index], rules.id))
                            {
                                lastPlayedCard = rules.players[rules.id].hand[index];
                                break;
                            }
                            else
                            {
                                Console.WriteLine("This turn is not possible");
                                log.Log($"   ------- Player {rules.id + 1} chose wrong card");
                            }
                        }
                        else { break; }
                    }
                    #endregion

                    #region turnEnd
                    Console.Clear();
                    if (!quit)
                    {
                        lastPlayedCard = rules.players[rules.id].hand[index];

                        if (rules.MultPlayedCards.Count == 0)
                        {
                            log.Log($"   ------- Played cards: {rules.players[rules.id].hand[index].card.Value} {GameRuleset.symbol[GameRuleset.possibleSuits.IndexOf(rules.players[rules.id].hand[index].card.Key)]} {rules.players[rules.id].hand[index].card.Key} ");
                        }
                        else
                        {
                            rules.playedCards.Add(lastPlayedCard);
                            lastPlayedCard = rules.MultPlayedCards[0];
                            Console.Write($"Played Cards:");

                            Console.WriteLine($"Defend against: {rules.MultPlayedCards[0].card.Value} {GameRuleset.symbol[GameRuleset.possibleSuits.IndexOf(rules.MultPlayedCards[0].card.Key)]} {rules.MultPlayedCards[0].card.Key}");
                        }

                        rules.playedCards.Add(lastPlayedCard);



                        rules.players[rules.id].playCard(index);
                        //Підкидання
                        if (rules.id == rules.attackerID)
                        {
                            rules.players.ForEach(x =>
                            {
                                if (x != rules.players[rules.id] && x != rules.players[rules.defendedID])
                                {
                                    for (int i = 0; i < x.hand.Count; i++)
                                    {
                                        if (x.hand[i].card.Value == lastPlayedCard.card.Value && rules.players.IndexOf(x) == rules.singlePlayerId || x.hand[i].card.Value == lastPlayedCard.card.Value && rules.singleplayer == false)
                                        {
                                            if (rules.MultPlayedCards.Count == 0) { rules.MultPlayedCards.Add(lastPlayedCard); }
                                            Console.WriteLine($"Player {rules.players.IndexOf(x) + 1} Do you want to play  {x.hand[i].card.Value} {GameRuleset.symbol[GameRuleset.possibleSuits.IndexOf(x.hand[i].card.Key)]} {x.hand[i].card.Key}");
                                            Console.WriteLine("[1] - Yes");
                                            Console.WriteLine("[2] - No");
                                            var choice = Console.ReadLine();
                                            if (choice == "1")
                                            {
                                                rules.MultPlayedCards.Add(x.hand[i]);
                                                x.playCard(i);
                                                log.Log($"   ------- Threw {rules.players[rules.id].hand[i].card.Value} {GameRuleset.symbol[GameRuleset.possibleSuits.IndexOf(rules.players[rules.id].hand[i].card.Key)]} {rules.players[rules.id].hand[i].card.Key} ");
                                                Console.Clear();
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (rules.MultPlayedCards.Count == 0) { rules.MultPlayedCards.Add(lastPlayedCard); }

                                            if (x.hand[i].card.Value == lastPlayedCard.card.Value)
                                            {
                                                rules.MultPlayedCards.Add(x.hand[i]);
                                                x.playCard(i);
                                                break;
                                            }
                                        }
                                    }

                                }
                            }
                            );
                        }
                        Console.Clear();
                    }
                    else if (rules.id == rules.attackerID)
                    {
                        lastPlayedCard = new Card("", "");

                        rules.defendedID += 1;
                        if (rules.defendedID >= rules.playerAmount) { rules.defendedID = 0; }

                        rules.attackerID += 1;
                        if (rules.attackerID >= rules.playerAmount) { rules.attackerID = 0; }

                        rules.id = rules.attackerID;

                        rules.playedCards = new List<Card>();

                        if (rules.deckSize != 0)
                        {
                            log.Log($"   ------- Players draw the cards");
                            rules.players.ForEach(x => { if (x.hand.Count > 0) { DrawCard(x, GameRuleset.startingHand); } });
                        }

                        log.Log($"   ------- Player {rules.id + 1} quits his turn");
                        
                        Console.Clear( );
                        continue;
                    }
                    else
                    {

                        lastPlayedCard = new Card("", "");


                        rules.defendedID += 1;
                        if (rules.defendedID >= rules.playerAmount) { rules.defendedID = 0; }

                        rules.attackerID += 1;
                        if (rules.attackerID >= rules.playerAmount) { rules.attackerID = 0; }

                        rules.id = rules.attackerID;

                        log.Log($"   ------- Player {rules.id + 1} quits his turn");

                        rules.playedCards.ForEach(x => rules.players[rules.id].hand.Add(x));

                        rules.MultPlayedCards.ForEach(x => { if (!rules.players[rules.id].hand.Contains(x)) { rules.players[rules.id].hand.Add(x); } });
                        rules.MultPlayedCards.Clear();
                        rules.playedCards = new List<Card>();
                        lastPlayedCard = new Card("", "");
                        if (rules.deckSize != 0)
                        {
                            log.Log($"   ------- Players draw the cards");
                            rules.players.ForEach(x => DrawCard(x, GameRuleset.startingHand));
                        }
                        
                        continue;
                    }

                    if (rules.MultPlayedCards.Count == 0 || rules.id == rules.attackerID)
                    {
                        if (rules.id == rules.attackerID) { rules.id = rules.defendedID; }
                        else if (rules.id == rules.defendedID) { rules.id = rules.attackerID; }
                    }

                    if (checkForLooser())
                    {
                        rules.gameFinished = true;
                        Console.WriteLine("Game ends");
                        log.Log($"   ------- Game ends");
                    }
                    #endregion
                }
                else { break; }
            }
        }
        #region Functions

        public static int FirstTurn() 
        {
            int output = 0;
            int value = -1;
            rules.players.ForEach(x => 
            {
                x.hand.ForEach(y => 
                {
                    if(GameRuleset.possibleSuits.IndexOf(y.card.Key) == rules.trumpCard && value == -1 || GameRuleset.possibleSuits.IndexOf(y.card.Key) == rules.trumpCard && GameRuleset.possibleValues.IndexOf(y.card.Value) < value) 
                    {
                        value = GameRuleset.possibleValues.IndexOf(y.card.Value);
                        output = rules.players.IndexOf(x);
                    }
                }
                );
                }
            );
            return output;
        }

        public static Player GenerateDecks(int handSize) 
        {
            var output = new Player();
            Random rng = new Random();
            Card input;
            for (int i = 0;i< handSize; i++) 
            {
                input = new Card(GameRuleset.possibleSuits[rng.Next(0, GameRuleset.possibleSuits.Count)], GameRuleset.possibleValues[rng.Next(0, GameRuleset.possibleValues.Count)]);
               
                if (checkCard(input))
                {
                    i--;
                }
                else
                {
                    rules.banned.Add(input);
                    output.AddToDeck(input);
                    rules.deckSize--;
                }
            }

            return output;
        }
        public static Player DrawCard(Player player,int handSize) 
        {
            

            var output = player;

            Random rng = new Random();
            Card input;
            for (int i = output.hand.Count; i < handSize; i++)
            {
                input = new Card(GameRuleset.possibleSuits[rng.Next(0, GameRuleset.possibleSuits.Count)], GameRuleset.possibleValues[rng.Next(0, GameRuleset.possibleValues.Count)]);

                if (checkCard(input))
                {
                    i--;
                }
                else
                {
                    rules.banned.Add(input);
                    output.AddToDeck(input);
                    rules.deckSize--;
                }
            }

            return output;
        }

        public static bool checkCard(Card input) 
        {
            var output = false;
            rules.banned.ForEach(x => {
                if (x.card.Value == input.card.Value && x.card.Key == input.card.Key)
                {
                    output = true;
                }
            });
            return output;
        }
        public static bool checkIfPossible(Card lastCard, Card playedCard,int id)
        {
            if (rules.id == rules.attackerID)
            {
                bool isPlayable = false;
                if (id == rules.attackerID && rules.playedCards.Count == 0) { return true; }
                else if (id == rules.attackerID && rules.playedCards.Count > 0)
                {
                    rules.playedCards.ForEach(x =>
                    {
                        if (x.card.Value == playedCard.card.Value)
                        {
                            isPlayable = true;
                        }
                    });
                }
                return isPlayable;
            }
            else
            {
                if (lastCard.card.Key == playedCard.card.Key || lastCard.card.Key == "")
                {
                    if (GameRuleset.possibleValues.IndexOf(lastCard.card.Value) < GameRuleset.possibleValues.IndexOf(playedCard.card.Value) || lastCard.card.Value == "")
                    {
                        return true;
                    }
                }
                else if (playedCard.card.Key == GameRuleset.possibleSuits[rules.trumpCard])
                {
                    if (lastCard.card.Key != GameRuleset.possibleSuits[rules.trumpCard])
                    {
                        return true;
                    }
                    else
                    {
                        if (GameRuleset.possibleValues.IndexOf(lastCard.card.Value) < GameRuleset.possibleValues.IndexOf(playedCard.card.Value) || lastCard.card.Value == "")
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public static bool checkForLooser() 
        {
            int PlayerWithCards = rules.playerAmount;
            rules.players.ForEach(x => { if (x.hand.Count == 0) { PlayerWithCards--; } });
            if(PlayerWithCards <= 1) { return true; }
            return false;
        }
       
        #endregion
    }

}