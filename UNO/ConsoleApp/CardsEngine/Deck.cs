namespace CardsEngine
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using GameConfiguration;

    public class Deck
    {
        private readonly GameConfiguration _configuration;

        public Deck(GameConfiguration? configuration = null)
        {
            if (configuration == null)
            {
                _configuration = new GameConfiguration();
            }
            else
            {
                _configuration = configuration;
            }

        }

        private Random random = new Random();

        public Card GetCard(List<Card> used)
        {
            var colors = Enum.GetValues(typeof(ColorType)).Cast<ColorType>().ToArray();
            var types = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList();
            types.Remove(CardType.Number);
            types.Remove(CardType.WildDrawFour);
            types.Remove(CardType.Wild);

            while (true)
            {
                var randomNumber = random.Next(100);
                ColorType color = colors[random.Next(colors.Length)];

                Card card;

                if (randomNumber < _configuration.NumberDrawChance)
                {
                    var value = random.Next(0, 9);
                    card = new Card(color, CardType.Number, value);
                }
                else if (randomNumber < _configuration.ActionCardDrawChance)
                {
                    var type = types[random.Next(types.Count)];
                    card = new Card(color, type);
                }
                else if (randomNumber < _configuration.WildCardDrawChance)
                {
                    card = new Card(null, CardType.Wild);

                }
                else if (randomNumber < _configuration.WildCardPlusFourDrawChance)
                {
                    card = new Card(null, CardType.WildDrawFour);
                }
                else
                {
                    card = null;
                }

                if (_configuration.LimitBreaker || IsCardValid(used, card))
                {
                    return card;
                }
            }
        }

        private bool IsCardValid(List<Card> used, Card card)
        {
            var usageCount = used.Count(c => Equals(c, card));
            return card.Type switch
            {
                CardType.WildDrawFour => usageCount < _configuration.WildPlusFourAmountInGame,
                CardType.Wild => usageCount < _configuration.WildAmountInGame,
                _ => usageCount <= 2
            };
        }


    }
}