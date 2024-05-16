using CardsEngine;
using DAL;
using GameEngine;
using menu;
using MenuItem;
using PlayerEngine;
using HelperEnums;
using ConsoleUI.Visualization;

namespace ConsoleUI;

public class GameController
{
    private readonly UnoGame _engine;
    private readonly IGameRepository _repository;


    public GameController(UnoGame engine, IGameRepository repository)
    {
        _engine = engine;
        _repository = repository;
    }


    public void Run()
    {
        var skipWarning = false;
        Console.Clear();
        while (true)
        {
            if (_engine.GetTurnPlayer().Type == EPlayertype.AI)
            {
                skipWarning = true;
                _engine.AiGameTurn();
                _engine.ProcessTurnIndex();
            }
            else
            {
                Console.Clear();
                if (skipWarning)
                {
                    skipWarning = false;
                }
                else
                {
                    LoadStartingMenu();
                }
                
                Console.Clear();
                var choiceCheck = EPutCard.Back;
                do
                {
                    choiceCheck = LoadActionMenu();
                } while (choiceCheck == EPutCard.Back);
                if (choiceCheck == EPutCard.Exit) break;
                switch (choiceCheck)
                {
                    case EPutCard.PlaySuccess:
                        _engine.ProcessTurnIndex();
                        break;
                    case EPutCard.SuccessfulZero:
                        _engine.RotateHands();
                        _engine.ProcessTurnIndex();
                        break;
                    case EPutCard.SuccessfulSeven:
                        LoadHandSwitchMenu();
                        _engine.ProcessTurnIndex();
                        break;
                    case EPutCard.TakeNewCard:
                        if (_engine.GetGameState().DrawCount > 0)
                        {
                            _engine.GiveCard(_engine.GetTurnIndex());
                            _engine.ProcessTurnIndex();
                        }
                        else
                        {
                            _engine.GiveCard(_engine.GetTurnIndex());
                            skipWarning = true;
                        }
                        break;
                    case EPutCard.CallOut:
                        _engine.CheckShout();
                        break;
                    case EPutCard.PlayWild:
                        var color = LoadColorSelectingMenu();
                        _engine.ChangeTableColor(color);
                        _engine.ProcessTurnIndex();
                        break;
                    case EPutCard.Failed:
                        continue;
                    case EPutCard.Shout:
                        _engine.Shout();
                        break;
                    default:
                        throw new Exception($"Result code not implemented: {choiceCheck}");
                }
            }
            _engine.UpdateGameState();
            if (_engine.CheckWin() != -1)
            {
                Console.WriteLine($"Player {_engine.GetGameState().Players[_engine.CheckWin()].Name} has won the game!");
                break;
            }

            _repository.Save(_engine.GetGameState().Id, _engine.GetGameState());
        }
        _engine.UpdateGameState();
        _repository.Save(_engine.GetGameState().Id, _engine.GetGameState());
    }

    private void LoadStartingMenu()
    {
        Console.WriteLine("WARNING!");
        Console.WriteLine("------------------------------------");
        Console.WriteLine($"Player name {_engine.GetTurnPlayer().Name} about to start.");
        Console.WriteLine("Please make sure no one else is looking at the screen except this player");
        Console.WriteLine("Game is automatically saved after each turn");
        Console.WriteLine("Press Any key to continue.");
        Console.ReadKey();
    }

    private EPutCard LoadActionMenu()
    {
        EPutCard result = EPutCard.Default;
        var playCard = new Menuitem("Play card", () => result = LoadCardPickMenu());
        var pickCard = new Menuitem("Take card", () => result = EPutCard.TakeNewCard);
        var callOut = new Menuitem("Call out", () => result = EPutCard.CallOut);
        var exit = new Menuitem("Exit game", () => result = EPutCard.Exit);

        var actionMenu = new Menu(_engine.GetGameState(), true, playCard, pickCard, callOut, exit);
        actionMenu.Display();
        return result;
    }

    private EPutCard LoadCardPickMenu()
    {
        EPutCard result = EPutCard.Default;
        var current = _engine.GetGameState().Players[_engine.GetTurnIndex()];
        var choices = new List<Menuitem>();
        choices.Add(new Menuitem("Shout", () => result = EPutCard.Shout));
        for (var i = 0; i < current.GetHand().Count; i++)
        {
            var currentPlayerCardIndex = i;
            var choice = new Menuitem($"Card {currentPlayerCardIndex + 1}",
                () => result = _engine.PutCardOnTable(current.GetHand()[currentPlayerCardIndex]));
            choices.Add(choice);
        }
        choices.Add(new Menuitem("back", () => result = EPutCard.Back));

        var playerCardChoices = choices.ToArray();
        var playerMenu = new Menu(_engine.GetGameState(), true, playerCardChoices);
        playerMenu.Display();
        return result;
    }

    private ColorType LoadColorSelectingMenu()
    {
        var result = ColorType.Red;
        var red = new Menuitem("Red", () => result = ColorType.Red);
        var blue = new Menuitem("Blue", () => result = ColorType.Blue);
        var yellow = new Menuitem("Yellow", () => result = ColorType.Yellow);
        var green = new Menuitem("Green", () => result = ColorType.Green);

        var colorPickMenu = new Menu(_engine.GetGameState(), true, red, blue, yellow, green);
        colorPickMenu.Display();
        return result;
    }

    private void LoadHandSwitchMenu()
    {
        var choices = new List<Menuitem>();
        var index = 0;
        foreach (var player in _engine.GetGameState().Players)
        {
            if (player.Name.Equals(_engine.GetGameState().Players[_engine.GetGameState().TurnIndex].Name))
            {
                index++;
                continue;
            }
            var name = player.Name;
            var cardAmount = player.GetHand().Count;
            var currentIndex = index;
            
            var choice = new Menuitem($"Switch hands with {name}. Has {cardAmount} card's",
                () => _engine.SwitchHand(currentIndex));
            choices.Add(choice);
            index++;
        }
        var playerCardChoices = choices.ToArray();
        var playerMenu = new Menu("Switch cards with: ", playerCardChoices);
        
        playerMenu.Display();
    }
}