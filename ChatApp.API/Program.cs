using ChatApp.API.Data;
using ChatApp.API.Interfaces;
using ChatApp.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddCors();

builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(
    builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins()
    );

app.MapControllers();

app.Run();
