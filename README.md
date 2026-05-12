# banana-auth-api

Authentication microservice — ASP.NET Core Web API.

## Environment Variables

| Variable | Description |
|---|---|
| `DATABASE_URL` | PostgreSQL connection string |
| `JWT_SECRET` | Secret key for signing JWT tokens |

## Run locally

```bash
cd src
dotnet run
```

## Run with Docker

```bash
docker compose up --build
```
