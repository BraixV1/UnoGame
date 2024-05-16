using System.ComponentModel.DataAnnotations;


namespace Database;
using PlayerEngine;


public class Player : BaseEntity
{
    [MaxLength(128)]
    public string NickName { get; set; } = default!;

    public EPlayertype PlayerType { get; set; }

    // use convenience naming <class>Id
    // nullability decides relationship type - mandatory or not
    public Guid GameId { get; set; }
    public UnoGame? Game { get; set; }
}


