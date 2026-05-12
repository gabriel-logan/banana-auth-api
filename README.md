# banana-auth-api

Authentication microservice — ASP.NET Core Web API.

## Environment Variables

| Variable | Description |
|---|---|
| `DATABASE_URL` | PostgreSQL connection string |
| `JWT_SECRET` | Secret key for signing JWT tokens |

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
- Auth database created manually
- Environment variables configured manually

```bash
cd src
dotnet run
```
