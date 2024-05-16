using Microsoft.EntityFrameworkCore;


namespace DAL;

public class AppDbContext : DbContext
{
    public DbSet<Database.UnoGame> Games { get; set; } = default!;
    public DbSet<Database.Player> Players { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
