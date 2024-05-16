using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json;
using Database;
using GameEngine;
using JsonHelper;

namespace DAL;

public class GameRepositoryFileSystem : IGameRepository
{
    // TODO: figure out system dependent location - maybe Path.GetTempPath()
    private const string SaveLocation = "C:\\Users\\Brajan\\Documents\\TalTech\\icd0008-23f\\UNO\\ConsoleApp\\UNO\\bin\\Debug\\net8.0\\savedGames";

    public void Save(Guid id, UnoGameState state)
    {
        var content = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);

        var fileName = Path.ChangeExtension(id.ToString(), ".json");

        if (!Path.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }

        File.WriteAllText(Path.Combine(SaveLocation, fileName), content);
    }

    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        // Create directory if it doesn't exist
        if (!Directory.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }

        var data = Directory.EnumerateFiles(SaveLocation);
        var res = data
            .Select(
                path => (
                    Guid.Parse(Path.GetFileNameWithoutExtension(path)),
                    File.GetLastWriteTime(path)
                )
            ).ToList();

        return res;
    }

    public UnoGameState LoadGame(Guid id)
    {
        var fileName = Path.ChangeExtension(id.ToString(), ".json");

        var jsonStr = File.ReadAllText(Path.Combine(SaveLocation, fileName));
        var res = JsonSerializer.Deserialize<UnoGameState>(jsonStr, JsonHelpers.JsonSerializerOptions);
        if (res == null) throw new SerializationException($"Cannot deserialize {jsonStr}");

        return res;
    }
}