using DAL;
using Database;
using JsonHelper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesController.Pages.LoadGamePage
{
    public class Games : PageModel
    {
        private IGameRepository _repository;

        public Games(IGameRepository repository)
        {
            _repository = repository;
        }

        public IList<UnoGame> Game { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var games = await Task.Run(() => _repository.GetSaveGames());


            Game = new List<UnoGame>();
            
            Console.WriteLine($"Size is {games.Count}");

            foreach (var gameInfo in games)
            {
                var gameState = _repository.LoadGame(gameInfo.id);
                Game.Add(new UnoGame()
                {
                    Status = gameState.Progress.ToString(),
                    Id = gameInfo.id,
                    MaxPlayer = gameState.MaxPlayers,
                    CreatedAtDt = gameInfo.dt,
                    UpdatedAtDt = gameInfo.dt,
                    State = System.Text.Json.JsonSerializer.Serialize(gameState, JsonHelpers.JsonSerializerOptions),
                    Players = gameState.Players.Select(p => new Database.Player()
                    {
                        NickName = p.Name,
                        PlayerType = p.Type,
                        GameId = gameInfo.id,
                    }).ToList(),
                });
            }
            
            
        }
    }
}
