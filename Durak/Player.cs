using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Durak.Serializer;

namespace Durak
{
    public class Player
    {
        static string path = Path.Combine("..", "..", "..", "log.txt");
        List<string> suits = new List<string> { "clubs", "spades", "hearts", "diamond" };
        List<string> symbol = new List<string> { "♣", "♠", "♥", "♦" };
        static List<string> possibleValues = new List<string> { "6", "7", "8", "9", "10", "J", "Q", "K", "T" };

        public readonly List<Card> hand;


        public Player()
        {
            hand = new List<Card>();
        }

        public Player(Player input) 
        {
            hand = input.hand;
        }

        public void AddToDeck(Card card) 
        {
            hand.Add(card);
        }

        public int getHandSize() 
        {
            return hand.Count;
        }

        public void playCard(int index) 
        {
            //Console.WriteLine($" - {hand[index].card.Value} {symbol[suits.IndexOf(hand[index].card.Key)]} {hand[index].card.Key} - ");
            hand.RemoveAt(index);
        }

        public void printHand() 
        {
            hand.ForEach(x => Console.WriteLine($"[{hand.IndexOf(x) + 1}] - {x.card.Value} {symbol[suits.IndexOf(x.card.Key)]} {x.card.Key} - "));
        }

        public int AIplay(Card lastPlayedCard,int trump,List<Card> playedCards,bool isAttacker)
        {
            var prioritizedList = new List<KeyValuePair<int, Card>>();


            if (isAttacker)
            {
                if (isAttacker && playedCards.Count == 0) { isAttacker = false; }
                else if (isAttacker && playedCards.Count > 0)
                {
                    
                    hand.ForEach(y =>
                    {
                        bool isPlayable = false;
                        playedCards.ForEach(x =>
                    {
                        if (x.card.Value == y.card.Value)
                        {
                            isPlayable = true;
                        }
                    }); 
                        if(isPlayable) 
                        {
                            prioritizedList.Add(new KeyValuePair<int, Card>(possibleValues.IndexOf(y.card.Value), y));
                        }
                    });
                }
            }
            if (prioritizedList.Count <= 0 && isAttacker == true) { return hand.Count; }

            if (isAttacker == false)
            {
                bool empty = false;
                if (lastPlayedCard.card.Key == "") { empty = true; }

                hand.ForEach(x =>
                {
                    if (empty == true)
                    {
                        if (suits.IndexOf(x.card.Key) == trump) { prioritizedList.Add(new KeyValuePair<int, Card>(possibleValues.IndexOf(x.card.Value) + 12, x)); }
                        else { prioritizedList.Add(new KeyValuePair<int, Card>(possibleValues.IndexOf(x.card.Value), x)); }
                    }
                    else if (x.card.Key == lastPlayedCard.card.Key && possibleValues.IndexOf(x.card.Value) > possibleValues.IndexOf(lastPlayedCard.card.Value)
                       || suits.IndexOf(x.card.Key) == trump && suits.IndexOf(lastPlayedCard.card.Key) != trump)
                    {


                        if (suits.IndexOf(x.card.Key) == trump) { prioritizedList.Add(new KeyValuePair<int, Card>(possibleValues.IndexOf(x.card.Value) + 12, x)); }
                        else { prioritizedList.Add(new KeyValuePair<int, Card>(possibleValues.IndexOf(x.card.Value), x)); }

                    }
                    else if (suits.IndexOf(lastPlayedCard.card.Key) == trump && possibleValues.IndexOf(x.card.Value) > possibleValues.IndexOf(lastPlayedCard.card.Value)
                            && x.card.Key == lastPlayedCard.card.Key)
                    {
                        prioritizedList.Add(new KeyValuePair<int, Card>(possibleValues.IndexOf(x.card.Value), x));
                    }
                }
                );
                if (prioritizedList.Count <= 0) { return hand.Count; }
            }
            for(var i = 0;i < prioritizedList.Count - 1;i++) 
            {

                if (Convert.ToInt32(prioritizedList[i].Key) > Convert.ToInt32(prioritizedList[i + 1].Key))
                {
                    var temp = prioritizedList[i];
                    prioritizedList[i] = prioritizedList[i + 1];
                    prioritizedList[i + 1] = temp;
                    i = -1;
                }
                
            }
            var debug = hand[hand.IndexOf(prioritizedList[0].Value)];

            return hand.IndexOf(prioritizedList[0].Value);
        }
    }
}
