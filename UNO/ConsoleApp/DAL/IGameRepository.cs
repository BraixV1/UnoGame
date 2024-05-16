namespace DAL;
using GameEngine;

public interface IGameRepository
{
    void Save(Guid id, UnoGameState state);
    List<(Guid id, DateTime dt)> GetSaveGames();

    UnoGameState LoadGame(Guid id);
}