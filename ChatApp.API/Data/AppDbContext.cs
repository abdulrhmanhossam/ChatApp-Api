using ChatApp.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : 
    IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>,
    AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
{
    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();

        var connecation = configuration
            .GetSection("connectionString")
            .GetSection("DefaultConnection").Value;

        optionsBuilder.UseSqlServer(connecation);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        //modelBuilder.Entity<UserLike>()
        //    .HasKey(k => new {k.SourceUserId, k.TargetUserId});

        //modelBuilder.Entity<UserLike>()
        //    .HasOne(s => s.SourceUser)
        //    .WithMany(l => l.LikedUsers)
        //    .HasForeignKey(f => f.SourceUserId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<UserLike>()
        //    .HasOne(t => t.TargetUser)
        //    .WithMany(l => l.LikedByUsers)
        //    .HasForeignKey(f => f.TargetUserId)
        //    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
