# syntax=docker/dockerfile:1.7
# ============================================================
# PlovCenter backend — production image
# Build context: repository root
#   docker build -t plov-center-api .
# ============================================================

ARG DOTNET_VERSION=10.0

# ---------- build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

# Copy solution + csproj files first so `dotnet restore` is cached
# until project dependencies actually change.
COPY global.json ./
COPY PlovCenter.sln ./
COPY src/Domain/PlovCenter.Domain.csproj                             src/Domain/
COPY src/Application.Contract/PlovCenter.Application.Contract.csproj src/Application.Contract/
COPY src/Application/PlovCenter.Application.csproj                   src/Application/
COPY src/Infrastructure/PlovCenter.Infrastructure.csproj             src/Infrastructure/
COPY src/WebApi/PlovCenter.WebApi.csproj                             src/WebApi/

RUN dotnet restore src/WebApi/PlovCenter.WebApi.csproj

# Copy the rest of the source and publish
COPY src/ src/

RUN dotnet publish src/WebApi/PlovCenter.WebApi.csproj \
        -c Release \
        -o /app/publish \
        --no-restore \
        /p:UseAppHost=false

# ---------- runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

# Ensure the uploads directory exists and is writable by the
# non-root user from the base image ($APP_UID = 1654 in .NET 8+).
RUN mkdir -p /app/wwwroot/uploads \
 && chown -R $APP_UID:$APP_UID /app

USER $APP_UID

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

ENTRYPOINT ["dotnet", "PlovCenter.WebApi.dll"]
