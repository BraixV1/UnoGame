using DAL;
using GameEngine;
using HelperEnums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesController.Pages.PlayGamePage;

public class CardPutPageModel : PageModel
{
    private IGameRepository _gameRepository;
    
    public UnoGame UnoGameEngine { get; set; } = default!;
    
    public CardPutPageModel(IGameRepository repository)
    {
        _gameRepository = repository;
    }
    
    [BindProperty] public Guid GameId { get; set; }

    [BindProperty] public string PlayerName { get; set; }

    [BindProperty] public int CardIndex { get; set; }

    [BindProperty] public string PickCard { get; set; } = "False";

    [BindProperty] public string? ShoutSay { get; set; } = "False";
    
    
    public async Task<IActionResult> OnGetAsync(Guid GameIdNew, string PlayerNameNew, int CardIndexNew, string PickCardNew, string? shoutCheck, string? Shout)
    {

        ShoutSay = Shout;
        GameId = GameIdNew;
        PlayerName = PlayerNameNew;
        CardIndex = CardIndexNew;
        PickCard = PickCardNew;
        
        var gameState = await Task.Run(() => _gameRepository.LoadGame(GameId));
        
        UnoGameEngine = new UnoGame(gameState);
        
        if (UnoGameEngine.GetTurnPlayer().Name != PlayerName)
        {
            return RedirectToPage("./Game", new {GameIdNew = GameId, PlayerNameNew = PlayerName});
        }
        
        
        if (gameState.Progress == Status.Completed)
        {
            return RedirectToPage("./WonPage", new {name = gameState.Players[UnoGameEngine.CheckWin()].Name});
        }
        
        if (shoutCheck != null)
        {
            Console.WriteLine(gameState.Players.Count);
            UnoGameEngine.CheckShout();
            UnoGameEngine.UpdateGameState();
            var state = UnoGameEngine.GetGameState();
            _gameRepository.Save(state.Id, state);
            Console.WriteLine("Shout Check was done");
            return RedirectToPage("./Game", new {GameIdNew = GameId, PlayerNameNew = PlayerName});
        }

        if (ShoutSay != null)
        {
            UnoGameEngine.Shout();
            UnoGameEngine.UpdateGameState();
            var state = UnoGameEngine.GetGameState();
            _gameRepository.Save(state.Id, state);
            Console.WriteLine("Shout was done");
            return RedirectToPage("./Game", new {GameIdNew = GameId, PlayerNameNew = PlayerName});
        }
        

        if (CardIndex == 0 && PickCard.Equals("False"))
        {
            return RedirectToPage("./Game", new {GameIdNew = GameId, PlayerNameNew = PlayerName});
        }
        
        
        if (CardIndex - 1 >= 0 || PickCard.Equals("True"))
        {
            CardIndex--;
            if (UnoGameEngine.GetTurnPlayer().Name.Equals(PlayerName))
            {
                var selected = UnoGameEngine.GetPlayer(PlayerName).GetHand()
                    .OrderBy(x => x.Color)
                    .ThenBy(x => x.Type)
                    .ThenBy(card => card.Number).ToArray();
                EPutCard result;
                if (CardIndex < 0) CardIndex++;
                if (PickCard.Equals("True"))
                {
                    result = EPutCard.TakeNewCard;
                }
                else
                {
                    var card = selected[CardIndex];
                    result = UnoGameEngine.PutCardOnTable(card);
                }
                Console.WriteLine(result);
                switch (result)
                {
                    case EPutCard.PlaySuccess:
                        UnoGameEngine.ProcessTurnIndex();
                        UnoGameEngine.UpdateGameState();
                        _gameRepository.Save(UnoGameEngine.GetGameState().Id, UnoGameEngine.GetGameState());
                        break;
                    case EPutCard.SuccessfulZero:
                        UnoGameEngine.RotateHands();
                        UnoGameEngine.ProcessTurnIndex();
                        break;
                    case EPutCard.SuccessfulSeven:
                        return RedirectToPage("handSwitch", new {id = GameId, name = PlayerName});
                    case EPutCard.TakeNewCard:
                        if (UnoGameEngine.GetGameState().DrawCount > 0)
                        {
                            UnoGameEngine.GiveCard(UnoGameEngine.GetTurnIndex());
                            UnoGameEngine.ProcessTurnIndex();
                        }
                        else
                        {
                            UnoGameEngine.GiveCard(UnoGameEngine.GetTurnIndex());
                        }
                        break;
                    case EPutCard.CallOut:
                        UnoGameEngine.CheckShout();
                        break;
                    case EPutCard.PlayWild:
                        return RedirectToPage("ColorChange");
                    case EPutCard.Failed:
                        return RedirectToPage("./Game", new {GameIdNew = GameId, PlayerNameNew = PlayerName});
                    case EPutCard.Shout:
                        UnoGameEngine.Shout();
                        break;
                    default:
                        throw new Exception($"Result code not implemented: {result}");
                }
                UnoGameEngine.UpdateGameState();
                _gameRepository.Save(UnoGameEngine.GetGameState().Id, UnoGameEngine.GetGameState());
            }
        }
        return RedirectToPage("./Game", new {GameIdNew = GameId, PlayerNameNew = PlayerName});
    }
}