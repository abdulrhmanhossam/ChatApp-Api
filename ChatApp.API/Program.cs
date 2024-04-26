using ChatApp.API.Data;
using ChatApp.API.Extensions;
using ChatApp.API.Interfaces;
using ChatApp.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors(
    builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins()
    );

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
