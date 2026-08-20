# Apitransac

Backend de autenticación desarrollado con **ASP.NET Core Web API (.NET 10)**, utilizando **SQL Server** como base de datos y **JWT (JSON Web Token)** para la autenticación y autorización.

## 🚀 Tecnologías

* **.NET 10**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server**
* **JWT Bearer Authentication**
* **Refresh Tokens**

## 📋 Funcionalidades

* Registro de usuarios
* Inicio de sesión
* Autenticación mediante JWT
* Access Token con expiración configurable
* Refresh Token
* Persistencia de usuarios mediante SQL Server
* Migraciones con Entity Framework Core

## 📁 Estructura del proyecto

```text
Apitransac/
│
├── Controllers/
│   └── AuthController.cs
│
├── Data/
│   └── ...
│
├── Services/
│   └── ...
│
├── Models/
│   └── ...
│
├── Middleware/
│   └── ...
│
├── DTOs/
│   └── ...
│
├── Migrations/
│   └── ...
│
├── Common/
│   └── ...
│
├── Program.cs
├── appsettings.json
└── Apitransac.csproj
```

### Descripción de las carpetas

| Carpeta       | Descripción                                                            |
| ------------- | ---------------------------------------------------------------------- |
| `Controllers` | Endpoints HTTP de la API                                               |
| `Data`        | Configuración y acceso a la base de datos                              |
| `Services`    | Lógica de negocio y servicios de la aplicación                         |
| `Models`      | Entidades utilizadas por la aplicación                                 |
| `Middleware`  | Middleware personalizado                                               |
| `DTOs`        | Objetos utilizados para recibir y devolver información mediante la API |
| `Migrations`  | Migraciones generadas por Entity Framework Core                        |
| `Common`      | Clases y componentes comunes                                           |

## 🔐 Autenticación

La API utiliza **JWT Bearer Authentication**.

Para acceder a endpoints protegidos se debe enviar el token mediante el header HTTP:

```http
Authorization: Bearer <access_token>
```

El **Access Token** tiene una duración configurada actualmente de **10 minutos**.

Una vez expirado, se utiliza el **Refresh Token** para obtener un nuevo Access Token, dependiendo de la implementación del servicio.

## 🌐 Endpoints

### Login

Permite autenticar un usuario y obtener los tokens de autenticación.

```http
POST /api/Auth/login
```

### Registro

Permite registrar un nuevo usuario.

```http
POST /api/Auth/register
```

### Refresh Token

El proyecto utiliza Refresh Tokens para renovar la autenticación cuando el Access Token expira.

> El endpoint específico de refresh token debe agregarse aquí cuando esté definido en el controlador.

## ⚙️ Configuración

La aplicación utiliza una cadena de conexión a SQL Server y parámetros para la configuración de JWT.

Ejemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=dbtest;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR_JWT_SECRET_KEY",
    "Issuer": "apitransaction",
    "Audience": "MiApiClient",
    "AccessTokenExpirationMinutes": 10
  }
}
```

### Variables de configuración

| Variable                              | Descripción                                 |
| ------------------------------------- | ------------------------------------------- |
| `ConnectionStrings:DefaultConnection` | Cadena de conexión a SQL Server             |
| `Jwt:Key`                             | Clave secreta utilizada para firmar los JWT |
| `Jwt:Issuer`                          | Emisor del token                            |
| `Jwt:Audience`                        | Audiencia del token                         |
| `Jwt:AccessTokenExpirationMinutes`    | Tiempo de expiración del Access Token       |

> **Importante:** No se recomienda subir secretos reales, claves JWT o credenciales de base de datos al repositorio Git. Para desarrollo local se recomienda utilizar User Secrets o variables de entorno.

## 🗄️ Base de datos

El proyecto utiliza **SQL Server** mediante **Entity Framework Core**.

La conexión está configurada para utilizar una instancia local de SQL Server Express:

```text
Server=localhost\SQLEXPRESS
Database=dbtest
```

## 🔄 Migraciones

Para crear una nueva migración:

```bash
dotnet ef migrations add InitialMigration
```

Para aplicar las migraciones a la base de datos:

```bash
dotnet ef database update
```

Si `dotnet ef` no está instalado, puede instalarse mediante:

```bash
dotnet tool install --global dotnet-ef
```

## ▶️ Ejecución del proyecto

### 1. Clonar el repositorio

```bash
git clone https://github.com/kennyVargas/Login_Bakend_Net
cd Apitransac
```

### 2. Configurar la base de datos

Verificar que SQL Server / SQL Server Express esté ejecutándose y configurar correctamente la cadena de conexión.

### 3. Ejecutar las migraciones

```bash
dotnet ef migrations add InitialMigration
dotnet ef database update
```

### 4. Ejecutar la aplicación

```bash
dotnet run
```

La API estará disponible en:

```text
http://localhost:8080
```

## 🧪 Pruebas de la API

Actualmente la API cuenta con los siguientes endpoints principales:

```text
POST /api/Auth/register
POST /api/Auth/login
```

Las solicitudes pueden probarse utilizando herramientas como:

* Postman
* Insomnia
* REST Client
* Swagger, si posteriormente se incorpora al proyecto

## 🔑 Flujo de autenticación

El flujo básico de autenticación es:

```text
┌──────────────┐
│    Cliente   │
└──────┬───────┘
       │
       │ POST /api/Auth/login
       ▼
┌──────────────────┐
│   AuthController │
└────────┬─────────┘
         │
         │ Validar credenciales
         ▼
┌──────────────────┐
│   Base de Datos  │
└────────┬─────────┘
         │
         │ Usuario válido
         ▼
┌──────────────────┐
│    JWT Service   │
└────────┬─────────┘
         │
         │ Access Token + Refresh Token
         ▼
┌──────────────┐
│    Cliente   │
└──────────────┘
```

Para acceder posteriormente a un recurso protegido:

```http
Authorization: Bearer <access_token>
```

Cuando el Access Token expira, el Refresh Token permite solicitar un nuevo token según la lógica implementada por el servicio.

## 🛡️ Seguridad

Para ambientes de producción se recomienda:

* No almacenar la clave JWT directamente en `appsettings.json`.
* Utilizar variables de entorno, User Secrets o un gestor de secretos.
* Utilizar HTTPS.
* Utilizar una clave JWT suficientemente larga y aleatoria.
* No subir credenciales al repositorio.
* Configurar correctamente la expiración y renovación de Refresh Tokens.
* Implementar revocación de Refresh Tokens cuando sea necesario.
* No utilizar credenciales de desarrollo en producción.

## 📌 Estado del proyecto

Actualmente el proyecto proporciona una API de autenticación con:

* ✅ Registro de usuarios
* ✅ Login
* ✅ JWT Authentication
* ✅ Refresh Token
* ✅ SQL Server
* ✅ Entity Framework Core
* ✅ Migraciones

`Production`

## 👨‍💻 Desarrollo
