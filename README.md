# CP Movies Portal

API backend en **.NET (SDK 10.0.100)** para un portal de alquiler/venta de películas, organizado como un **monolito modular**: un único proceso desplegable (`CP.Portal.Api`) compuesto por módulos de negocio independientes (`Movies.Module`, `Users.Module`) que se comunican entre sí mediante un mediador interno en vez de llamarse directamente.

## Arquitectura general

```
┌─────────────────────────────────────────────────────────────┐
│                     CP.Portal.Api (host)                     │
│   Program.cs: composition root — registra todos los módulos  │
│   FastEndpoints · JWT Auth · Serilog · ExceptionMiddleware    │
└───────────────┬───────────────────────────┬──────────────────┘
                │                            │
                ▼                            ▼
┌───────────────────────────┐   ┌───────────────────────────┐
│      Movies.Module         │   │       Users.Module         │
│  Endpoints, Services,      │   │  Endpoints, UseCases       │
│  Data (EF Core + Postgres) │   │  Data (EF Core + Identity) │
│  Integrations (handlers)   │   │  (Cart, Auth)               │
└──────────────┬──────────────┘   └──────────────┬──────────────┘
               │                                 │
               └────────────┬────────────────────┘
                             ▼
                 CP.Contracts (Core.Contracts)
        Contratos compartidos entre módulos (queries/eventos
        de MediatOR) + infraestructura de validación de endpoints

                             ▲
                             │ usan
                 libs/ (Core.MediatOR, Core.Results, Core.Mappy)
              Mediador in-process, tipo Result y mapeo, sin
              dependencias de negocio — reutilizables en cualquier módulo
```

Cada módulo posee su propio `DbContext`, sus propias migraciones y su propio esquema de dominio, pero **ambos comparten la misma base de datos PostgreSQL** (`monolithic_db`), lo que refleja el estilo "monolito modular": separación lógica de código, sin separación física de infraestructura.

## Estructura de carpetas

```
app/backend/
├── CP.Portal.sln
├── docker-compose.yml              # Postgres 16 (monolithic-db)
├── Api/src/CP.Portal.Api/          # Host ASP.NET Core — punto de entrada
│   ├── Program.cs                  # Composition root
│   └── Middleware/ExceptionMiddleware.cs
├── CP.Contracts/src/Core.Contracts/
│   ├── Core/                       # ValidatedEndpoint, IValidator, ValidationEndpointFilter
│   └── MovieDetails/                # Contratos MediatOR compartidos entre módulos
├── Movies.Module/
│   ├── src/CP.Portal.Movies.Module/
│   │   ├── Data/                   # Movie, MovieDbContext, Migrations, Repositories, Seedings
│   │   ├── Endpoints/               # Endpoints FastEndpoints (CRUD de películas)
│   │   ├── Services/                # IMovieService / MovieService (lógica de negocio)
│   │   ├── Integrations/            # Handlers MediatOR que exponen datos a otros módulos
│   │   └── MovieServiceExtensions.cs # DI: registro del módulo
│   └── tests/CP.Portal.Movies.Module.Tests/
└── Users.Module/src/CP.Portal.Users.Module/
    ├── Data/                        # ApplicationUser, CartMovie, UserDbContext (Identity)
    ├── Endpoints/
    │   ├── UserEndpoints/            # Registro y login (JWT)
    │   └── CartEndpoints/            # Carrito de compras
    ├── UseCases/                     # Comandos/queries MediatOR (p.ej. AddMovieToCart)
    └── UsersModuleExtensions.cs      # DI: registro del módulo

libs/
├── Core.MediatOR/                    # Mediador in-process propio (IRequest, IRequestHandler, pipeline behaviors)
├── Core.Results/                     # Tipo Result/PagedResult para manejo de errores sin excepciones
└── Core.Mappy/                       # Utilidades de mapeo entre entidades y DTOs
```

## Componentes clave

### Host (`CP.Portal.Api`)
Es el **composition root**: en `Program.cs` se registran los servicios de cada módulo (`AddMovieServices`, `AddUserModuleServices`), se configura autenticación JWT, FastEndpoints, Serilog y se aplican las migraciones de ambos `DbContext` automáticamente al arrancar. No contiene lógica de negocio propia.

### Módulos de negocio
- **Movies.Module**: catálogo de películas. Expone endpoints REST (`/api/movies`) implementados con **FastEndpoints**, con lógica en `IMovieService`/`MovieService` y persistencia vía EF Core (`MovieDbContext`) sobre PostgreSQL. Incluye seeding asíncrono de datos de ejemplo (películas, géneros, cast, crew).
- **Users.Module**: usuarios, autenticación (ASP.NET Core Identity + JWT) y carrito de compras. Expone `/users/login`, `/users` (registro) y endpoints de carrito.

### Comunicación entre módulos: MediatOR
Los módulos **no se referencian directamente entre sí**. Cuando `Users.Module` necesita datos de `Movies.Module` (por ejemplo, al añadir una película al carrito), envía una query definida en `CP.Contracts` (`MovieDetailsQuery`) a través de `IMediatOR`, que `Movies.Module` resuelve con un `IRequestHandler` (`MovieDetailsQueryHandler`). Esto mantiene el acoplamiento bajo control y hace explícitos los contratos entre módulos.

`libs/Core.MediatOR` es una implementación propia y minimalista del patrón mediator (equivalente reducido a MediatR), con soporte para pipeline behaviors.

### Validación de endpoints
`CP.Contracts` define `ValidatedEndpoint<TRequest>`, una clase base sobre `FastEndpoints.Endpoint<T>` que ejecuta automáticamente los `IValidator<TRequest>` registrados en DI antes de invocar la lógica del endpoint (`OnValidatedAsync`), devolviendo un `400` estructurado si hay errores.

### Resultados
`libs/Core.Results` provee un tipo `Result`/`Result<T>` (y `PagedResult`) para modelar éxito/fallo (`NotFound`, `Unauthorized`, `Success`, etc.) sin usar excepciones para control de flujo entre capas.

## Stack tecnológico

- **.NET 10** (SDK `10.0.100`, ver `global.json`)
- **FastEndpoints** — framework de endpoints minimalista (alternativa a Controllers/MVC)
- **Entity Framework Core** con **Npgsql** (PostgreSQL) y convención snake_case
- **ASP.NET Core Identity** + **JWT Bearer** para autenticación
- **Serilog** para logging estructurado
- **PostgreSQL 16** vía Docker Compose (`monolithic-db`)
- Mediador, resultados y mapeo propios (`libs/Core.*`) en vez de MediatR/AutoMapper de terceros

## Cómo ejecutar

```bash
# 1. Levantar la base de datos
cd app/backend
docker compose up -d

# 2. Ejecutar la API (aplica migraciones y hace seeding automáticamente)
dotnet run --project Api/src/CP.Portal.Api
```

La API expone Swagger/OpenAPI (`AddOpenApi`) y usa `CP.Portal.Api.http` como colección de requests de ejemplo.

## Tests

```bash
dotnet test app/backend/CP.Portal.sln
```

Actualmente `Movies.Module` cuenta con tests de integración sobre sus endpoints (`Movies.Module/tests/CP.Portal.Movies.Module.Tests`).
