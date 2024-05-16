using CardsEngine;
using DAL;
using GameEngine;
using HelperEnums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PlayerEngine;

namespace RazorPagesController.Pages.PlayGamePage;



public class Game : PageModel
{

    private readonly IGameRepository _gameRepository = default!;

    public string _bgColor;
    public UnoGame UnoGameEngine { get; set; } = default!;



    public Game(IGameRepository repository)
    {
        _gameRepository = repository;
    }

    [BindProperty(SupportsGet = true)] public Guid GameId { get; set; }

    [BindProperty(SupportsGet = true)] public string PlayerName { get; set; }
    
    public async Task<IActionResult> OnGetAsync(Guid? GameIdNew, string? PlayerNameNew)
    {
        GameId = (Guid)GameIdNew;
        PlayerName = PlayerNameNew;
        
        var gameState = await Task.Run(() => _gameRepository.LoadGame(GameId));

        UnoGameEngine = new UnoGame(gameState);
        
        UnoGameEngine.UpdateGameState();

        if (UnoGameEngine.GetTurnPlayer().Type == EPlayertype.AI)
        {
            UnoGameEngine.AiGameTurn();
            UnoGameEngine.ProcessTurnIndex();
        }
        
        UnoGameEngine.UpdateGameState();
        
        _gameRepository.Save(UnoGameEngine.GetGameState().Id, UnoGameEngine.GetGameState());

        return Page();
    }
}