namespace GameEngine
{
    using PlayerEngine;
    using CardsEngine;
    using HelperEnums;
    using GameConfiguration;

    public class UnoGame
    {
        private int _turnIndex;
        private Card? _onTable;
        private bool _reverse;
        private bool _skip;
        private int _drawCount;
        private readonly List<Player> _players;
        private readonly Deck _deck;
        private readonly UnoGameState _state;
        private Status _progress;
        private readonly GameConfiguration _configuration;
        private bool _hasToStack;


        public UnoGame(UnoGameState gameState)
        {
            _hasToStack = gameState.HasToStack;
            _turnIndex = gameState.TurnIndex;
            _onTable = gameState.OnTable;
            _progress = gameState.Progress;
            _reverse = gameState.Reverse;
            _skip = gameState.Skip;
            _drawCount = gameState.DrawCount;
            _players = gameState.Players;
            _configuration = gameState.Config;
            _deck = new Deck(_configuration);
            var count = _players.Sum(t => t.GetHand().Count);
            if (count == 0)
            {
                InitializeHands();
            }
            _state = gameState;
        }
        
        public void UpdateGameState()
        {
            _state.Players = _players;
            _state.TurnIndex = _turnIndex;
            _state.Reverse = _reverse;
            _state.Skip = _skip;
            _state.DrawCount = _drawCount;
            _state.OnTable = _onTable;
            _progress = CheckWin() == -1 ? Status.Playing : Status.Completed;
            _state.Progress = _progress;
            _state.Config = _configuration;
            _state.HasToStack = _hasToStack;
        }


        public UnoGameState GetGameState()
        {
            var cloned = _state.Clone();
            return cloned;
        }

        
        private void InitializeHands()
        {
            foreach (var player in _players)
            {
                for (int i = 0; i < _configuration.StartCards; i++)
                {
                    player.Add(_deck.GetCard(GetInUse()));
                }
            }
        }

        public Player GetPlayer(string name)
        {
            return (from index in _players where index.Name.Equals(name) select index.Clone()).FirstOrDefault();
        }
        


        private List<Card> GetInUse()
        {
            List<Card> result = new List<Card>();
            foreach (var player in _players)
            {
                result.AddRange(player.GetHand());
            }

            return result;
        }
        

        private bool CanPut(Card want)
        {
            if (_onTable == null)
            {
                return true;
            }

            var isOnTableNumbered = (_onTable.Type == CardType.Number);
            var isWantWild = want.Type is CardType.Wild or CardType.WildDrawFour;
            var isColorMatch = (want.Color == _onTable.Color);
            var isNumberMatch = (want.Number == _onTable.Number);

            if (isOnTableNumbered)
            {
                return isWantWild || isColorMatch || isNumberMatch;
            }
    
            return isWantWild || isColorMatch || (want.Type == _onTable.Type);
        }


        public Player GetTurnPlayer()
        {
            var current = _players[_turnIndex].Clone();
            return current;
        }

        public void GiveCard(int playerIndex)
        {
            _hasToStack = false;
            if (_drawCount == 0) _drawCount = 1;
            for (var i = 0; i < _drawCount; i++)
            {
                _players[playerIndex].Add(_deck.GetCard(GetInUse()));
            }
            _drawCount = 0;
        }

        public void CheckShout()
        {
            var indexes = new List<int>();
            for (var i = 0; i < _players.Count; i++)
            {
                if (i == _turnIndex) continue;
                if (_players[i].GetHand().Count < 2 && !_players[i].Shout.Equals("UNO")) indexes.Add(i);
                if (_players[i].GetHand().Count > 2 && _players[i].Shout.Equals("UNO")) indexes.Add(i);
            }

            foreach (var number in indexes)
            {
                _drawCount = 2;
                GiveCard(number);
            }

            foreach (var player in _players)
            {
                player.Shout = "";
            }

            if (indexes.Count != 0) return;
            _drawCount = 2;
            GiveCard(GetTurnIndex());
        }

        public int CheckWin() // return's the winner index  in the players list
        {
            for (var i = 0; i < _players.Count; i++)
            {
                if (_players[i].GetHand().Count == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private List<Card> GetPossibleAmount(List<Card> hand)
        {
            List<Card> result = new List<Card>();

            foreach (var card in hand)
            {
                if (CanPut(card))
                {
                    result.Add(card);
                }
            }
            return result;
        }

        public int GetTurnIndex()
        {
            return _turnIndex;
        }
        

        public void ProcessTurnIndex()
        {
            int direction = _reverse ? -1 : 1;

            if (_skip)
            {
                _turnIndex += 2 * direction;
                if (_turnIndex < 0)
                {
                    _turnIndex = _players.Count - 1;
                }
                else if (_turnIndex >= _players.Count)
                {
                    _turnIndex = 0;
                    
                }
                _skip = false;
            }
            else
            {
                _turnIndex += direction;
                if (_turnIndex < 0)
                {
                    _turnIndex = _players.Count - 1;
                }
                else if (_turnIndex >= _players.Count)
                {
                    _turnIndex = 0;
                }
            }

            if (_turnIndex < 0 || _turnIndex > _players.Count)
                throw new IndexOutOfRangeException($"Turn index processing error: {_turnIndex}");
        }

        private bool CanStack(Card selected)
        {
            switch (_onTable.Type)
            {
                case CardType.WildDrawFour when selected.Type == CardType.WildDrawFour:
                case CardType.DrawTwo when selected.Type is CardType.DrawTwo or CardType.WildDrawFour:
                    return true;
                default:
                    return false;
            }
        }

        public EPutCard PutCardOnTable(Card selected)
        {
            if (CanPut(selected) && !_hasToStack)
            {
                if (_players[GetGivenPlayerIndex()].Shout.Equals("UNO"))
                {
                    _players[GetGivenPlayerIndex()].Shout = "";
                }
                switch (selected.Type)
                {
                    case CardType.Skip:
                        _skip = true;
                        break;
                    case CardType.Reverse:
                        _reverse = !_reverse;
                        break;
                    case CardType.DrawTwo:
                    {
                        _skip = _configuration.PlusTwoSkip;
                        var givenPlayer = GetGivenPlayerIndex();
                        _drawCount = _configuration.PlusTwoAmount;
                        if (_skip)
                        {
                            GiveCard(givenPlayer); 
                        }

                        if (_configuration.Stacking)
                        {
                            _hasToStack = true;
                        }
                        if (!_skip && !_configuration.Stacking)
                        {
                            GiveCard(givenPlayer);
                        }

                        break;
                    }
                    case CardType.WildDrawFour:
                    {
                        _skip = _configuration.PlusFourSKip;
                        var givenPlayer = GetGivenPlayerIndex();
                        _drawCount = _configuration.PlusFourAmount;
                        if (_skip)
                        {
                            GiveCard(givenPlayer); 
                        }
                        if (_configuration.Stacking)
                        {
                            _hasToStack = true;
                        }

                        if (!_skip && !_configuration.Stacking)
                        {
                            GiveCard(givenPlayer);
                        }

                        break;
                    }
                }

                _onTable = selected;
                _players[_turnIndex].Remove(selected);
                if (_configuration.SevenSwitch && selected.Number == 7)
                {
                    return EPutCard.SuccessfulSeven;
                }

                if (_configuration.ZeroSwitch && selected.Number == 0)
                {
                    return EPutCard.SuccessfulZero;
                }
                return (selected.Type is CardType.Wild or CardType.WildDrawFour) ? EPutCard.PlayWild : EPutCard.PlaySuccess;
            }

            if (CanPut(selected) && _hasToStack && CanStack(selected))
            {
                if (_players[GetGivenPlayerIndex()].Shout.Equals("UNO"))
                {
                    _players[GetGivenPlayerIndex()].Shout = "";
                }
                switch (selected.Type)
                {
                    case CardType.WildDrawFour:
                        _drawCount += 4;
                        break;
                    case CardType.DrawTwo:
                        _drawCount += 2;
                        break;
                }
                return (selected.Type is CardType.Wild or CardType.WildDrawFour) ? EPutCard.PlayWild : EPutCard.PlaySuccess;
            }

            if (CanPut(selected) && _hasToStack && !CanStack(selected))
            {
                if (_players[GetGivenPlayerIndex()].Shout.Equals("UNO"))
                {
                    _players[GetGivenPlayerIndex()].Shout = "";
                }
                GiveCard(_turnIndex);
                return EPutCard.PlaySuccess;
            }

            return EPutCard.Failed;
        }

        private int GetGivenPlayerIndex()
        {
            var givenPlayerIndex = (_reverse) ? _turnIndex - 1 : _turnIndex + 1;

            if (givenPlayerIndex < 0)
            {
                givenPlayerIndex = _players.Count - 1;
            }
            
            else if (givenPlayerIndex >= _players.Count)
            {
                givenPlayerIndex = 0;
            }

            if (givenPlayerIndex == -1)
            {
                throw new Exception("Something wrong with adding cards code");
            }

            return givenPlayerIndex;
        }

        public void ChangeTableColor(ColorType color)
        {
            if (_onTable != null) _onTable.Color = color;
        }

        public void Shout()
        {
            _players[_turnIndex].Shout = "UNO";
        }


        public void AiGameTurn()
        {
            var hand = GetPossibleAmount(_players[_turnIndex].GetHand());
            
            while (hand.Count < 1)
            {
                GiveCard(_turnIndex);
                hand = GetPossibleAmount(_players[_turnIndex].GetHand());
            }
            

            var order = new List<CardType>() {CardType.WildDrawFour, CardType.DrawTwo, CardType.Skip,
                                            CardType.Reverse, CardType.Wild, CardType.Wild, CardType.Number};
            
            hand = hand.OrderBy(x => x.Color).ToList(); 
            var next = _players[GetGivenPlayerIndex()];
            if (next.GetHand().Count < 2 && next.Shout != "UNO")
            {
                CheckShout();
                
            }
            if (next.GetHand().Count > 2)
            {
                order.Reverse();
            }

            var bot = GetTurnPlayer();
            
            bot.Shout = "UNO";
            AiCardPlacement(order, hand);
        }


        private void AiCardPlacement(List<CardType> priorityQueue, List<Card> hand)
        {
            var mostColors = FindMostColors(hand);
            foreach (var type in priorityQueue)
            {
                var index = FindCard(hand, type);
                if (index == -1) continue;
                var result = PutCardOnTable(hand[index]);
                switch (result)
                {
                    case EPutCard.SuccessfulSeven:
                    {
                        var temp = new List<Player>(_players);
                        temp.Remove(GetTurnPlayer());
                        temp = temp.OrderBy(player => player.Hand.Count).ToList();
                        var switchIndex = _players.IndexOf(temp[0]);
                        SwitchHand(switchIndex);
                        break;
                    }
                    case EPutCard.SuccessfulZero:
                        RotateHands();
                        break;
                    case EPutCard.PlayWild:
                        ChangeTableColor(mostColors ?? ColorType.Red);
                        break;
                }
                break;
            }
        }

        private static int FindCard(IReadOnlyList<Card> hand, CardType type)
        {
            for (var i = 0; i < hand.Count; i++)
            {
                if (hand[i].Type == type) return i;
            }

            return -1;
        }

        private static ColorType? FindMostColors(List<Card> hand)
        {
            Dictionary<ColorType, int> found = new Dictionary<ColorType, int>();
            var black = 0;

            foreach (var card in hand)
            {
                var current = card.Color;

                if (card.Color == null)
                {
                    black++;
                    continue;
                } 
                if (found.TryGetValue(current.Value, out var value))
                {
                    found[current.Value] = ++value;
                }
                else
                {
                    found.Add(current.Value, 1);
                }
            }
            
            var sortedFound = found.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);


            var first = sortedFound.FirstOrDefault();
            
            if (first.Value > black)
            {
                return first.Key;
            }

            return null;
        }

        public void SwitchHand(int index = -1)
        {
            List<Card> turnPlayerHand;
            List<Card> newPlayerHand;
            var first = GetTurnIndex();
            if (index == -1)
            {
                turnPlayerHand = new List<Card>(_players[0].GetHand());
                newPlayerHand = new List<Card>(_players[1].GetHand());
                first = 0;
                index = 1;
            }
            else
            {
                turnPlayerHand = new List<Card>(GetTurnPlayer().GetHand());
                newPlayerHand = new List<Card>(_players[index].GetHand()); 
            }
            _players[first].Hand = newPlayerHand;
            _players[index].Hand = turnPlayerHand;
        }

        public void RotateHands()
        {
            if (_players.Count < 3)
            {
                SwitchHand();
            } 
            var handStack = new Stack<List<Card>>();
            
            switch (_reverse)
            {
                case true:
                {
                    handStack.Push(new List<Card>(_players.First().GetHand()));
                    for (var i = _players.Count - 1; i > 0; i--)
                    {
                        handStack.Push(new List<Card>(_players[i].GetHand()));
                    }
                    break;
                }
                default:
                {
                    for (var i = _players.Count-2; i >= 0; i--)
                    {
                        handStack.Push(new List<Card>(_players[i].GetHand()));
                    }
                    handStack.Push(new List<Card>(_players.Last().GetHand()));
                    break;
                }
            }

            foreach (var t in _players)
            {
                t.Hand = handStack.Pop();
            }
        }
    }
}
    