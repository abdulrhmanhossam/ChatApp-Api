using ChatApp.API.Data;
using ChatApp.API.Data.Repositories;
using ChatApp.API.Helpers;
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
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<LogUserActivity>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.Configure<ColudinarySettings>(configuration.GetSection("ColudinarySettings"));
            
            return services;
        }
    }
}
