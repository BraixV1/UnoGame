using CardsEngine;
using DAL;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RazorPagesController.Pages.PlayGamePage;

public class HandSwitchModel : PageModel
{

    private readonly IGameRepository _gameRepository = default!;

    
    public HandSwitchModel(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;

    }
    
    [BindProperty] public int PlayerIndex { get; set; }
    
    [BindProperty] public Guid GameId { get; set; }

    [BindProperty] public string PlayerName { get; set; }

    
    public SelectList Options { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var state = await Task.Run(() => _gameRepository.LoadGame(GameId));
        var engine = new UnoGame(state);
        engine.SwitchHand(PlayerIndex);
        engine.ProcessTurnIndex();
        engine.UpdateGameState();
        _gameRepository.Save(engine.GetGameState().Id, engine.GetGameState());
        return RedirectToPage("Game", new {GameIdNew = GameId, PlayerNameNew = PlayerName, CardIndexNew = 0, PickCardNew = "False"});
    }
     
    public async Task<IActionResult> OnGet(Guid id, string name)
    {

        GameId = id;
        PlayerName = name;

        var state = await Task.Run(() => _gameRepository.LoadGame(GameId));

        var players = state.Players;

        var optionsList = new List<SelectListItem>();
        for (var i = 0; i < players.Count; i++)
        {
            if (players[i].Name.Equals(PlayerName)) continue;
            optionsList.Add(new SelectListItem { Value = $"{i}", Text = players[i].Name});
        }

        Options = new SelectList(optionsList, "Value", "Text");
        return Page();
    }
}