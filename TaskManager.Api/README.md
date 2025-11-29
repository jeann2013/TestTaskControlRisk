# 🛠️ TaskManager Backend — .NET 9 + SQL Server + CosmosDB + JWT + IA

Backend del proyecto **TaskManager**, construido con **.NET 9 Web API**, autenticación con **JWT**, persistencia con **SQL Server y/o CosmosDB**, y funciones de **Inteligencia Artificial (OpenAI)** para análisis automático de tareas y generación de subtareas.

---

## 🚀 ¿Qué hace este Backend?

* Autenticación de usuarios con **JWT**
* CRUD de tareas (crear, listar, actualizar, eliminar)
* Marcar tareas como completadas
* Integración con **OpenAI GPT-4o-mini** para:

  * **Analizar tareas → resumen + prioridad**
  * **Generar subtareas automáticamente**
* Documentación interactiva con **Swagger**
* Base de datos usando:

  * **SQL Server** (por defecto)
  * **CosmosDB** (opcional)

---

# ⚙️ Tecnologías utilizadas

| Tecnología                  | Uso                          |
| --------------------------- | ---------------------------- |
| **.NET 9 Web API**          | Backend principal            |
| **Entity Framework Core 9** | ORM                          |
| **SQL Server**              | Base de datos                |
| **CosmosDB**                | Persistencia opcional        |
| **JWT Bearer**              | Autenticación                |
| **Swagger / Swashbuckle**   | Documentación                |
| **OpenAI SDK 2.7**          | IA para análisis y subtareas |

---

# 📦 Instalación

## 1️⃣ Requisitos previos

Debes tener instalado:

* **.NET SDK 9**
* **SQL Server Express o Docker**
* **Un editor (VS Code / Rider / Visual Studio)**
* **Una API Key de OpenAI**

---

## 2️⃣ Clonar el repositorio

```bash
git clone https://github.com/jeann2013/TestTaskControlRisk.git
cd TestTaskControlRisk
```

---

## 3️⃣ Configurar el archivo `appsettings.json`

Editar:

```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TaskManager;Trusted_Connection=True;TrustServerCertificate=True"
},
"OpenAI": {
  "ApiKey": "TU_API_KEY_AQUI"
},
"Jwt": {
  "Key": "CLAVE_SUPER_SECRETA"
}
```

---

## 4️⃣ Restaurar dependencias

```bash
dotnet restore
```

---

## 5️⃣ Aplicar migraciones (si usas SQL Server)

```bash
dotnet ef database update
```

---

## 6️⃣ Ejecutar el servidor

```bash
dotnet run
```

La API iniciará en:

```
https://localhost:7179
http://localhost:5100
```

---

# 📘 Uso de Swagger

Este backend incluye **Swagger UI** para explorar y probar los endpoints sin necesidad de Postman.

### 👉 Abrir Swagger

```
https://localhost:7179/swagger
```

### Qué puedes hacer en Swagger:

* Registrar usuario (`POST /auth/register`)
* Iniciar sesión (`POST /auth/login`)
* Obtener token JWT
* Enviar token para:

  * Listar tareas
  * Crear tarea
  * Completar tarea
  * Analizar tarea con IA (`POST /tasks/analyze`)
  * Generar subtareas (`POST /tasks/suggest`)

Swagger autocompleta los modelos y permite probar cada endpoint fácilmente.

---

# 🔌 Estructura del backend

```
TaskManager.Api/
│
├── Controllers/
│   ├── AuthController.cs
│   └── TasksController.cs
│
├── Entities/
├── Data/
├── Services/
│   ├── AiService.cs
│   └── JwtService.cs
│
├── Program.cs
├── appsettings.json
└── TaskManager.Api.csproj
```

---

# 🧠 IA en el Backend

Este backend tiene 2 endpoints IA:

### 🔍 Analizar tarea

`POST /tasks/analyze`

Devuelve:

```json
{
  "summary": "...",
  "priority": "alta | media | baja"
}
```

### 🧩 Generar subtareas

`POST /tasks/suggest`

Devuelve:

```json
{
  "subtasks": ["...", "...", "...", "..."]
}
```

---

# 🧪 Scripts útiles

| Comando                           | Descripción           |
| --------------------------------- | --------------------- |
| `dotnet run`                      | Inicia la API         |
| `dotnet restore`                  | Restaura dependencias |
| `dotnet build`                    | Compila el proyecto   |
| `dotnet ef migrations add Nombre` | Crear migración       |
| `dotnet ef database update`       | Aplicar migraciones   |

---

# 🎯 Notas finales

* Swagger siempre está habilitado en modo **Development**.
* La API requiere un token **Bearer** para todos los endpoints excepto `/auth/*`.
* Si cambias el modelo IA, actualiza `AiService.cs`.
