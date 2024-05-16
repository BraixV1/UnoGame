using DAL;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesController.Pages.LoadGamePage;

public class DeletePlayerModel : PageModel
{

    private IGameRepository _gameRepository;

    public DeletePlayerModel(IGameRepository repository)
    {
        _gameRepository = repository;
    }
    
    [BindProperty] private UnoGameState State { get; set; }
    
    [BindProperty] private Guid GameId { get; set; }
    
    [BindProperty] private string Playername { get; set; }
    
    public async Task<IActionResult> OnGetAsync(Guid Gameid, string? PlayerName)
    {
        GameId = Gameid;

        Playername = PlayerName;
        
        State = await Task.Run(() => _gameRepository.LoadGame(GameId));

        for (var i = 0; i < State.Players.Count; i++)
        {
            if (State.Players[i].Name != PlayerName) continue;
            State.Players.Remove(State.Players[i]);
            break;
        }
        
        _gameRepository.Save(State.Id, State);

        return RedirectToPage("./Edit", new { id = GameId });
    }
}