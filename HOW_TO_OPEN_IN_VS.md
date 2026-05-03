# 📸 Guía Visual - Cómo abrir en Visual Studio

Esta guía describe paso a paso cómo abrir la solución en Visual Studio Community 2026.

---

## 🎯 Objetivo
Abrir `Taller_Mecanico_Users.sln` en Visual Studio y ejecutar la aplicación.

---

## 📍 Paso 1: Localizar la solución

La solución está en:
```
E:\7 semestre\Arquitectura de Software\pActual\Taller_Mecanico_Users\Taller_Mecanico_Users.sln
```

**Notas:**
- Busca un archivo con icono azul y nombre `Taller_Mecanico_Users.sln`
- NO abras las carpetas individuales
- Abre siempre el archivo `.sln` (Solution)

---

## 💻 Paso 2: Abrir Visual Studio

1. **Inicia Visual Studio Community 2026**
   - Si ya está abierto, salta al Paso 3
   - Si no está, busca el icono en el Escritorio o Inicio

2. **Espera a que cargue completamente**
   - Verás una pantalla de bienvenida con opciones

---

## 📂 Paso 3: Abrir la solución

**Opción A: Desde la pantalla de bienvenida**
1. Busca un botón que diga **"Open a project or solution"**
2. Haz clic en él
3. Se abrirá un navegador de archivos

**Opción B: Desde VS (si ya está abierto)**
1. Haz clic en **File** (menú superior)
2. Selecciona **Open** → **Project/Solution**
3. Se abrirá un navegador de archivos

```
Visual Studio
├── File                    ← Click aquí
│   ├── New Project/Solution
│   ├── Open
│   │   ├── Project/Solution    ← O aquí
│   │   └── Folder
│   └── ...
```

---

## 📁 Paso 4: Navegar a la carpeta

En el navegador de archivos:

1. **Busca la ruta:**
   ```
   E:\7 semestre\Arquitectura de Software\pActual\Taller_Mecanico_Users\
   ```

2. **Opción A: Copiar/pegar en la barra de direcciones**
   - Haz clic en la barra de direcciones (arriba)
   - Borra el texto actual
   - Pega: `E:\7 semestre\Arquitectura de Software\pActual\Taller_Mecanico_Users\`
   - Presiona **Enter**

3. **Opción B: Navegar manualmente**
   - Haz clic en "Equipo" o "This PC"
   - Navega: `E:` → `7 semestre` → `Arquitectura de Software` → `pActual` → `Taller_Mecanico_Users`

---

## 📄 Paso 5: Seleccionar el archivo .sln

En la carpeta verás varios elementos:

```
Taller_Mecanico_Users/
├── 🔵 Taller_Mecanico_Users.sln        ← ESTE (azul)
├── 📁 App
├── 📁 Domain
├── 📁 Data
├── 📁 UseCases
├── 📁 Framework
└── 📄 *.md (archivos de documentación)
```

**IMPORTANTE:** Selecciona **`Taller_Mecanico_Users.sln`** (el archivo azul)
- NO selecciones las carpetas
- NO selecciones un archivo `.csproj`

---

## ✅ Paso 6: Abrir

1. Haz clic en el archivo `Taller_Mecanico_Users.sln` para seleccionarlo
2. Haz clic en el botón **"Open"** (abajo derecha)
3. Espera a que VS cargue la solución

**Tiempo esperado:** 10-30 segundos (depende del PC)

---

## 🔄 Paso 7: Esperar carga

Visual Studio mostrará:
- Barra de progreso
- Mensaje: "Loading..."
- Finalmente: "Solution loaded"

**Paciencia:** Es normal que tarde un poco la primera vez.

---

## ✨ Paso 8: Ver la estructura

Una vez cargada, a la izquierda verás el **Solution Explorer:**

```
✓ Taller_Mecanico_Users (solución)
  ├── ├ Domain
  │   ├ Data
  │   ├ UseCases
  │   ├ Framework
  │   └ App
  └── References
```

**Si no lo ves:**
- Presiona: **Ctrl + Alt + L**
- O: **View** → **Solution Explorer**

---

## 🔧 Paso 9: Compilar la solución

1. Presiona: **Ctrl + Shift + B**
   O
   Haz clic en: **Build** → **Build Solution**

2. Espera el mensaje: **"Build succeeded"** (abajo derecha)

**Resultado esperado:**
```
Build succeeded. Time elapsed: 00:00:XX
```

Si ves errores, asegúrate de:
- ✓ Abriste el `.sln` (no una carpeta)
- ✓ Esperaste a que cargara completamente
- ✓ Tienes .NET 10 instalado

---

## ⚙️ Paso 10: Establecer App como startup

El proyecto **App** debe ser el que se ejecute.

1. En el **Solution Explorer** (izquierda), busca la carpeta **"App"**
2. Haz clic derecho en **"App"**
3. Selecciona: **"Set as Startup Project"**

**Confirmación:** El proyecto **"App"** aparecerá en **negrita**

```
  ├ Domain
  ├ Data
  ├ UseCases
  ├ Framework
  └ **App**  ← Debe estar en negrita
```

---

## ▶️ Paso 11: Ejecutar la aplicación

### Opción A: Sin debugger (más rápido)
- Presiona: **Ctrl + F5**

### Opción B: Con debugger (para depuración)
- Presiona: **F5**

**Resultado esperado:**
1. Se abrirá una consola (ventana negra)
2. Verás mensajes de inicio
3. Verás algo como: `Application started. Press Ctrl+C to shut down.`
4. La URL aparecerá: `https://localhost:5001` o similar

---

## 🧪 Paso 12: Probar la API

Con la aplicación corriendo, abre **PowerShell** y ejecuta:

```powershell
# Crear un usuario
$body = @{
    email = "test@example.com"
    name = "Test User"
    phone = "1234567890"
} | ConvertTo-Json

curl -X POST https://localhost:5001/api/users `
  -SkipCertificateCheck `
  -Headers @{"Content-Type" = "application/json"} `
  -Body $body
```

**Resultado esperado:**
```json
{
  "id": 1,
  "email": "test@example.com",
  "name": "Test User",
  "phone": "1234567890",
  "createdAt": "2024-01-15T10:30:00..."
}
```

---

## 🛑 Paso 13: Parar la aplicación

En Visual Studio:
- Presiona: **Shift + F5**
- O en la consola: **Ctrl + C**

---

## 🎉 ¡Listo!

Ya tienes todo corriendo. Ahora puedes:

✅ **Explorar el código:** Abre archivos en el Solution Explorer  
✅ **Entender la arquitectura:** Lee `README.md` en la raíz  
✅ **Agregar funcionalidades:** Sigue `DEVELOPMENT_GUIDE.md`  
✅ **Debuggear:** Establece breakpoints con F9  

---

## 🐛 Solución de problemas

### Error: "Solo una unidad de compilación puede tener instrucciones de nivel superior"
**Causa:** Hay dos `Program.cs`
**Solución:** Eliminamos el problema. Compila de nuevo: `Ctrl+Shift+B`

### Error: "Proyecto no compatible"
**Causa:** Falta .NET 10
**Solución:** 
1. Descarga .NET 10 desde: https://dotnet.microsoft.com/download
2. Instala y reinicia VS

### Error: "El certificado no es válido"
**Causa:** HTTPS
**Solución:** Usa `-SkipCertificateCheck` en curl (ya está en los ejemplos)

### Error: "Puerto ya en uso"
**Causa:** Otra aplicación usa 5001
**Solución:** 
1. Para esa aplicación
2. O en `App/Program.cs`, cambia el puerto

---

## 📚 Archivos de ayuda en la solución

Una vez que abras la solución, verás estos archivos en la raíz:

| Archivo | Contenido |
|---------|----------|
| `README.md` | Explicación completa de la arquitectura |
| `QUICK_START.md` | Atajos de VS y comandos PowerShell |
| `DEVELOPMENT_GUIDE.md` | Cómo agregar nuevas funcionalidades |
| `ARCHITECTURE_VISUAL.md` | Diagramas y explicación visual |
| `SETUP_COMPLETE.md` | Resumen del setup |

Haz clic en cualquiera en VS para abrirlo.

---

## ✨ Siguientes pasos

1. ✅ Abre la solución (estos pasos)
2. ✅ Compila
3. ✅ Ejecuta (F5)
4. ✅ Prueba un endpoint
5. 📖 Lee `README.md` para entender la arquitectura
6. 🚀 Sigue `DEVELOPMENT_GUIDE.md` para agregar funcionalidades

---

**¿Necesitas ayuda?** 
- Revisa los archivos .md en la solución
- Busca el error en "Solución de problemas" arriba
- Consulta la documentación de Visual Studio

**¡Bienvenido a tu nueva arquitectura hexagonal!** 🎉
