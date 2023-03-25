using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak
{
    public class Card
    {
        public KeyValuePair<string, string> card;

        public Card(Durak.Card input)
        {
            card = input.card;
        }

        public Card()
        {
            card = new KeyValuePair<string, string> ();
        }

        public Card(string _suit,string _value)
        {
            card = new KeyValuePair<string,string>(_suit,_value) ;
        }

        public Card(KeyValuePair <string, string> _card)
        {
            card = _card;
        }

    }

    
}
