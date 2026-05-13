# banana-auth-api

Authentication microservice for Banana Meeting Rooms.

## Responsibility

- Register users
- Authenticate users
- Issue JWT access tokens
- Rotate refresh tokens

This service does not call the reservations service directly. The frontend uses the JWT issued here to access `banana-reservations-api`.

## Tech Choices

- **ASP.NET Core Web API** because the challenge explicitly asks for a C# API and ASP.NET Core is the most standard and production-proven choice in this ecosystem. I chose it over building a more minimal custom host because it gives native routing, model validation, dependency injection and middleware with very little boilerplate, which keeps the auth service straightforward and maintainable.
- **Entity Framework Core** because the project needs relational persistence with a mature ORM that integrates naturally with ASP.NET Core. I chose EF Core over writing raw SQL or using a micro-ORM like Dapper because this service is small, CRUD-oriented and benefits more from fast development, migrations and clear entity mapping than from lower-level query control.
- **PostgreSQL** because it is a robust relational database, fits the challenge requirement well and works consistently across both microservices. I chose it over SQLite because the project models concurrent business data and relational integrity more realistically with a server database, and over heavier enterprise options because PostgreSQL is simpler to run locally in Docker.
- **BCrypt** because password storage should be irreversible and intentionally expensive to brute-force. I chose it over storing plain hashes with algorithms like SHA256 because those are not appropriate for passwords. BCrypt is also simple to integrate here without introducing unnecessary auth complexity.
- **JWT + Refresh Token** because the architecture requires the auth service to issue a token that can be validated independently by the reservations service. I chose this over server-side sessions because JWT fits microservice separation better, and I added refresh token rotation to avoid forcing frequent logins while still keeping short-lived access tokens.

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

You can keep the shared example value from `.env.example` for quick local testing.

Optional (recommended): generate a stronger shared secret with:

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

## How This API Works

The authentication flow starts when the frontend calls `POST /auth/register` or `POST /auth/login`. The API validates the request body using ASP.NET Core model validation, normalizes the email, and then either creates the user or checks the submitted password against the stored BCrypt hash.

After a successful authentication, the service generates a JWT access token signed with `JWT_SECRET`. This token carries the authenticated user identity and is meant to be sent by the frontend to the reservations API. The service also generates a refresh token, stores only its hash in the database and returns the raw token to the client.

When the frontend calls `POST /auth/refresh`, the API hashes the received refresh token, searches for the matching stored hash, validates expiration and rotates the refresh token before returning a new access token. In this architecture, this service is the only place responsible for credentials, password verification and token issuance.

## Routes

- `GET /health`
- `POST /auth/register`
  Body: `{ email, password }`
- `POST /auth/login`
  Body: `{ email, password }`
- `POST /auth/refresh`
  Body: `{ refreshToken }`
