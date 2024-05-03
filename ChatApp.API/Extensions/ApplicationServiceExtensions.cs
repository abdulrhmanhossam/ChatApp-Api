using ChatApp.API.Data;
using ChatApp.API.Data.Repositories;
using ChatApp.API.Interfaces;
using ChatApp.API.Services;

namespace ChatApp.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>();

            services.AddCors();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            return services;
        }
    }
}
