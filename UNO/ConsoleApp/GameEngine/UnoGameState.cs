namespace GameEngine
{
    using System;
    using PlayerEngine;
    using CardsEngine;
    using GameConfiguration;
    
    
    public class UnoGameState
    {
        // Properties representing the game state data
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<Player> Players { get; set; } = new List<Player>();
        public int TurnIndex { get; set; }
        public bool Reverse { get; set; }
        public bool Skip { get; set; }
        
        public bool HasToStack { get; set; }
        
        public int DrawCount { get; set; }
        public Card OnTable { get; set; } = Card.GenerateTableCard();

        public int MaxPlayers { get; set; } = 6;
        
        public Status Progress { get; set; } = Status.Starting;

        public GameConfiguration Config { get; set; } = new GameConfiguration();
        
        public UnoGameState Clone()
        {
            var cloned = new UnoGameState
            {
                Id = Id,
                Players = Players.Select(player => player.Clone()).ToList(),
                TurnIndex = TurnIndex,
                DrawCount = DrawCount,
                Skip = Skip,
                Reverse = Reverse,
                OnTable = OnTable?.Clone(),
                MaxPlayers = MaxPlayers,
                Progress = Progress,
                Config = Config
            };
            return cloned;
        }
    }
    

}