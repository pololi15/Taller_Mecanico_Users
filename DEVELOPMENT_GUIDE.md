# Guía de Desarrollo - Arquitectura Hexagonal

Esta guía te ayuda a agregar nuevas funcionalidades manteniendo la arquitectura limpia.

---

## 📋 Checklist para agregar una nueva funcionalidad

### Ejemplo: Crear un nuevo caso de uso "GetUsersByRole"

#### 1️⃣ **Domain** (Define la entidad)

Si necesitas nuevos campos, modifica `Domain/Entities/User.cs`:

```csharp
public class User
{
    // ...existing code...
    public string Role { get; set; } = "User";  // Nuevo campo
}
```

#### 2️⃣ **Data** (Define el contrato)

En `Data/Repositories/IUserRepository.cs`:

```csharp
public interface IUserRepository
{
    // ...existing methods...
    Task<List<User>> GetUsersByRoleAsync(string role);  // Nuevo método
}
```

#### 3️⃣ **UseCases** (Implementa la lógica)

Crea `UseCases/Users/GetUsersByRole.cs`:

```csharp
using Domain.Entities;
using Data.Repositories;

namespace UseCases.Users;

public class GetUsersByRole
{
    private readonly IUserRepository _userRepository;

    public GetUsersByRole(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<User>> ExecuteAsync(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("El rol es requerido");

        return await _userRepository.GetUsersByRoleAsync(role);
    }
}
```

#### 4️⃣ **Framework** (Implementa el repositorio)

En `Framework/Persistence/UserRepository.cs`:

```csharp
public async Task<List<User>> GetUsersByRoleAsync(string role)
{
    return await Task.FromResult(_users.Where(u => u.Role == role).ToList());
}
```

#### 5️⃣ **App** (Expone el endpoint)

a) Registra el caso de uso en `App/Program.cs`:
```csharp
builder.Services.AddScoped<GetUsersByRole>();
```

b) Inyecta en el controller y crea el endpoint en `App/Controllers/UsersController.cs`:
```csharp
private readonly GetUsersByRole _getUsersByRole;

public UsersController(/* ... */, GetUsersByRole getUsersByRole)
{
    _getUsersByRole = getUsersByRole;
}

[HttpGet("by-role/{role}")]
public async Task<ActionResult<List<UserDto>>> GetUsersByRole(string role)
{
    try
    {
        var users = await _getUsersByRole.ExecuteAsync(role);
        return Ok(users.Select(UserMapper.ToDto).ToList());
    }
    catch (ArgumentException ex)
    {
        return BadRequest(ex.Message);
    }
}
```

#### 6️⃣ **Compilar y probar**

```bash
# Compilar
Ctrl + Shift + B

# Ejecutar
F5

# Probar en PowerShell
curl -X GET https://localhost:5001/api/users/by-role/admin -SkipCertificateCheck
```

---

## 🔄 Flujo de datos

```
Request HTTP
    ↓
UsersController.GetUsersByRole()
    ↓
GetUsersByRole (Caso de Uso)
    ├─ Validación de negocio
    ├─ Lógica específica
    └─ Llama a _userRepository.GetUsersByRoleAsync()
    ↓
UserRepository (Framework)
    ├─ Acceso a BD
    └─ Retorna User[]
    ↓
UserMapper.ToDto() convierte a UserDto[]
    ↓
Response JSON al cliente
```

---

## 🧩 Patrones útiles

### Validación de negocio (en Casos de Uso)

```csharp
public async Task<User> ExecuteAsync(string email)
{
    // Validación básica
    if (string.IsNullOrWhiteSpace(email))
        throw new ArgumentException("Email requerido");

    // Validación de negocio
    var user = await _userRepository.GetUserByEmailAsync(email);
    if (user == null)
        throw new KeyNotFoundException("Usuario no encontrado");

    return user;
}
```

### Manejo de errores (en Controller)

```csharp
try
{
    var result = await _myUseCase.ExecuteAsync(param);
    return Ok(result);
}
catch (ArgumentException ex)
{
    return BadRequest(ex.Message);  // 400
}
catch (KeyNotFoundException ex)
{
    return NotFound(ex.Message);    // 404
}
catch (InvalidOperationException ex)
{
    return Conflict(ex.Message);    // 409
}
catch (Exception ex)
{
    return StatusCode(500, ex.Message);  // 500
}
```

### DTO personalizado con validaciones

```csharp
// Framework/DTOs/Users/CreateUserDto.cs
public class CreateUserDto
{
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Teléfono inválido")]
    public string Phone { get; set; } = string.Empty;
}
```

---

## 🔌 Cambiar de tecnología de datos

### Escenario: Cambiar de in-memory a SQL Server

#### 1. Instalar Entity Framework Core

En `Framework/Framework.csproj`:
```xml
<ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
</ItemGroup>
```

#### 2. Crear DbContext

`Framework/Data/AppDbContext.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Framework.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}
```

#### 3. Actualizar UserRepository

`Framework/Persistence/UserRepository.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using Framework.Data;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // ... resto de métodos
}
```

#### 4. Registrar en App/Program.cs

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
);
```

#### 5. Agregar appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TallerMecanico;Integrated Security=true;"
  }
}
```

**Ventaja:** Solo cambió `Framework/`, los casos de uso y controllers siguen igual. ✅

---

## 📝 DTOs para diferentes operaciones

```csharp
// Create
public class CreateUserRequest
{
    public string Email { get; set; }
    public string Name { get; set; }
}

// Update
public class UpdateUserRequest
{
    public string Email { get; set; }
    public string Name { get; set; }
}

// Response
public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Mapper
public class UserMapper
{
    public static UserDto ToDto(User user) => new() { /* ... */ };
    public static User ToCreate(CreateUserRequest req) => new() { /* ... */ };
    public static void ToUpdate(User user, UpdateUserRequest req) { /* ... */ }
}
```

---

## 🧪 Testing (Próximo paso)

Ejemplo de test unitario sin dependencias reales:

```csharp
// UseCases.Tests/Users/CreateUserTests.cs
using Xunit;
using Moq;
using Data.Repositories;
using UseCases.Users;

public class CreateUserTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_CreatesUser()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserByEmailAsync("test@test.com"))
            .ReturnsAsync((User?)null);

        var useCase = new CreateUser(mockRepo.Object);

        // Act
        var result = await useCase.ExecuteAsync("test@test.com", "Test", "1234567890");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
        mockRepo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingEmail_ThrowsException()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserByEmailAsync("existing@test.com"))
            .ReturnsAsync(new User { Email = "existing@test.com" });

        var useCase = new CreateUser(mockRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => useCase.ExecuteAsync("existing@test.com", "Test", "1234567890")
        );
    }
}
```

---

## 🚀 Próximos pasos recomendados

1. **Implementar actualización (Update):**
   - Crear caso de uso `UpdateUser` en `UseCases/`
   - Implementar en `Framework/Persistence/`
   - Exponer endpoint `PUT /api/users/{id}` en Controller

2. **Agregar autenticación:**
   - Crear entidad `Role` en `Domain/Entities/`
   - Casos de uso: `Login`, `GenerateToken`
   - Middleware en `App/`

3. **Validaciones avanzadas:**
   - FluentValidation en `Framework/Validators/`
   - Reglas de negocio complejas

4. **Testing completo:**
   - Proyecto `UseCases.Tests`
   - Proyecto `Framework.Tests`
   - Proyecto `IntegrationTests`

5. **Documentación API:**
   - Swagger/OpenAPI (volver a agregar)
   - XML comments en métodos públicos

---

## 📞 Referencia rápida

| Acción | Archivo |
|--------|---------|
| Agregar nueva entidad | `Domain/Entities/` |
| Agregar nueva operación de BD | `Data/Repositories/` |
| Agregar lógica de negocio | `UseCases/` |
| Implementar BD | `Framework/Persistence/` |
| Exponer endpoint | `App/Controllers/` |
| Convertir datos | `Framework/Mappers/` |

---

**Recuerda:** Siempre que agregues algo, sigue este flujo: Domain → Data → UseCases → Framework → App ✨
