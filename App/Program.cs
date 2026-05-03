using Data.Repositories;
using Framework.Persistence;
using UseCases.Users;

var builder = WebApplication.CreateBuilder(args);

// Inyección de Dependencias
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Casos de uso
builder.Services.AddScoped<GetUserById>();
builder.Services.AddScoped<CreateUser>();
builder.Services.AddScoped<GetAllUsers>();
builder.Services.AddScoped<DeleteUser>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
