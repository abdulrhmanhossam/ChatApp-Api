using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ChatApp.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Data;
public abstract class Seed
{
    public static async Task SeedUsersAsync(AppDbContext dbContext)
    {
        if (await dbContext.Users.AnyAsync())
            return;
        
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        foreach (var user in users)
        {
            using var hmac = new HMACSHA512();

            dbContext.Users.Add(user);
        }

        await dbContext.SaveChangesAsync();
    }
}