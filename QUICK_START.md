# Guía Rápida - Cómo abrir y usar la solución

## 🚀 Primeros pasos en Visual Studio

### 1. Abrir la solución
1. Abre **Visual Studio Community 2026**
2. **File** → **Open** → **Project/Solution**
3. Navega a: `E:\7 semestre\Arquitectura de Software\pActual\Taller_Mecanico_Users\`
4. Selecciona **`Taller_Mecanico_Users.sln`** y haz clic en **Open**

### 2. Explorar la estructura
En el **Solution Explorer** (lado izquierdo) verás:
```
Taller_Mecanico_Users
├── Domain
├── Data
├── UseCases
├── Framework
└── App
```

Cada carpeta es un **proyecto independiente (módulo)** con su propia responsabilidad.

---

## 🔧 Compilar la solución

### Opción 1: Atajo rápido
Presiona: **Ctrl + Shift + B**

### Opción 2: Menú
**Build** → **Build Solution**

**Resultado esperado:** "Build succeeded" (abajo en la ventana de Output)

---

## ▶️ Ejecutar la API

### 1. Establecer App como proyecto de inicio
- En **Solution Explorer**, haz clic derecho en el proyecto **App**
- Selecciona **"Set as Startup Project"**
  - El proyecto se pondrá en **negrita** (confirmación)

### 2. Ejecutar sin debuggear
Presiona: **Ctrl + F5**

O menú: **Debug** → **Start Without Debugging**

### 3. Ejecutar con debuggear (recomendado para desarrollo)
Presiona: **F5**

O menú: **Debug** → **Start Debugging**

**Resultado:** Se abrirá una consola y la API estará escuchando en:
- `https://localhost:5001` (o el puerto que asigne)

---

## 🧪 Probar los endpoints

### Opción 1: Usar curl en PowerShell

Abre **PowerShell** (Windows) y ejecuta:

#### GET - Obtener todos los usuarios
```powershell
curl -X GET https://localhost:5001/api/users -SkipCertificateCheck
```

#### POST - Crear un nuevo usuario
```powershell
$body = @{
    email = "juan@example.com"
    name = "Juan Pérez"
    phone = "1234567890"
} | ConvertTo-Json

curl -X POST https://localhost:5001/api/users `
  -SkipCertificateCheck `
  -Headers @{"Content-Type" = "application/json"} `
  -Body $body
```

#### GET - Obtener usuario por ID
```powershell
curl -X GET https://localhost:5001/api/users/1 -SkipCertificateCheck
```

### Opción 2: Usar Postman o Insomnia
1. Descarga [Postman](https://www.postman.com/downloads/) o [Insomnia](https://insomnia.rest/)
2. Crea un nuevo request:
   - **Method:** GET
   - **URL:** `https://localhost:5001/api/users`
3. Haz clic en **Send**

---

## 📂 Estructura de carpetas y qué editar

### Para agregar un nuevo **Caso de Uso**:

**1. Crear la clase en `UseCases/Users/`**

Ejemplo: `UseCases/Users/UpdateUser.cs`
```csharp
using Domain.Entities;
using Data.Repositories;

namespace UseCases.Users;

public class UpdateUser
{
    private readonly IUserRepository _userRepository;

    public UpdateUser(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> ExecuteAsync(int id, string email, string name, string phone)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"Usuario {id} no encontrado");

        return await _userRepository.UpdateUserAsync(
            new User 
            { 
                Id = id, 
                Email = email, 
                Name = name, 
                Phone = phone,
                CreatedAt = user.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
```

**2. Registrar el caso de uso en `App/Program.cs`**
```csharp
builder.Services.AddScoped<UpdateUser>();
```

**3. Agregar el endpoint en `App/Controllers/UsersController.cs`**
```csharp
[HttpPut("{id}")]
public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
{
    try
    {
        var user = await _updateUser.ExecuteAsync(id, request.Email, request.Name, request.Phone);
        return Ok(UserMapper.ToDto(user));
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(ex.Message);
    }
}
```

---

## 🔍 Entender el flujo de una petición

Cuando haces una petición `GET /api/users/1`:

```
1. Request llega a UsersController.GetUserById()
   ↓
2. Controller llama a: _getUserById.ExecuteAsync(1)
   ↓
3. GetUserById (caso de uso) valida y llama a: _userRepository.GetUserByIdAsync(1)
   ↓
4. UserRepository (Framework) busca en la BD/fuente de datos
   ↓
5. Retorna User (Domain entity)
   ↓
6. Controller convierte a UserDto con UserMapper.ToDto()
   ↓
7. Response vuelve como JSON al cliente
```

**Ventaja:** Si mañana cambias de BD (de in-memory a SQL), solo cambias `Framework/Persistence/UserRepository.cs`, ¡todo lo demás sigue igual!

---

## 🛠️ Tareas comunes en VS

| Tarea | Atajo | Alternativa |
|-------|-------|-------------|
| Compilar solución | `Ctrl + Shift + B` | Build → Build Solution |
| Ejecutar (sin debug) | `Ctrl + F5` | Debug → Start Without Debugging |
| Ejecutar (con debug) | `F5` | Debug → Start Debugging |
| Parar ejecución | `Shift + F5` | Debug → Stop Debugging |
| Ir a definición de símbolo | `F12` | Click derecho → Go to Definition |
| Encontrar referencias | `Shift + F12` | Click derecho → Find All References |
| Formatear documento | `Ctrl + K, Ctrl + D` | Edit → Advanced → Format Document |
| Abrir Terminal | `Ctrl + `` | View → Terminal |
| Solución Explorer | `Ctrl + Alt + L` | View → Solution Explorer |

---

## 📝 Estructura de un módulo

Cada módulo sigue este patrón:

```
ModuleName/
├── ModuleName.csproj
├── Entidad1/
│   └── Clase1.cs
├── Entidad2/
│   └── Clase2.cs
└── Entidad3/
    └── Clase3.cs
```

**Ejemplo en UseCases:**
```
UseCases/
├── UseCases.csproj
└── Users/
    ├── GetUserById.cs
    ├── CreateUser.cs
    ├── GetAllUsers.cs
    └── DeleteUser.cs    ← Agregar aquí casos de uso nuevos
```

---

## ⚡ Tips de Visual Studio

1. **Auto-complete:** Escribe un nombre y presiona `Ctrl + Espacio`
2. **Renombrar símbolo:** Click derecho en un nombre → Rename
3. **Extraer método:** Selecciona código → Ctrl + M, Ctrl + M
4. **Ir a línea:** `Ctrl + G` → ingresa número de línea
5. **Buscar en archivos:** `Ctrl + Shift + F`
6. **Reemplazar en archivos:** `Ctrl + H`

---

## 🐛 Depuración (Debugging)

1. **Establecer breakpoint:** Haz clic en el margen izquierdo de una línea (punto rojo)
2. **Ejecutar con debug:** `F5`
3. **Cuando pause en el breakpoint:**
   - **F10:** Siguiente línea (Step Over)
   - **F11:** Entrar en función (Step Into)
   - **Shift + F11:** Salir de función (Step Out)
   - **F5:** Continuar
4. **Ver variables:** Hover sobre una variable o en la ventana "Locals"

---

## 📦 Instalar dependencias (NuGet)

Si necesitas agregar un paquete NuGet:

1. Click derecho en el proyecto → **Manage NuGet Packages**
2. Busca el paquete (ej: "Entity Framework Core")
3. Haz clic en **Install**

O en la **Package Manager Console:**
```powershell
Install-Package Microsoft.EntityFrameworkCore -Version 10.0.0 -Project Framework
```

---

## 🎉 ¡Listo!

Ahora tienes una arquitectura hexagonal bien estructurada. Puedes:
- ✅ Compilar sin errores
- ✅ Ejecutar la API
- ✅ Probar endpoints
- ✅ Agregar nuevas funcionalidades

¿Preguntas? Revisa el `README.md` para más detalles sobre la arquitectura.
