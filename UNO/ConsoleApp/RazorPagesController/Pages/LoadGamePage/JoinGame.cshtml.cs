using DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesController.Pages.LoadGamePage;

public class JoinGame : PageModel
{
    
    private IGameRepository _repository;
    public JoinGame(IGameRepository repository)
    {
        _repository = repository;
    }
    

    public UnoGame Game { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Game = new UnoGame();

        Game.Id = id;
        Game.Players = await Task.Run( () => _repository.LoadGame(id).Players.Select(p => new Database.Player()
        {
            NickName = p.Name,
            PlayerType = p.Type,
            GameId = id,
        }).ToList());
        return Page();
    }
}

