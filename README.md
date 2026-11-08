# ASP.NET Core Authentication API

A REST API built with **ASP.NET Core 9, C#, PostgreSQL, Entity Framework Core, ASP.NET Core Identity, and JWT**.

The project implements user registration and login. It uses ASP.NET Core Identity to manage user records and password hashing, PostgreSQL for persistence, and JSON Web Tokens (JWT) to issue an authentication token after a successful login.

## Features implemented

- User registration with first name, last name, email, and password.
- Duplicate-email check during registration.
- User creation through ASP.NET Core Identity.
- Password verification during login.
- JWT generation after successful login.
- PostgreSQL integration through Entity Framework Core and Npgsql.
- Dependency injection for the authentication service.
- Swagger/OpenAPI for exploring the API.
- Dockerfile for containerised builds and execution.
- EF Core migrations for database schema management.

## Technology stack

| Technology | Role |
| --- | --- |
| C# / .NET 9 | Application language and runtime |
| ASP.NET Core Web API | HTTP API framework |
| ASP.NET Core Identity | User management and password hashing |
| Entity Framework Core | Object-relational mapping and migrations |
| PostgreSQL | Relational database |
| Npgsql | PostgreSQL provider for EF Core |
| JWT Bearer | Token generation and bearer-token validation configuration |
| Swagger / Swashbuckle | API documentation |
| Docker | Container build and runtime |

## Project structure

The current repository is organised as follows:

```text
authApi/
├── authapi/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   └── WeatherForecastController.cs
│   ├── DTOs/
│   │   ├── UserLoginDto.cs
│   │   └── UserSignupDto.cs
│   ├── Datas/
│   │   └── ApplicationDbContext.cs
│   ├── IServices/
│   │   └── IAuthService.cs
│   ├── Migrations/
│   ├── Models/
│   │   └── ApplicationUser.cs
│   ├── Servicess/
│   │   └── AuthService.cs
│   ├── Program.cs
│   ├── appsettings.json
│   ├── authapi.csproj
│   └── Dockerfile
└── authapi.sln
```

### Architecture

The API uses a controller-service separation:

- **Controllers** receive HTTP requests and return HTTP responses.
- **IAuthService** defines the authentication operations exposed to the controller.
- **AuthService** contains registration, login, and JWT creation logic.
- **DTOs** define the data accepted by registration and login endpoints.
- **ApplicationUser** extends ASP.NET Core Identity's `IdentityUser` with first-name and last-name properties.
- **ApplicationDbContext** extends `IdentityDbContext<ApplicationUser>` and provides the EF Core database context.
- **Migrations** track database schema changes.

The authentication service is registered with dependency injection in `Program.cs`.

## API endpoints

The authentication controller uses the route prefix `/api/Auth`.

| Method | Endpoint | Purpose | Successful response |
| --- | --- | --- | --- |
| POST | `/api/Auth/register` | Creates a user account | HTTP 200 with a success message |
| POST | `/api/Auth/login` | Verifies credentials and creates a JWT | HTTP 200 with the token |

Both endpoints return HTTP 400 when the service reports a failed operation or model validation fails.

### Registration request

```json
{
  "firstName": "Ada",
  "lastName": "Okafor",
  "email": "ada@example.com",
  "password": "Use-A-Strong-Password",
  "confirmPassword": "Use-A-Strong-Password"
}
```

### Login request

```json
{
  "email": "ada@example.com",
  "password": "Use-A-Strong-Password"
}
```

### Authentication flow

1. The client submits registration details to `POST /api/Auth/register`.
2. The authentication service checks whether the email is already registered.
3. ASP.NET Core Identity creates the user and securely hashes the password.
4. The client submits credentials to `POST /api/Auth/login`.
5. The service retrieves the user and verifies the password with ASP.NET Core Identity.
6. On successful login, the service creates and returns a signed JWT.

## Run locally

### Prerequisites

- .NET 9 SDK
- PostgreSQL database
- Git
- .NET EF Core CLI tool (for applying migrations)

### 1. Clone the repository

```bash
git clone https://github.com/Toluwani2024/authApi.git
cd authApi
```

### 2. Configure local secrets

The application reads its database connection string and JWT settings from .NET configuration.

For local development, use .NET User Secrets rather than committing credentials:

```bash
dotnet user-secrets init --project authapi/authapi.csproj
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=authapi;Username=postgres;Password=YOUR_LOCAL_PASSWORD" --project authapi/authapi.csproj
dotnet user-secrets set "Jwt:Secret" "REPLACE_WITH_A_LONG_RANDOM_SECRET" --project authapi/authapi.csproj
dotnet user-secrets set "Jwt:Issuer" "authapi" --project authapi/authapi.csproj
dotnet user-secrets set "Jwt:Audience" "authapi-client" --project authapi/authapi.csproj
```

Use a long, randomly generated signing key appropriate for the configured HMAC algorithm. Never commit real connection strings, passwords, tokens, or signing keys.

For deployment environments, provide equivalent configuration through environment variables:

- `ConnectionStrings__DefaultConnection`
- `Jwt__Secret`
- `Jwt__Issuer`
- `Jwt__Audience`

### 3. Restore dependencies

```bash
dotnet restore authapi.sln
```

### 4. Apply database migrations

If the EF Core CLI is not installed:

```bash
dotnet tool install --global dotnet-ef
```

Apply the migrations:

```bash
dotnet ef database update --project authapi/authapi.csproj
```

### 5. Run the API

```bash
dotnet run --project authapi/authapi.csproj
```

Use the local URL printed by the application and open `/swagger` to explore the available endpoints.

## Docker

A multi-stage Dockerfile is included in `authapi/Dockerfile`. From the repository root, build the image with:

```bash
docker build -t authapi -f authapi/Dockerfile .
```

Provide the database and JWT configuration to the container through environment variables. Do not bake secrets into the image.

## Engineering notes

The repository is an implementation and learning project. Before using it as a production authentication service, the following items need to be completed and verified:

- Add `app.UseAuthentication()` before `app.UseAuthorization()` in the HTTP pipeline.
- Align the JWT issuer configuration key used when creating tokens with the key used during token validation (`Jwt:Issuer`).
- Add and test protected endpoints using `[Authorize]`.
- Strengthen DTO validation, including email format, required password fields, and confirmation-password matching.
- Return appropriate HTTP status codes and useful validation details without exposing sensitive information.
- Align ASP.NET Core Identity and JWT package versions with the project's .NET 9 target, then run build and integration tests.
- Keep secrets out of tracked configuration and Git history; rotate any credentials that have previously been committed.

These are explicit follow-up engineering tasks, not features claimed as already complete.

## Author

**Toluwani Aderibigbe**  
Web Developer | Founder of Lumeon.dev

- GitHub: [@Toluwani2024](https://github.com/Toluwani2024)
- Portfolio: [dev-portfolio-two-flame.vercel.app](https://dev-portfolio-two-flame.vercel.app)
