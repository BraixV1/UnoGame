using System.Text;
using CardsEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization; // Add this namespace

namespace PlayerEngine
{
    public class Player
    {
        private string name;
        private EPlayertype type;

        [JsonPropertyName("Hand")] // Use this attribute to rename the property in JSON
        private List<Card> hand;

        private string _shout = "";
        
        public Player()
        {
            this.name = "UnknownPlayer";
            this.hand = new List<Card>();
            this.Type = EPlayertype.Human;
        }
        
        
        public Player(string name = "UnknownPlayer", EPlayertype type = EPlayertype.Human)
        {
            this.name = name;
            this.hand = new List<Card>();
            this.type = type;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Shout
        {
            get => _shout;
            set => _shout = value;
        }

        public EPlayertype Type
        {
            get => type;
            set => type = value;
        }
        
        public List<Card> Hand
        {
            get => hand;
            set => hand = value;
        }
        

        public void Add(Card card)
        {
            hand.Add(card);
        }

        public void Remove(Card card)
        {
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].Equals(card))
                { 
                    hand.Remove(hand[i]);
                    break;
                }
            }
        }

        public List<Card> GetHand()
        {
            return hand;
        }

        public string GetHandAsString()
        {
            var result = new StringBuilder("[ ");
            var index = 1;
            foreach (var card in hand)
            {
                result.Append($"{index}) {card.GetColorSymbol()} {card.GetCardValue()} | ");
                index++;
            }

            result.Append(']');
            return result.ToString();
        }

        public Player Clone()
        {
            Player result = new Player(name, type);
            result.hand = hand;
            result._shout = _shout;

            return result;
        }
    }
}