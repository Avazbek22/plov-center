# Docker production setup

Two images, no `docker-compose`, external Postgres, HTTP only.

## Backend

Build context = repo root.

```bash
cp .env.example .env        # fill in the empty values
docker build -t plov-center-api .
```

### Required env vars

All declared in `.env.example`. Empty = must be filled before first run:

| Variable                       | Purpose                                                       |
|--------------------------------|---------------------------------------------------------------|
| `ConnectionStrings__Postgres`  | Npgsql connection string to your Postgres                     |
| `Jwt__SigningKey`              | Random ≥32-char secret. `openssl rand -base64 48`             |
| `SeedAdmin__Password`          | Initial admin password (used on first boot only)              |
| `Cors__AllowedOrigins__0`      | Public URL of the frontend (e.g. `http://plov.example.com`)   |

Defaults (override only if needed): `Jwt__Issuer`, `Jwt__Audience`, `Jwt__ExpiresMinutes`, `SeedAdmin__Username`, `SeedAdmin__IsActive`, `FileStorage__RootPath`, `ASPNETCORE_ENVIRONMENT`, `ASPNETCORE_URLS`.

### Run

```bash
# one-time: shared user-defined network so frontend can resolve "plov-api" by name
docker network create plov-net

# one-time: prepare host upload directory with the .NET non-root UID (1654)
sudo mkdir -p /srv/plov-center/uploads
sudo chown -R 1654:1654 /srv/plov-center/uploads

docker run -d \
  --name plov-api \
  --network plov-net \
  --env-file .env \
  -v /srv/plov-center/uploads:/app/wwwroot/uploads \
  -p 8080:8080 \
  --restart unless-stopped \
  plov-center-api
```

The app auto-applies EF migrations and seeds the admin on startup. Check `docker logs -f plov-api`.

## Frontend

Build context = `./front`. No backend URL is baked into the bundle — `apiFetch` already calls relative paths (`/api/...`, `/uploads/...`). The nginx inside the container reverse-proxies those to the backend.

```bash
cd front
docker build -t plov-center-front .

docker run -d \
  --name plov-front \
  --network plov-net \
  -e BACKEND_URL=http://plov-api:8080 \
  -p 80:80 \
  --restart unless-stopped \
  plov-center-front
```

Two env vars control the frontend container:

| Variable      | Default                  | Notes                                                                                   |
|---------------|--------------------------|-----------------------------------------------------------------------------------------|
| `BACKEND_URL` | `http://plov-api:8080`   | Full URL: scheme + host + (optional) port. **No trailing slash.** Domain, IP, or container name — pofig. |
| `NGINX_PORT`  | `80`                     | Port nginx listens on **inside** the container. Must match the container-side of `-p`. |

Examples:

```bash
# Same docker host, container-to-container
-e BACKEND_URL=http://plov-api:8080

# Backend on another machine, plain IP
-e BACKEND_URL=http://192.168.1.10:5000

# Backend behind a domain with HTTPS
-e BACKEND_URL=https://api.example.com

# nginx listens on 8888 inside, exposed on host 8888
-e NGINX_PORT=8888 -p 8888:8888
```

If backend lives outside Docker, drop `--network plov-net`.

## Updating

```bash
# Backend
docker build -t plov-center-api .
docker rm -f plov-api
docker run -d --name plov-api --network plov-net --env-file .env \
  -v /srv/plov-center/uploads:/app/wwwroot/uploads -p 8080:8080 \
  --restart unless-stopped plov-center-api

# Frontend
cd front && docker build -t plov-center-front .
docker rm -f plov-front
docker run -d --name plov-front --network plov-net \
  -e BACKEND_URL=http://plov-api:8080 -p 80:80 \
  --restart unless-stopped plov-center-front
```

Uploads survive container recreation (bind mount). Postgres data lives outside Docker entirely.
