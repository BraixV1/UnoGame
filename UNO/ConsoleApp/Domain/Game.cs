namespace Database;


public class UnoGame : BaseEntity
{
    public DateTime CreatedAtDt { get; set; } = DateTime.Now;
    public DateTime UpdatedAtDt { get; set; } = DateTime.Now;

    public string Status { get; set; } = "Starting";

    public int MaxPlayer { get; set; } = 0;
    
    public string State { get; set; } = "";

    // null, if you did not do the join (.include in c#)
    public ICollection<Player>? Players { get; set; }
}