using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ChatApp.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Data;
public abstract class Seed
{
    public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
    {
        if (await userManager.Users.AnyAsync())
            return;
        
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        if (users == null)
            return;

        foreach (var user in users)
        {
           await userManager.CreateAsync(user, "Pa$$w0rd");
        }
    }
}