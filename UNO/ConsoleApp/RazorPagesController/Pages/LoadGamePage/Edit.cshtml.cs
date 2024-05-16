using System.Text.Json;
using JsonHelper;
using NuGet.Protocol.Core.Types;

namespace RazorPagesController.Pages.LoadGamePage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using GameEngine;
using RazorPagesController.Pages.PlayGamePage;
using Database;
using GameConfiguration;



public class EditModel : PageModel
{

    
    private readonly IGameRepository _gameRepository = default!;

    public EditModel(IGameRepository repository)
    {
        _gameRepository = repository;
    }

    [BindProperty]
    public Database.UnoGame Game { get; set; } = default!;

    [BindProperty] 
    public GameConfiguration Config { get; set; } = new GameConfiguration();

    [BindProperty] public UnoGameState State { get; set; } = default!;

    [BindProperty] public List<PlayerEngine.Player> Players { get; set; } = default;

    
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        State = await Task.Run(() => _gameRepository.LoadGame(id));
        if (State.Progress != Status.Starting)
        {
            return RedirectToPage("./Games");
        }
        Config = State.Config;
        Players = State.Players;
        
        Game = new Database.UnoGame()
        {
            MaxPlayer = State.MaxPlayers,
            Id = id,
            State = System.Text.Json.JsonSerializer.Serialize(State, JsonHelpers.JsonSerializerOptions),
            Players = State.Players.Select(p => new Database.Player()
            {
                NickName = p.Name,
                PlayerType = p.Type,
                GameId = State.Id,
            }).ToList(),
        };
        return Page();
    }
    
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        var players = await Task.Run(() => _gameRepository.LoadGame(Game.Id).Players);
        State.Config = Config;
        State.Players = players;
        Game.State = JsonSerializer.Serialize(State, JsonHelpers.JsonSerializerOptions);
        
        
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        _gameRepository.Save(State.Id, State);

        return Page();
    }
}