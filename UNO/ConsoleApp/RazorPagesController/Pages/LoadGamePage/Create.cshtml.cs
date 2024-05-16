namespace RazorPagesController.Pages.LoadGamePage;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DAL;
using GameEngine;
public class CreateModel : PageModel
{
    private IGameRepository _gameRepository;
    

    public CreateModel(IGameRepository repository)
    {
        _gameRepository = repository;
    }

    public async Task<IActionResult>? OnGetAsync()
    {
        var state = await Task.Run(() => new UnoGameState());
        _gameRepository.Save(state.Id, state);
        return RedirectToPage("./Edit", new {id = state.Id});
    }
}