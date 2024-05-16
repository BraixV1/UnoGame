using System.Text.Json;
using GameEngine;
using JsonHelper;


namespace DAL;

public class GameRepositoryEF : IGameRepository
{
    private readonly AppDbContext _ctx;

    public GameRepositoryEF(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    
    public void Save(Guid id, UnoGameState state)
    {
        var game = _ctx.Games.FirstOrDefault(g => g.Id == state.Id);
        if (game == null)
        {
            game = new Database.UnoGame()
            {
                
                Id = state.Id,
                MaxPlayer = state.MaxPlayers,
                State = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions),
                Players = state.Players.Select(p => new Database.Player()
                {
                    NickName = p.Name,
                    PlayerType = p.Type
                }).ToList()
            };
            switch (state.Progress)
            {
                case Status.Starting:
                    game.Status = "Starting";
                    break;
                case Status.Completed:
                    game.Status = "Completed";
                    break;
                case Status.Playing:
                    game.Status = "Playing";
                    break;
            }
            _ctx.Games.Add(game);
        }
        else
        {
            switch (state.Progress)
            {
                case Status.Starting:
                    game.Status = "Starting";
                    break;
                case Status.Completed:
                    game.Status = "Completed";
                    break;
                case Status.Playing:
                    game.Status = "Playing";
                    break;
            }
            game.UpdatedAtDt = DateTime.Now;
            game.State = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);
        }

        var changeCount = _ctx.SaveChanges();
        Console.WriteLine("SaveChanges: " + changeCount);

    }

    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        return _ctx.Games
            .OrderByDescending(g => g.UpdatedAtDt)
            .ToList()
            .Select(g => (g.Id, g.UpdatedAtDt))
            .ToList();
    }

    public UnoGameState LoadGame(Guid id)
    {
        var game = _ctx.Games.First(g => g.Id == id);
        return JsonSerializer.Deserialize<UnoGameState>(game.State, JsonHelpers.JsonSerializerOptions)!;
    }
}