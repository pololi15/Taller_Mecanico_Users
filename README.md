# Taller Mecánico - Módulo de Usuarios

## 📋 Arquitectura Hexagonal/Limpia

Esta solución implementa una arquitectura hexagonal monolítica con separación clara de responsabilidades en 5 módulos independientes.

### 🏗️ Estructura de Módulos

```
Taller_Mecanico_Users/
├── Domain/           ← Núcleo de negocio (sin dependencias)
├── Data/             ← Contratos/Interfaces de datos
├── UseCases/         ← Lógica de negocio (Interactors)
├── Framework/        ← Implementación técnica
├── App/              ← API REST (ASP.NET Core)
└── Taller_Mecanico_Users.sln
```

---

## 📚 Descripción de cada módulo

### 1. **Domain** 🎯
**Propósito:** Definir las entidades de negocio puro, sin dependencias externas.

- **Qué contiene:**
  - `Entities/` → Clases de dominio (`User.cs`)

- **Características:**
  - ✅ Sin dependencias de otros proyectos
  - ✅ Representa las "verdades" del negocio
  - ✅ Fácil de testear

- **Ejemplo:**
  ```csharp
  public class User
  {
      public int Id { get; set; }
      public string Email { get; set; }
      public string Name { get; set; }
      // ...
  }
  ```

---

### 2. **Data** 📋
**Propósito:** Definir contratos (interfaces) sin revelar detalles de implementación.

- **Qué contiene:**
  - `Repositories/` → Interfaces de repositorios (`IUserRepository.cs`)

- **Características:**
  - ✅ Define "qué" se puede hacer, no "cómo"
  - ✅ Permite inyección de dependencias
  - ✅ Facilita testing con mocks

- **Ejemplo:**
  ```csharp
  public interface IUserRepository
  {
      Task<User?> GetUserByIdAsync(int id);
      Task<User> CreateUserAsync(User user);
      // ...
  }
  ```

---

### 3. **UseCases** 🎭
**Propósito:** Contener la lógica de negocio específica (Interactors/Acciones).

- **Qué contiene:**
  - `Users/` → Casos de uso (`GetUserById.cs`, `CreateUser.cs`, etc.)

- **Características:**
  - ✅ Una acción = una clase
  - ✅ Orquesta el flujo de datos
  - ✅ Contiene validaciones de negocio
  - ✅ Usa repositorios (no conoce su implementación)

- **Ejemplo:**
  ```csharp
  public class CreateUser
  {
      private readonly IUserRepository _userRepository;

      public async Task<User> ExecuteAsync(string email, string name, string phone)
      {
          // Validaciones de negocio
          if (string.IsNullOrWhiteSpace(email))
              throw new ArgumentException("Email requerido");

          // Lógica específica del caso de uso
          var existingUser = await _userRepository.GetUserByEmailAsync(email);
          if (existingUser != null)
              throw new InvalidOperationException("Email ya existe");

          // Crear usuario
          var user = new User { Email = email, Name = name, Phone = phone };
          return await _userRepository.CreateUserAsync(user);
      }
  }
  ```

---

### 4. **Framework** 🔧
**Propósito:** Implementar los detalles técnicos sucios (BD, APIs, mappers, DTOs).

- **Qué contiene:**
  - `Persistence/` → Implementaciones de repositorios (`UserRepository.cs`)
  - `DTOs/` → Data Transfer Objects (`UserDto.cs`)
  - `Mappers/` → Convertidores entre Domain y DTOs (`UserMapper.cs`)

- **Características:**
  - ✅ Implementa las interfaces de `Data`
  - ✅ Acceso a bases de datos, APIs externas
  - ✅ Convierte datos entre capas
  - ✅ Fácil de cambiar (ej: de Firebase a SQL)

- **Ejemplos:**
  ```csharp
  // Implementación del repositorio
  public class UserRepository : IUserRepository
  {
      public async Task<User?> GetUserByIdAsync(int id)
      {
          // Acceso a BD aquí
          return await _context.Users.FindAsync(id);
      }
  }

  // DTO para transferencia
  public class UserDto
  {
      public int Id { get; set; }
      public string Email { get; set; }
  }

  // Mapper
  var dto = UserMapper.ToDto(user);
  ```

---

### 5. **App** 🚀
**Propósito:** API REST (ASP.NET Core) - punto de entrada de la aplicación.

- **Qué contiene:**
  - `Controllers/` → Endpoints REST (`UsersController.cs`)
  - `Program.cs` → Configuración y DI (Dependency Injection)
  - `appsettings.json` → Configuración

- **Características:**
  - ✅ Maneja requests/responses HTTP
  - ✅ Injección de dependencias
  - ✅ Middleware y configuración
  - ✅ Usa casos de uso de `UseCases`

- **Ejemplo:**
  ```csharp
  [ApiController]
  [Route("api/[controller]")]
  public class UsersController : ControllerBase
  {
      private readonly GetUserById _getUserById;

      [HttpGet("{id}")]
      public async Task<ActionResult<UserDto>> GetUserById(int id)
      {
          var user = await _getUserById.ExecuteAsync(id);
          return Ok(UserMapper.ToDto(user));
      }
  }
  ```

---

## 🔄 Flujo de Dependencias

```
App (Presentación/API)
  ↓
UseCases (Lógica de negocio)
  ↓
Data (Contratos)
  ↓
Domain (Entidades puras)

Framework (implementa Data)
```

**Regla de Oro:** Las dependencias siempre apuntan hacia el interior (hacia Domain)

---

## 📡 Endpoints disponibles

### `GET /api/users/{id}`
Obtiene un usuario por ID.

```bash
curl -X GET http://localhost:5000/api/users/1
```

**Response:**
```json
{
  "id": 1,
  "email": "juan@example.com",
  "name": "Juan Pérez",
  "phone": "1234567890",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

---

### `GET /api/users`
Obtiene todos los usuarios.

```bash
curl -X GET http://localhost:5000/api/users
```

---

### `POST /api/users`
Crea un nuevo usuario.

```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "email": "carlos@example.com",
    "name": "Carlos López",
    "phone": "9876543210"
  }'
```

---

## 🧪 Cómo extender con una base de datos real

Actualmente, el `UserRepository` usa una lista en memoria. Para migrar a SQL/Entity Framework:

### 1. Actualizar `Framework/Framework.csproj`
```xml
<ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
</ItemGroup>
```

### 2. Crear `Framework/Data/AppDbContext.cs`
```csharp
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("your-connection-string");
    }
}
```

### 3. Actualizar `Framework/Persistence/UserRepository.cs`
```csharp
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    // ... rest de métodos
}
```

### 4. Registrar DbContext en `App/Program.cs`
```csharp
builder.Services.AddDbContext<AppDbContext>();
```

**Ventaja:** Solo cambias el `Framework`, todo lo demás sigue igual.

---

## ✅ Beneficios de esta arquitectura

| Beneficio | Descripción |
|-----------|-------------|
| **Mantenibilidad** | Código organizado y fácil de modificar |
| **Testabilidad** | Cada capa es independiente y mockeable |
| **Escalabilidad** | Agregar nuevos casos de uso es simple |
| **Reusabilidad** | Los casos de uso pueden usarse en múltiples interfaces (Web, Mobile, CLI) |
| **Flexibilidad** | Cambiar de tecnología (BD, API, etc.) sin tocar la lógica de negocio |
| **Independencia de frameworks** | La lógica de negocio no depende de ASP.NET, EF, etc. |

---

## 📝 Próximos pasos

1. **Abrir la solución en Visual Studio:**
   - Haz doble clic en `Taller_Mecanico_Users.sln`

2. **Compilar la solución:**
   - `Ctrl + Shift + B`

3. **Ejecutar la API:**
   - Selecciona `App` como proyecto de inicio
   - `F5` para debuggear o `Ctrl + F5` para ejecutar sin debuggear

4. **Probar con Swagger:**
   - Abre `https://localhost:5001/swagger` (o el puerto que asigne)

---

## 📚 Referencias y conceptos

- **Domain-Driven Design (DDD):** Enfoque para modelar el dominio del negocio
- **Clean Architecture:** Separación de responsabilidades en capas independientes
- **Hexagonal Architecture (Puertos y Adaptadores):** Aislar la lógica del negocio de frameworks
- **Repository Pattern:** Abstracción del acceso a datos
- **Dependency Injection:** Inversión de control para flexibilidad
- **Use Case Pattern:** Cada acción de negocio es una clase independiente

---

## 🤝 Contribuir

Para agregar nuevas funcionalidades:

1. Define la entidad en `Domain/Entities/`
2. Crea la interfaz del repositorio en `Data/Repositories/`
3. Implementa casos de uso en `UseCases/`
4. Implementa el repositorio en `Framework/Persistence/`
5. Agrega endpoints en `App/Controllers/`

---

**Creado:** Enero 2025  
**Arquitectura:** Hexagonal/Limpia monolítica  
**Framework:** .NET 10
