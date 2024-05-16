namespace GameEngine;
using PlayerEngine;
using CardsEngine;

public interface IUnoGameUI
{
    Player GetCurrentPlayer();

    Player getOnTable();

    Card? GetOnTable();
}