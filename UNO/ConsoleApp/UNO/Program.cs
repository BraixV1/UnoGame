using menu;
using MenuItem;
using DAL;
using GameEngine;
using Microsoft.EntityFrameworkCore;
using UNO;
using ConsoleUI;
using GameConfiguration;
using HelperEnums;


var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite("Data Source=app.db")
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;



using var db = new AppDbContext(contextOptions);

db.Database.Migrate();
//IGameRepository gameRepository = new GameRepositoryEF(db);
IGameRepository gameRepository = new GameRepositoryFileSystem();


var freshState = new UnoGameState();
var config = new GameConfiguration.GameConfiguration();
var configCheck = ELoadMenu.MainMenu;
do
{
    configCheck = GameStateSetup.ConfigureGame(config, freshState, gameRepository, configCheck);
} while (configCheck != ELoadMenu.Start && configCheck != ELoadMenu.Load);

if (configCheck != ELoadMenu.Load)
{
    if (config.StartCards * freshState.MaxPlayers > 80) config.LimitBreaker = true;
    freshState.Config = config;
}
else
{
    freshState = gameRepository.LoadGame(freshState.Id);
}

var game = new UnoGame(freshState);



var gameController = new GameController(game, gameRepository);
    
gameController.Run();