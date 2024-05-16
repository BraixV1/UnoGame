using CardsEngine;
using DAL;
using Database;
using GameEngine;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UnoGame = GameEngine.UnoGame;

namespace RazorPagesController.Pages.PlayGamePage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
public class ColorChangeModel : PageModel
{
    

    private readonly IGameRepository _gameRepository = default!;
    
    public ColorChangeModel(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    
    [BindProperty] public Guid GameIdReal { get; set; }

    [BindProperty] public string PlayerNameSaved { get; set; }

    [BindProperty] public int CardIndexSaved { get; set; }

    [BindProperty] public string PickCardSaved { get; set; } = "False";


    
    [BindProperty] public string Color { get; set; } = "False";
    
    public SelectList Options { get; set; }
    
    

    public async Task<IActionResult> OnPostAsync()
    {
        /*
        Console.WriteLine($"GameID: {GameIdReal}");
        Console.WriteLine($"CardIndexSaved: {CardIndexSaved}");
        Console.WriteLine($"PickCardSaved: {PickCardSaved}");
        Console.WriteLine($"PlayerNameSaved: {PlayerNameSaved}");
        */
        var state = await Task.Run(() => _gameRepository.LoadGame(GameIdReal));
        var unoGameEngine = new UnoGame(state);
        var ordered = unoGameEngine.GetPlayer(PlayerNameSaved).GetHand()
            .OrderBy(x => x.Color)
            .ThenBy(x => x.Type)
            .ThenBy(card => card.Number).ToArray();
        Console.WriteLine(unoGameEngine.PutCardOnTable(ordered[CardIndexSaved-1]));
        
        switch (Color)
        {
            case("Red"):
                unoGameEngine.ChangeTableColor(ColorType.Red);
                break;
            case("Blue"):
                unoGameEngine.ChangeTableColor(ColorType.Blue);
                break;
            case("Yellow"):
                unoGameEngine.ChangeTableColor(ColorType.Yellow);
                break;
            case("Green"):
                unoGameEngine.ChangeTableColor(ColorType.Green);
                break;
        }
        unoGameEngine.ProcessTurnIndex();
        unoGameEngine.UpdateGameState();
        _gameRepository.Save(unoGameEngine.GetGameState().Id, unoGameEngine.GetGameState());
        return RedirectToPage("Game", new {GameIdNew = GameIdReal, PlayerNameNew = PlayerNameSaved, CardIndexNew = 0, PickCardNew = "False"});
        // return Page();
    }
    
    
    public async Task<IActionResult> OnGetAsync(Guid GameId, string PlayerName, int CardIndex, string PickCard)
    {
         GameIdReal= GameId;
         PlayerNameSaved = PlayerName;
         CardIndexSaved = CardIndex;
         PickCardSaved = PickCard;
         var state = await Task.Run(() => _gameRepository.LoadGame(GameIdReal));

         var unoGameEngine = new UnoGame(state);
         if (unoGameEngine.GetTurnPlayer().Name != PlayerNameSaved)
         {
             return RedirectToPage("Game", new {GameIdNew = GameIdReal, PlayerNameNew = PlayerNameSaved, CardIndexNew = 0, PickCardNew = "False"});
         }
         
         var optionsList = new List<SelectListItem>
         {
             new SelectListItem { Value = "Red", Text = "Red" },
             new SelectListItem { Value = "Blue", Text = "Blue" },
             new SelectListItem { Value = "Yellow", Text = "Yellow" },
             new SelectListItem { Value = "Green", Text = "Green" },
         };

         Options = new SelectList(optionsList, "Value", "Text");

         return Page();
    }
}