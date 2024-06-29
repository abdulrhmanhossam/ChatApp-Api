using ChatApp.API.Extensions;
using ChatApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChatApp.API.Helpers;
public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (context.HttpContext.User.Identity?.IsAuthenticated != true) 
            return;

        var username = resultContext.HttpContext.User
            .GetUserName();

        var repo = resultContext.HttpContext.RequestServices
            .GetRequiredService<IUserRepository>();

        var user = await repo
            .GetUserByUsernameAsync(username);

        if (user == null) 
            return;

        user.LastActive = DateTime.UtcNow;

        await repo.SaveAllAsync();
    }
}