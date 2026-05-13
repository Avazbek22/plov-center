# syntax=docker/dockerfile:1.7
# ============================================================
# PlovCenter frontend — production image
# Build context: ./front
#   cd front && docker build -t plov-center-front .
# ============================================================

# ---------- build stage ----------
FROM node:22-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build

# ---------- runtime stage ----------
FROM nginx:1.27-alpine AS runtime

# Any *.template file dropped into /etc/nginx/templates/ is run through
# envsubst by the official nginx entrypoint at container startup and
# written to /etc/nginx/conf.d/<file> before nginx starts.
COPY nginx.conf.template /etc/nginx/templates/default.conf.template

COPY --from=build /app/dist /usr/share/nginx/html

# Default backend target — override at `docker run` with `-e BACKEND_URL=...`.
# Full URL with scheme, host and (optionally) port. NO trailing slash.
#   examples: http://plov-api:8080   http://192.168.1.10:5000   https://api.example.com
ENV BACKEND_URL=http://plov-api:8080 \
    NGINX_PORT=80

# Default listening port — documentation only; override with `-e NGINX_PORT=...`
# and the matching `-p <host>:<NGINX_PORT>` mapping.
EXPOSE 80

# The base image's entrypoint runs envsubst over templates and then `nginx -g 'daemon off;'`.
