Taller_Mecanico_Users
=====================

Servicio de usuarios extraído de Taller_Mecanico_Arqui.

Configuración rápida para conexión a PostgreSQL en Docker:

1. Agregar cadena de conexión en variables de entorno o `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres;Port=5432;Database=taller;Username=postgres;Password=postgres"
  }
}
```

2. Si usas Docker Compose, asegúrate de que el servicio de la base de datos tenga el nombre `postgres` o ajusta `Host`.

3. Construir y ejecutar (desde la carpeta del servicio):

```bash
dotnet build
dotnet run --project App/App.csproj
```
