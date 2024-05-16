namespace GameConfiguration;

public class GameConfiguration
{
    public int StartCards { get; set; } = 6;
    public int PlusTwoAmount { get; set; } = 2;
    public int PlusFourAmount { get; set; } = 4;
    
    public bool SevenSwitch { get; set; } = false;
    public bool ZeroSwitch { get; set; } = false;
    
    public bool PlusTwoSkip { get; set; } = false;
    public bool PlusFourSKip { get; set; } = false;

    public int WildAmountInGame { get; set; } = 2;
    public int WildPlusFourAmountInGame { get; set; } = 2;

    public int NumberDrawChance { get; set; } = 50;
    public int ActionCardDrawChance { get; set; } = 85;
    public int WildCardDrawChance { get; set; } = 95;
    public int WildCardPlusFourDrawChance { get; set; } = 100;
    
    public bool Stacking { get; set; } = false;
    public bool LimitBreaker { get; set; } = false;

    






}