using DAL;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesController.Pages.LoadGamePage;

public class StartModel : PageModel
{
    
    private IGameRepository _repository;
    
    public StartModel(IGameRepository repository)
    {
        _repository = repository;
    }
    
    [BindProperty]
    public Guid GameId { get; set; }
    
    public async Task<IActionResult> OnGet(Guid id)
    {
        GameId = id;
        
        var state = await Task.Run(() => _repository.LoadGame(GameId));
        state.Progress = Status.Playing;
        
        Console.WriteLine(state.Progress);
        
        _repository.Save(GameId, state);
        
        return RedirectToPage("./JoinGame", new {id = GameId});
    }
}