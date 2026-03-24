# Data Model: Admin Frontend Setup

## Frontend Types (TypeScript)

### AuthSession

Состояние авторизации, хранится в React Context.

| Field           | Type                | Description                        |
|-----------------|---------------------|------------------------------------|
| isAuthenticated | boolean             | Авторизован ли пользователь        |
| token           | string \| null      | JWT bearer token                   |
| expiresAtUtc    | string \| null      | ISO 8601 дата истечения токена      |
| admin           | AdminUser \| null   | Данные текущего администратора     |

### AdminUser

| Field    | Type    | Description              |
|----------|---------|--------------------------|
| id       | string  | UUID администратора      |
| username | string  | Имя пользователя         |
| isActive | boolean | Активен ли аккаунт       |

### LoginCredentials

| Field    | Type   | Validation                  |
|----------|--------|-----------------------------|
| username | string | Обязательное, непустое      |
| password | string | Обязательное, непустое      |

### LoginResponse (from API)

| Field        | Type          | Description                     |
|--------------|---------------|---------------------------------|
| token        | string        | JWT bearer token                |
| expiresAtUtc | string        | ISO 8601 дата истечения          |
| admin        | AdminUser     | Данные администратора           |

### ApiErrorResponse (from API)

| Field   | Type                              | Description              |
|---------|-----------------------------------|--------------------------|
| code    | string                            | Код ошибки               |
| message | string                            | Сообщение                |
| traceId | string                            | ID трассировки           |
| errors  | Record<string, string[]> \| null  | Ошибки валидации по полям |

## State Transitions

```
[Not Authenticated] --login success--> [Authenticated]
[Authenticated] --logout--> [Not Authenticated]
[Authenticated] --token expired--> [Not Authenticated]
[Any] --navigate /admin/*--> [Check Token] --no token--> [Redirect /admin/login]
```
