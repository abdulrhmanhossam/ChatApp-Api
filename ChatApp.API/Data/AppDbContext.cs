using ChatApp.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();

        var connecation = configuration
            .GetSection("connectionString")
            .GetSection("DefaultConnection").Value;

        optionsBuilder.UseMySql(connecation, ServerVersion.AutoDetect(connecation));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
