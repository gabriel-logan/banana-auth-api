# banana-auth-api

Authentication microservice for Banana Meeting Rooms.

## Responsibility

- Register users
- Authenticate users
- Issue JWT access tokens
- Rotate refresh tokens

This service does not call the reservations service directly. The frontend uses the JWT issued here to access `banana-reservations-api`.

## Tech Choices

- **ASP.NET Core Web API** for a direct and explicit REST API
- **Entity Framework Core** for relational persistence and migrations
- **PostgreSQL** as the service database
- **BCrypt** for password hashing
- **JWT + Refresh Token** for auth and session renewal

## Requirements

- Docker
- Docker Compose

## Environment Variables

Copy `.env.example` to `.env` and adjust only if needed.

| Variable | Description |
|---|---|
| `DATABASE_URL` | PostgreSQL connection string used by the API container |
| `JWT_SECRET` | Secret used to sign JWT tokens. It must be exactly the same value used by `banana-reservations-api` |
| `ALLOWED_DOMAINS` | Allowed frontend origins separated by comma, or `*` in development |

Generate a strong shared secret with:

```bash
openssl rand -base64 32
```

## Run

```bash
docker compose up --build
```

The API will be available at `http://localhost:5000`.

## Database, Migrations and Seeds

- Migrations are applied automatically on startup
- Initial auth users are seeded automatically on an empty database

Seeded users:

- `ana.silva@banana.local` / `Banana#2026`
- `bruno.costa@banana.local` / `Banana#2026`

If you want to recreate the database, migrations and seeds from scratch:

```bash
docker compose down -v
docker compose up --build
```

## Routes

- `GET /health`
- `POST /auth/register`
  Body: `{ email, password }`
- `POST /auth/login`
  Body: `{ email, password }`
- `POST /auth/refresh`
  Body: `{ refreshToken }`
