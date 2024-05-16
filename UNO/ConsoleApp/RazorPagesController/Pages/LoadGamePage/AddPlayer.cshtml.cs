using System.Text.Json;
using JsonHelper;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlayerEngine;

namespace RazorPagesController.Pages.LoadGamePage;
using DAL;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GameConfiguration;

public class AddPLayerModel : PageModel
{
    
    private readonly IGameRepository _gameRepository;
    
    public AddPLayerModel(IGameRepository repository)
    {
        _gameRepository = repository;
    }
    
    [BindProperty]
    public Database.UnoGame Game { get; set; } = default!;

    [BindProperty] public Guid Gameid { get; set; } = default;

    [BindProperty] public UnoGameState State { get; set; } = default!;

    [BindProperty] public Player _player { get; set; } = new Player();

    [BindProperty] public string name { get; set; } = default;
    
    public SelectList Options { get; set; }
    
    [BindProperty] public string Type { get; set; } = "False";
    
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Gameid = id;

        State = await Task.Run(() => _gameRepository.LoadGame(id));
        
        var optionsList = new List<SelectListItem>
        {
            new SelectListItem { Value = "AI", Text = "AI" },
            new SelectListItem { Value = "Human", Text = "Human" }
        };
        
        
        Options = new SelectList(optionsList, "Value", "Text");
        
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        _player.Name = name;
        if (Type.Equals("Human"))
        {
            _player.Type = EPlayertype.Human;
        }
        else
        {
            _player.Type = EPlayertype.AI;
        }

        var state = await Task.Run(() => _gameRepository.LoadGame(Gameid));

        if (!state.Players.Any(m => m.Name.Equals(_player.Name)))
        {
            state.Players.Add(_player);
        }
        
        _gameRepository.Save(Gameid, state);
        
        return RedirectToPage("./Edit", new {id = Game.Id});
    }
}