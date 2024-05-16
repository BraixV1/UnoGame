namespace CardsEngine
{
    public class Card
    {
        private sealed class ColorTypeNumberEqualityComparer
        {
            public int GetHashCode(Card obj)
            {
                return HashCode.Combine(obj.Color, (int)obj.Type, obj.Number);
            }
        }
        

        
        public ColorType? Color { get; set; }
        public CardType Type { get; }
        public int Number { get; }
        public List<Actions> Actions { get; } = new List<Actions>();

        public Card(ColorType? color, CardType type, int number = -1, List<Actions> actions = null)
        {
            // Set color to null for Wild cards
            Color = color;
            Type = type;
            Number = (type != CardType.Number) ? -1 : number;

            if (actions != null)
                Actions.AddRange(actions);
        }

        public override string ToString()
        {
            string cardColor = GetColorSymbol();
            string cardValue = GetCardValue();
            int maxWidth = 10; // Adjust the width as needed
            Console.ForegroundColor = GetTextColor();

            string formattedCard = $@"
┌{new string('─', maxWidth)}┐
│{cardColor.PadLeft(maxWidth)}│
│{"".PadLeft(maxWidth)}│
│{CenterString(cardValue, maxWidth)}│
│{"".PadLeft(maxWidth)}│
│{cardColor.PadRight(maxWidth)}│
└{new string('─', maxWidth)}┘";
            
            Console.ResetColor();
            return formattedCard;
        }

        private string CenterString(string text, int width)
        {
            if (text.Length >= width)
            {
                return text;
            }

            int padding = (width - text.Length) / 2;
            return text.PadLeft(text.Length + padding).PadRight(width);
        }


        public string GetColorSymbol()
        {
            switch (Color)
            {
                case ColorType.Red:
                    return "🟥";
                case ColorType.Green:
                    return "🟩";
                case ColorType.Blue:
                    return "🟦";
                case ColorType.Yellow:
                    return "🟨";
                default:
                    return "Wild";
            }
        }
        
        private ConsoleColor GetTextColor()
        {
            switch (Color)
            {
                case ColorType.Red:
                    return ConsoleColor.Red;
                case ColorType.Green:
                    return ConsoleColor.Green;
                case ColorType.Blue:
                    return ConsoleColor.Blue;
                case ColorType.Yellow:
                    return ConsoleColor.Yellow;
                default:
                    return ConsoleColor.White;
            }
        }
        public string GetCardValue()
        {
            switch (Type)
            {
                case CardType.Wild:
                    return "Wild";
                case CardType.WildDrawFour:
                    return "Wild+4";
                case CardType.Reverse:
                    return "🔁";
                case CardType.Skip:
                    return "⏭️";
                case CardType.DrawTwo:
                    return "+2";
                case CardType.Number:
                    return (Number != -1) ? Number.ToString() : "";
                default:
                    return " ";
            }
        }
        public override bool Equals(object? obj)
        {
            if (obj is Card otherCard)
            {
                return otherCard.Type == this.Type && otherCard.Color == this.Color && otherCard.Number == this.Number;
            }

            return false;
        }

        public Card Clone()
        {
            var replica = new Card(Color ,Type, Number);
            return replica;
        }
        
        private static Random random = new Random();
        
        public static Card GenerateTableCard()
        {
            var colors = Enum.GetValues(typeof(ColorType)).Cast<ColorType>().ToArray();
            ColorType color = colors[random.Next(colors.Length)];
            var value = random.Next(0, 9);
            var card = new Card(color, CardType.Number, value);
            return card;
        }

    }
}

