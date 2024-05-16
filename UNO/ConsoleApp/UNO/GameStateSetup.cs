
using DAL;
using HelperEnums;
namespace UNO;
using GameEngine;
using menu;
using PlayerEngine;
using MenuItem;
using GameConfiguration;

public static class GameStateSetup
{
    private static ELoadMenu AskInput(string printed, int min, int max, GameConfiguration config, UnoGameState state, string field)
    {
        var toLoad = ELoadMenu.MainMenu;

        while (true)
        {
            Console.WriteLine(printed);
            Console.WriteLine($"Number must be between {min} and {max}");
            var resultString = Console.ReadLine();

            if (!int.TryParse(resultString, out var result)) continue;
            if (result < min || result > max) continue;

            switch (field)
            {
                case "MaxPlayers":
                    state.MaxPlayers = result;
                    toLoad = ELoadMenu.PlayersMenu;
                    break;
                case "StartCards":
                    config.StartCards = result;
                    toLoad = ELoadMenu.CardAlgorithmMenu;
                    break;
                case "plusTwoAmount":
                    config.PlusTwoAmount = result;
                    toLoad = ELoadMenu.CardAlgorithmMenu;
                    break;
                case "plusFourAmount":
                    config.PlusFourAmount = result;
                    toLoad = ELoadMenu.CardAlgorithmMenu;
                    break;
                case "wildAmountInGame":
                    config.WildAmountInGame = result;
                    toLoad = ELoadMenu.CardAlgorithmMenu;
                    break;
                case "wildPlusFourAmountInGame":
                    config.WildPlusFourAmountInGame = result;
                    toLoad = ELoadMenu.CardAlgorithmMenu;
                    break;
                case "numberDrawChance":
                    config.NumberDrawChance = result;
                    toLoad = ELoadMenu.CardAlgorithmMenu;
                    break;
                case "actionCardDrawChance":
                    config.ActionCardDrawChance = result;
                    toLoad = ELoadMenu.CardAlgorithmMenu;
                    break;
                case "wildCardDrawChance":
                    config.WildCardDrawChance = result;
                    toLoad = ELoadMenu.CardAlgorithmMenu;
                    break;
                case "wildCardPlusFourDrawChance":
                    config.WildCardPlusFourDrawChance = result;
                    toLoad = ELoadMenu.CardAlgorithmMenu;
                    break;
            }
            break;
        }

        return toLoad;
    }

    private static ELoadMenu AskName(string printed, Player player)
    {
        while (true)
        {
            Console.WriteLine(printed);
            var result = Console.ReadLine();
            if (string.IsNullOrEmpty(result)) continue;

            player.Name = result;
            break;
        }

        return ELoadMenu.PlayersMenu;
    }

    private static ELoadMenu AskSwitch(string printed, GameConfiguration config, string field)
    {
        var toLoad = ELoadMenu.MainMenu;
        var result = false;

        var yes = new Menuitem("Turn on", () => result = true);
        var no = new Menuitem("Turn off", () => result = false);

        var menu = new Menu(printed, yes, no);
        menu.Display();

        switch (field)
        {
            case "zeroSwitch":
                config.ZeroSwitch = result;
                toLoad = ELoadMenu.ActionCardMenu;
                break;
            case "sevenSwitch":
                config.SevenSwitch = result;
                toLoad = ELoadMenu.ActionCardMenu;
                break;
            case "plusTwoSkip":
                config.PlusTwoSkip = result;
                config.Stacking = config.PlusTwoSkip switch
                {
                    (true) => false,
                    _ => config.Stacking
                };
                toLoad = ELoadMenu.ActionCardMenu;
                break;
            case "plusFourSKip":
                config.PlusFourSKip = result;
                config.Stacking = config.PlusTwoSkip switch
                {
                    (true) => false,
                    _ => config.Stacking
                };

                toLoad = ELoadMenu.ActionCardMenu;
                break;
            case "stacking":
                config.Stacking = result;
                if (config.Stacking)
                {
                    config.PlusTwoSkip = false;
                    config.PlusFourSKip = false;
                }
                toLoad = ELoadMenu.ActionCardMenu;
                break;
        }

        return toLoad;
    }

    private static ELoadMenu AskTypeSwitch(string printed, Player player)
    {
        var yes = new Menuitem("Player", () => player.Type = EPlayertype.Human);
        var no = new Menuitem("BOT", () => player.Type = EPlayertype.AI);

        var menu = new Menu(printed, yes, no);
        menu.Display();

        return ELoadMenu.PlayersMenu;
    }

    private static ELoadMenu OverwriteGameState(Guid id, UnoGameState state, IGameRepository repo)
    {
        var found = repo.LoadGame(id);
        state.Id = found.Id;
        return ELoadMenu.Load;
    }

    public static ELoadMenu ConfigureGame(GameConfiguration gameConfig, UnoGameState state, IGameRepository gameRepository, ELoadMenu menuToLoad)
    {
        // all menus under new game
        var playerChoices = new List<Menuitem>();
        var actionCardChoices = new List<Menuitem>();
        var cardAlgorithmChoices = new List<Menuitem>();
        var result = menuToLoad;

        var playerCount = new Menuitem($"Players in game {state.MaxPlayers}",
            () => result = AskInput($"Players in game. Current {state.MaxPlayers}", 2, 7, gameConfig, state, "MaxPlayers"));
        playerChoices.Add(playerCount);

        var startCard = new Menuitem($"Start cards amount current setting {gameConfig.StartCards}",
            () => result = AskInput($"Start amount of cards. Current {gameConfig.StartCards}", 1, int.MaxValue, gameConfig, state, "StartCards"));
        cardAlgorithmChoices.Add(startCard);

        var plusTwoAmount = new Menuitem($"plus two give amount. Current {gameConfig.PlusTwoAmount}",
            () => result = AskInput($"plus two give amount. Current {gameConfig.PlusTwoAmount}", 0, int.MaxValue, gameConfig, state, "plusTwoAmount"));
        cardAlgorithmChoices.Add(plusTwoAmount);

        var plusFourAmount = new Menuitem($"plus four give amount. Current {gameConfig.PlusTwoAmount}",
            () => result = AskInput($"plus four give amount. Current {gameConfig.PlusTwoAmount}", 0, int.MaxValue, gameConfig, state, "plusFourAmount"));
        cardAlgorithmChoices.Add(plusFourAmount);

        var wildAmountInGame = new Menuitem($"Wild cards amount in deck {gameConfig.WildAmountInGame}",
            () => result = AskInput($"Wild cards amount in deck {gameConfig.WildAmountInGame}", 0, int.MaxValue, gameConfig, state, "wildAmountInGame"));
        cardAlgorithmChoices.Add(wildAmountInGame);

        var wildPlusFourAmountInGame = new Menuitem(
            $"Wild+4 cards amount in deck {gameConfig.WildPlusFourAmountInGame}",
            () => result = AskInput($"Wild+4 cards amount in deck {gameConfig.WildPlusFourAmountInGame}", 0, int.MaxValue, gameConfig, state, "wildPlusFourAmountInGame"));
        cardAlgorithmChoices.Add(wildPlusFourAmountInGame);

        var numberDrawChance = new Menuitem($"Chance to draw number from deck. Current {100 - gameConfig.NumberDrawChance}%",
            () => result = AskInput($"Chance to draw number from deck. Current {100 - gameConfig.NumberDrawChance}", 0, 100, gameConfig, state, "numberDrawChance"));
        cardAlgorithmChoices.Add(numberDrawChance);

        var actionCardDrawChance = new Menuitem(
            $"Chance to draw action card from deck. Current {gameConfig.NumberDrawChance - gameConfig.ActionCardDrawChance}%",
            () => result = AskInput(
                $"Chance to draw action card from deck. Current {gameConfig.NumberDrawChance - gameConfig.ActionCardDrawChance}%",
                0, 100, gameConfig, state, "actionCardDrawChance"));
        cardAlgorithmChoices.Add(actionCardDrawChance);

        var wildCardDrawChance = new Menuitem(
            $"Chance to draw wild card from deck. Current {gameConfig.ActionCardDrawChance - gameConfig.WildCardDrawChance}%",
            () => result = AskInput(
                $"Chance to draw wild card from deck. Current {gameConfig.ActionCardDrawChance - gameConfig.WildCardDrawChance}%",
                0, 100, gameConfig, state, "wildCardDrawChance"));
        cardAlgorithmChoices.Add(wildCardDrawChance);

        var wildCardPlusFourDrawChance = new Menuitem(
            $"Chance to draw wild +4 from deck. Current {gameConfig.WildCardDrawChance - gameConfig.WildCardPlusFourDrawChance}%",
            () => result = AskInput(
                $"Chance to draw number from deck. Current {gameConfig.WildCardDrawChance - gameConfig.WildCardPlusFourDrawChance}%",
                0, 100, gameConfig, state, "wildCardDrawChance"));
        cardAlgorithmChoices.Add(wildCardPlusFourDrawChance);

        var zeroSwitch = new Menuitem($"$playing zero will rotate hands. current setting {gameConfig.ZeroSwitch}",
            () => result = AskSwitch($"playing zero will rotate hands. current setting {gameConfig.ZeroSwitch}", gameConfig, "zeroSwitch"));
        actionCardChoices.Add(zeroSwitch);

        var sevenSwitch = new Menuitem(
            $"$playing seven will let you swap hands with someone. current setting {gameConfig.SevenSwitch}",
            () => result = AskSwitch(
                $"playing seven will let you swap hands with someone. current setting {gameConfig.SevenSwitch}", gameConfig, "sevenSwitch"));
        actionCardChoices.Add(sevenSwitch);

        var plusTwoSkip = new Menuitem(
            $"playing +{gameConfig.PlusTwoAmount} will also skip. Current setting {gameConfig.PlusTwoSkip}",
            () => result = AskSwitch(
                $"playing +{gameConfig.PlusTwoAmount} will also skip. Current setting {gameConfig.PlusTwoSkip}", gameConfig, "plusTwoSkip"));
        actionCardChoices.Add(plusTwoSkip);

        var plusFourKSip = new Menuitem(
            $"playing wild+{gameConfig.PlusFourAmount} will also skip. Current setting {gameConfig.PlusFourSKip}",
            () => result = AskSwitch(
                $"playing wild+{gameConfig.PlusFourAmount} will also skip. Current setting {gameConfig.PlusFourSKip}", gameConfig, "plusFourSKip"));
        actionCardChoices.Add(plusFourKSip);

        var stacking = new Menuitem(
            $"if someone sends you +2 or +4 you can forward it to next by playing +2 or +4 {gameConfig.Stacking}",
            () => result = AskSwitch(
                $"if someone sends you +2 or +4 you can forward it to next by playing +2 or +4 {gameConfig.Stacking}", gameConfig, "stacking"));
        actionCardChoices.Add(stacking);

        if (state.Players == null)
        {
            state.Players = new List<Player>();
            for (var i = 0; i < state.MaxPlayers; i++)
            {
                state.Players.Add(new Player());
            }
        }
        else if (state.MaxPlayers > state.Players.Count)
        {
            while (state.MaxPlayers - -1 != state.Players.Count)
            {
                state.Players.Add(new Player());
            }
        }
        else if (state.MaxPlayers < state.Players.Count)
        {
            for (var i = state.Players.Count - 1; i >= state.MaxPlayers; i--)
            {
                state.Players.RemoveAt(i);
            }
        }

        for (var i = 0; i < state.MaxPlayers; i++)
        {
            var current = i;
            var playerName = new Menuitem($"Player{current + 1} name. Current {state.Players[current].Name}",
                () => result = AskName($"Name player {current + 1}. Current {state.Players[current].Name}", state.Players[current]));

            var playerType = new Menuitem($"Player{current + 1} type. Current {state.Players[current].Type}",
                () => result = AskTypeSwitch($"Select Player type. Current {state.Players[current].Name}",
                    state.Players[current]));
            playerChoices.Add(playerName);
            playerChoices.Add(playerType);
        }

        // Back Buttons
        var backToNewGameMenu = new Menuitem("Back", () => result = ELoadMenu.NewGameMenu);
        playerChoices.Add(backToNewGameMenu);
        cardAlgorithmChoices.Add(backToNewGameMenu);
        actionCardChoices.Add(backToNewGameMenu);

        var playerMenu = new Menu("Customise Players", playerChoices.ToArray());
        var actionCardMenu = new Menu("Customise action cards", actionCardChoices.ToArray());
        var cardAlgorithmMenu = new Menu("Customise card algorithm", cardAlgorithmChoices.ToArray());

        var editPlayer = new Menuitem("Edit players", () => { }, playerMenu);
        var editActionCards = new Menuitem("Edit Action Cards", () => { }, actionCardMenu);
        var editCardAlgorithm = new Menuitem("Edit card algorithm", () => { }, cardAlgorithmMenu);
        var applyConfig = new Menuitem("Start new Game", () => result = ELoadMenu.Start);

        var backToMainMenu = new Menuitem("back", () => result = ELoadMenu.MainMenu);

        // New Game Menu
        var newGameMenu = new Menu("Edit game Configurations", editPlayer, editActionCards, editCardAlgorithm, backToMainMenu, applyConfig);

        // LoadMenu
        var saveGameList = gameRepository.GetSaveGames();
        var loadGameChoices = new List<Menuitem>();

        for (var i = 0; i < saveGameList.Count; i++)
        {
            var saveIndex = i;
            loadGameChoices.Add(new Menuitem(saveGameList[saveIndex].ToString(),
                () => result = OverwriteGameState(saveGameList[saveIndex].id, state, gameRepository)));
        }
        loadGameChoices.Add(backToMainMenu);
        var loadMenu = new Menu("Select Game to load", loadGameChoices.ToArray());

        // Main Menu
        var newGame = new Menuitem("Start new game", () => { }, newGameMenu);
        var loadGame = new Menuitem("Load game", () => { }, loadMenu);
        var exitGame = new Menuitem("Exit", () => Environment.Exit(0));

        var mainMenu = new Menu("Welcome to UNO, version 3.5 early access", newGame, loadGame, exitGame);

        switch (menuToLoad)
        {
            case ELoadMenu.MainMenu:
                mainMenu.Display();
                break;
            case ELoadMenu.PlayersMenu:
                playerMenu.Display();
                break;
            case ELoadMenu.ActionCardMenu:
                actionCardMenu.Display();
                break;
            case ELoadMenu.CardAlgorithmMenu:
                cardAlgorithmMenu.Display();
                break;
            case ELoadMenu.NewGameMenu:
                newGameMenu.Display();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(menuToLoad), menuToLoad, null);
        }

        return result;
    }
}