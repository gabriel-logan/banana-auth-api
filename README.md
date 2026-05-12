# banana-auth-api

Authentication microservice — ASP.NET Core Web API.

## Routes

- **GET /health** — Health check
- **POST /api/auth/register** — Register a new user. Body: `{ email, password }`
- **POST /api/auth/login** — Authenticate and receive JWT. Body: `{ email, password }`
- **POST /api/auth/refresh** — Receive a new JWT and a new refresh token. Body: `{ refreshToken }`

## Environment Variables

| Variable | Description |
|---|---|
| `DATABASE_URL` | PostgreSQL connection string |
| `JWT_SECRET` | Secret key for signing JWT tokens |

## Database

The service uses Entity Framework Core with PostgreSQL.

Database migrations are applied automatically on startup.

## Recommended: Run with Docker

```bash
docker compose up --build

# or

docker-compose up --build
```

This starts:

- **auth-api** — ASP.NET Core API on port 5000
- **postgres-auth** — PostgreSQL database

## Manual setup

*Not recommended.*

Requires:

- PostgreSQL installed locally
- Environment variables configured manually

```bash
cd src
dotnet run
```
