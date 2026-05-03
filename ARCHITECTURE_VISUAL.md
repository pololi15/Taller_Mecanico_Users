# 🏗️ Arquitectura Visual

## Estructura de directorios

```
Taller_Mecanico_Users/
│
├── 📄 Taller_Mecanico_Users.sln              ← Abre esto en VS
│
├── 📁 Domain/                                 (Layer 5 - innermost)
│   ├── Domain.csproj                         ← Proyecto C#
│   └── Entities/
│       └── User.cs                           ← Entidad de negocio
│
├── 📁 Data/                                   (Layer 4)
│   ├── Data.csproj
│   └── Repositories/
│       └── IUserRepository.cs                ← Contrato (interfaz)
│
├── 📁 UseCases/                               (Layer 3)
│   ├── UseCases.csproj
│   └── Users/
│       ├── GetUserById.cs
│       ├── CreateUser.cs
│       ├── GetAllUsers.cs
│       └── DeleteUser.cs
│
├── 📁 Framework/                              (Layer 2)
│   ├── Framework.csproj
│   ├── Persistence/
│   │   └── UserRepository.cs                 ← Implementación
│   ├── DTOs/
│   │   └── Users/
│   │       └── UserDto.cs
│   └── Mappers/
│       └── UserMapper.cs
│
├── 📁 App/                                    (Layer 1 - Outermost)
│   ├── App.csproj
│   ├── Program.cs                            ← Entry point
│   ├── Controllers/
│   │   └── UsersController.cs                ← Endpoints
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── 📄 README.md                               ← Documentación
├── 📄 QUICK_START.md                          ← Guía para VS
├── 📄 DEVELOPMENT_GUIDE.md                    ← Cómo desarrollar
├── 📄 SETUP_COMPLETE.md                       ← Este setup
└── 📄 .gitignore                              ← Git config

```

---

## 🔗 Dependencias entre proyectos

```
     App (ASP.NET Core)
      ↓↓↓
   UseCases
   ↙  ↓  ↘
Domain  Data  Framework
       ↓
    Domain
```

**Regla:** Las flechas siempre apuntan hacia Domain (el centro)

| Proyecto | Depende de | Razón |
|----------|-----------|-------|
| App | UseCases, Framework, Data, Domain | Orquesta todo |
| UseCases | Data, Domain | Usa interfaces y entidades |
| Framework | Data, Domain | Implementa interfaces con Domain |
| Data | Domain | Solo define interfaces que usan Domain |
| Domain | NADA | Es independiente |

---

## 📊 Responsabilidades

### 🟢 Domain (Verde - Núcleo)
```
RESPONSABILIDAD: Entidades de negocio puro
CONTIENE:
  - User.cs (modelo de datos)
DEPENDE DE:
  - NADA (sin dependencias externas)
USADO POR:
  - Todos los demás módulos
```

### 🔵 Data (Azul - Contratos)
```
RESPONSABILIDAD: Definir qué operaciones se pueden hacer
CONTIENE:
  - IUserRepository.cs (interfaz)
DEPENDE DE:
  - Domain
USADO POR:
  - UseCases, Framework
```

### 🟡 UseCases (Amarillo - Lógica)
```
RESPONSABILIDAD: Lógica de negocio específica
CONTIENE:
  - GetUserById.cs
  - CreateUser.cs
  - GetAllUsers.cs
  - DeleteUser.cs
DEPENDE DE:
  - Data (interfaces), Domain (entidades)
USADO POR:
  - App (controller)
```

### 🟣 Framework (Púrpura - Técnico)
```
RESPONSABILIDAD: Implementación técnica y detalles sucios
CONTIENE:
  - UserRepository.cs (implementación)
  - UserDto.cs (transfer objects)
  - UserMapper.cs (conversión)
DEPENDE DE:
  - Data (interfaces a implementar), Domain (entidades)
USADO POR:
  - App (inyección)
```

### 🔴 App (Rojo - Presentación)
```
RESPONSABILIDAD: Endpoint HTTP y entrada de la aplicación
CONTIENE:
  - UsersController.cs (endpoints)
  - Program.cs (configuración)
DEPENDE DE:
  - UseCases, Framework, Data, Domain
USADO POR:
  - Clientes HTTP
```

---

## 🔄 Ciclo de vida de una petición

### Request: `GET /api/users/1`

```
1️⃣  ENTRADA (App)
    └─ HTTP GET /api/users/1
       └─ UsersController.GetUserById(1)

2️⃣  ORQUESTACIÓN (UseCases)
    └─ _getUserById.ExecuteAsync(1)
       ├─ Validar: id > 0? ✓
       └─ Llama: _userRepository.GetUserByIdAsync(1)

3️⃣  IMPLEMENTACIÓN (Framework)
    └─ UserRepository.GetUserByIdAsync(1)
       ├─ Busca en lista de usuarios
       └─ Retorna: User { id: 1, ... }

4️⃣  CONVERSIÓN (Framework)
    └─ UserMapper.ToDto(user)
       └─ Retorna: UserDto { id: 1, ... }

5️⃣  SALIDA (App)
    └─ HTTP 200 OK
       └─ JSON: { "id": 1, "email": "...", ... }
```

---

## 💾 Flujo de datos

### Create User: `POST /api/users`

```json
Request Body:
{
  "email": "juan@example.com",
  "name": "Juan",
  "phone": "123456"
}
        ↓
CreateUser.ExecuteAsync(email, name, phone)
        ↓
[Validar email no exista]
        ↓
new User { Email=..., Name=..., Phone=... }
        ↓
_userRepository.CreateUserAsync(user)
        ↓
UserRepository agrega a lista
        ↓
Retorna User con Id asignado
        ↓
UserMapper.ToDto(user)
        ↓
Response: HTTP 201 Created
{
  "id": 1,
  "email": "juan@example.com",
  "name": "Juan",
  "phone": "123456",
  "createdAt": "2024-01-15..."
}
```

---

## 📦 Mapeo de archivos

### Domain
```
Domain.csproj
└── Entities/
    └── User.cs                 (Entidad principal)
```
**Qué tiene:** Solo clases C# con propiedades  
**Qué NO tiene:** Validaciones complejas, lógica de BD

### Data
```
Data.csproj
└── Repositories/
    └── IUserRepository.cs      (Interfaz de operaciones)
```
**Qué tiene:** Definición de métodos sin implementar  
**Qué NO tiene:** Implementación real

### UseCases
```
UseCases.csproj
└── Users/
    ├── GetUserById.cs          (Obtener uno)
    ├── CreateUser.cs           (Crear
    ├── GetAllUsers.cs          (Obtener todos)
    └── DeleteUser.cs           (Eliminar)
```
**Qué tiene:** Casos de uso con lógica de negocio  
**Qué NO tiene:** Acceso a BD, HTTP

### Framework
```
Framework.csproj
├── Persistence/
│   └── UserRepository.cs       (Implementación real)
├── DTOs/
│   └── Users/
│       └── UserDto.cs          (Objeto de transferencia)
└── Mappers/
    └── UserMapper.cs           (Conversión)
```
**Qué tiene:** Implementación, DTOs, mappers  
**Qué NO tiene:** Lógica de negocio

### App
```
App.csproj
├── Program.cs                  (Configuración de la app)
└── Controllers/
    └── UsersController.cs      (Endpoints HTTP)
```
**Qué tiene:** Endpoints y configuración  
**Qué NO tiene:** Lógica de negocio

---

## 🎯 Dónde agregar cada cosa

| ¿Qué necesitas? | ¿Dónde va? | ¿Ejemplo? |
|-----------------|-----------|----------|
| Nueva entidad | `Domain/Entities/` | `Vehicle.cs` |
| Nuevo método de BD | `Data/Repositories/` | `IVehicleRepository.GetByBrand()` |
| Nueva acción de usuario | `UseCases/` | `ReserveService.cs` |
| Acceso a BD real | `Framework/Persistence/` | Implementar en `VehicleRepository` |
| Conversión de datos | `Framework/Mappers/` | `VehicleMapper` |
| DTO para transferencia | `Framework/DTOs/` | `VehicleDto` |
| Nuevo endpoint HTTP | `App/Controllers/` | `VehiclesController` |

---

## ⚡ Atajos VS útiles

| Acción | Atajo |
|--------|-------|
| Compilar | `Ctrl + Shift + B` |
| Run (F5) | `F5` |
| Run (Ctrl+F5) | `Ctrl + F5` |
| Ir a definición | `F12` |
| Encontrar referencias | `Shift + F12` |
| Formatear código | `Ctrl + K, D` |
| Solución Explorer | `Ctrl + Alt + L` |
| Buscar archivo | `Ctrl + P` |
| Buscar en archivos | `Ctrl + Shift + F` |

---

## 📈 Crecimiento esperado

```
Hoy (MVP):
├── Domain → User
├── Data → IUserRepository
├── UseCases → Get, Create, Delete
└── App → /api/users

Mañana (v1):
├── Domain → User, Vehicle, Service
├── Data → IUserRepository, IVehicleRepository
├── UseCases → 10+ casos de uso
└── App → /api/users, /api/vehicles, /api/services

Después (v2):
├── Domain → Auth, Roles, Permissions
├── UseCases → Autenticación, Autorización
├── Framework → JWT, OAuth2
└── App → Seguridad a nivel enterprise
```

---

## ✨ Resumen visual

```
┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃  CLIENTE HTTP                     ┃
┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫
┃  GET /api/users/1                ┃
┗━━━━━━━━━┬━━━━━━━━━━━━━━━━━━━━━━━━┛
          │
┌─────────▼────────────────────────────┐
│ 🔴 APP (Controllers)                │
│ ┌──────────────────────────────────┐ │
│ │ UsersController                  │ │
│ │ ├─ GetUserById(1)                │ │
│ │ ├─ CreateUser(dto)               │ │
│ │ ├─ GetAllUsers()                 │ │
│ │ └─ DeleteUser(1)                 │ │
│ └──────────────────────────────────┘ │
└─────────┬────────────────────────────┘
          │
┌─────────▼────────────────────────────┐
│ 🟡 USECASES (Lógica de negocio)     │
│ ┌──────────────────────────────────┐ │
│ │ GetUserById._userRepository...   │ │
│ │ CreateUser.(validar email...)    │ │
│ │ DeleteUser.(verificar existe...) │ │
│ └──────────────────────────────────┘ │
└─────────┬────────────────────────────┘
          │
┌─────────▼────────────────────────────┐
│ 🟣 FRAMEWORK (Implementación)        │
│ ┌──────────────────────────────────┐ │
│ │ UserRepository                   │ │
│ │ ├─ (acceso a datos real)         │ │
│ │ └─ Retorna: User                 │ │
│ │                                  │ │
│ │ UserMapper                       │ │
│ │ └─ Convierte: User → UserDto    │ │
│ └──────────────────────────────────┘ │
└─────────┬────────────────────────────┘
          │
┌─────────▼────────────────────────────┐
│ 🟢 DOMAIN (Entidades)                │
│ ┌──────────────────────────────────┐ │
│ │ User                             │ │
│ │ ├─ Id: int                       │ │
│ │ ├─ Email: string                 │ │
│ │ ├─ Name: string                  │ │
│ │ └─ ...                           │ │
│ └──────────────────────────────────┘ │
└────────────────────────────────────────┘
          ↓
┌─────────────────────────────────────────┐
│ HTTP 200 OK                             │
│ { "id": 1, "email": "...", "name": ... } │
└─────────────────────────────────────────┘
```

---

## 🎓 Concepto clave: Dependency Inversion

```
❌ MAL (Acoplamiento alto):
UseCases → SQL Server directo
UseCases → Firebase directo
UseCases → cambio frecuente de tecnología

✅ BIEN (Acoplamiento bajo):
UseCases → [IUserRepository] ← Framework
                                 └─ puede ser SQL
                                 └─ puede ser Firebase
                                 └─ puede ser MongoDB
                                 └─ puede ser en memoria
```

---

**Esta arquitectura te permite cambiar tecnologías sin tocar la lógica de negocio. ¡Ese es el poder!** 🚀
