var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// DI registrations - register repository, auth helper and connection factory
builder.Services.AddScoped<Taller_Mecanico_Users.Framework.Persistence.ISqlConnectionFactory, Taller_Mecanico_Users.App.Infrastructure.SqlConnectionFactory>();
builder.Services.AddScoped<Taller_Mecanico_Users.Domain.Ports.IUsuarioLoginRepository, Taller_Mecanico_Users.Data.Repositories.UsuarioLoginRepository>();
builder.Services.AddScoped<Taller_Mecanico_Users.Framework.Services.IAuthenticationHelper, Taller_Mecanico_Users.App.Services.AuthenticationHelper>();
builder.Services.AddHttpContextAccessor();

// Allow configuring connection string for Docker DB via environment or appsettings
// Set environment variable: ConnectionStrings__DefaultConnection

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
