# API Contracts: PlovCenter Frontend ↔ Backend

Контракты соответствуют существующему бэкенду PlovCenter (ASP.NET Core). Фронтенд ничего не меняет в бэкенде.

## Auth

### POST /api/admin/auth/login

Авторизация администратора.

**Request**:
```json
{
  "username": "admin",
  "password": "Admin123!"
}
```

**Response 200**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAtUtc": "2026-03-24T14:00:00Z",
  "admin": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "username": "admin",
    "isActive": true
  }
}
```

**Response 401** (неверные креды):
```json
{
  "code": "unauthorized",
  "message": "Invalid username or password.",
  "traceId": "00-abc123..."
}
```

**Response 400** (validation):
```json
{
  "code": "validation_error",
  "message": "One or more validation errors occurred.",
  "traceId": "00-abc123...",
  "errors": {
    "Username": ["'Username' must not be empty."],
    "Password": ["'Password' must not be empty."]
  }
}
```

### GET /api/admin/auth/me

Получение текущего администратора по токену. Используется для проверки валидности сессии.

**Headers**: `Authorization: Bearer <token>`

**Response 200**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "username": "admin",
  "isActive": true
}
```

**Response 401** (нет/невалидный токен):
```json
{
  "code": "unauthorized",
  "message": "Authentication is required.",
  "traceId": "00-abc123..."
}
```
