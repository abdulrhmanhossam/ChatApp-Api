using ChatApp.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddCors();

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
