using System.Text;
using GameEngine;

namespace ConsoleUI.Visualization;

public static class ConsoleVisualization
{
    public static void DrawTurn(UnoGameState gameState)
    {
        Console.OutputEncoding = Encoding.Unicode;
        for (var i = 0; i < gameState.Players.Count; i++)
        {
            if (i == gameState.TurnIndex)
            {
                continue;
            }
            Console.WriteLine($"{gameState.Players[i].Name} has {gameState.Players[i].Hand.Count} cards {gameState.Players[i].Shout}");
        }
        Console.WriteLine("---------------------------------------");
        if (gameState.OnTable == null) Console.WriteLine("EMPTY");
        if (gameState.OnTable != null) Console.WriteLine(gameState.OnTable.ToString());
        
        Console.WriteLine("--------------------------------------");
        Console.WriteLine($"Your hand: {gameState.Players[gameState.TurnIndex].GetHandAsString()}");
    }
    
}