using ChatApp.API.Extensions;
using ChatApp.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerServices(builder.Configuration);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExecptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
