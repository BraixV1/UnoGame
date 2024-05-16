using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace DAL;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("C:\\Users\\Brajan\\Documents\\TalTech\\C#\\UNO\\ConsoleApp\\UNO\\bin\\Debug\\net7.0\\app.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}