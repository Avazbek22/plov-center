# API Contracts: Admin CRUD Panel

**Source**: Existing backend endpoints. Frontend consumes only — no backend changes needed.

## Base URL

All endpoints prefixed with `/api/admin/`. All require `Authorization: Bearer {jwt}` header.

## Categories — `/api/admin/categories`

### GET `/api/admin/categories`

List all categories.

**Response**: `CategoryResponse[]`

```json
[
  {
    "id": "guid",
    "name": "string",
    "sortOrder": 0,
    "isVisible": true,
    "dishCount": 5,
    "createdUtc": "2026-01-01T00:00:00Z",
    "updatedUtc": "2026-01-01T00:00:00Z"
  }
]
```

### GET `/api/admin/categories/{categoryId}`

Get single category.

**Response**: `CategoryResponse` (same shape as above)

### POST `/api/admin/categories`

Create category.

**Request**:

```json
{
  "name": "string",
  "sortOrder": 0,
  "isVisible": true
}
```

**Response**: `201 Created` with `CategoryResponse`

### PUT `/api/admin/categories/{categoryId}`

Update category.

**Request**:

```json
{
  "name": "string",
  "sortOrder": 0,
  "isVisible": true
}
```

**Response**: `CategoryResponse`

### PATCH `/api/admin/categories/{categoryId}/visibility`

Toggle visibility.

**Request**:

```json
{ "isVisible": true }
```

**Response**: `CategoryResponse`

### PUT `/api/admin/categories/reorder`

Reorder categories.

**Request**:

```json
{
  "items": [
    { "categoryId": "guid", "sortOrder": 0 },
    { "categoryId": "guid", "sortOrder": 1 }
  ]
}
```

**Response**: `204 No Content`

### DELETE `/api/admin/categories/{categoryId}`

Delete category. Fails if category has dishes.

**Response**: `204 No Content`
**Error**: `409 Conflict` if has dishes

---

## Dishes — `/api/admin/dishes`

### GET `/api/admin/dishes?categoryId={guid}`

List dishes, optionally filtered by category.

**Query params**: `categoryId` (optional, guid)

**Response**: `DishResponse[]`

```json
[
  {
    "id": "guid",
    "categoryId": "guid",
    "categoryName": "string",
    "name": "string",
    "description": "string | null",
    "price": 45000.00,
    "photoPath": "string | null",
    "sortOrder": 0,
    "isVisible": true,
    "createdUtc": "2026-01-01T00:00:00Z",
    "updatedUtc": "2026-01-01T00:00:00Z"
  }
]
```

### GET `/api/admin/dishes/{dishId}`

Get single dish.

**Response**: `DishResponse`

### POST `/api/admin/dishes`

Create dish.

**Request**:

```json
{
  "categoryId": "guid",
  "name": "string",
  "description": "string | null",
  "price": 45000.00,
  "photoPath": "string | null",
  "sortOrder": 0,
  "isVisible": true
}
```

**Response**: `201 Created` with `DishResponse`

### PUT `/api/admin/dishes/{dishId}`

Update dish.

**Request**: Same shape as create.

**Response**: `DishResponse`

### PATCH `/api/admin/dishes/{dishId}/visibility`

Toggle visibility.

**Request**:

```json
{ "isVisible": true }
```

**Response**: `DishResponse`

### DELETE `/api/admin/dishes/{dishId}`

Delete dish.

**Response**: `204 No Content`

---

## Content — `/api/admin/content`

### GET `/api/admin/content`

Get all site content.

**Response**: `AdminSiteContentResponse`

```json
{
  "about": {
    "text": "string | null",
    "photoPath": "string | null"
  },
  "contacts": {
    "address": "string | null",
    "phone": "string | null",
    "hours": "string | null",
    "mapEmbed": "string | null"
  }
}
```

### PUT `/api/admin/content/about`

Update "About" section.

**Request**:

```json
{
  "text": "string | null",
  "photoPath": "string | null"
}
```

**Response**: `AdminSiteContentResponse`

### PUT `/api/admin/content/contacts`

Update contacts.

**Request**:

```json
{
  "address": "string | null",
  "phone": "string | null",
  "hours": "string | null",
  "mapEmbed": "string | null"
}
```

**Response**: `AdminSiteContentResponse`

---

## Uploads — `/api/admin/uploads`

### POST `/api/admin/uploads/image`

Upload image file. Multipart/form-data.

**Request**: `multipart/form-data`

| Field | Type | Values |
|-------|------|--------|
| Area  | int  | `1` = Dish, `2` = About |
| File  | file | Image file |

**Response**: `UploadImageResponse`

```json
{
  "relativePath": "/uploads/dishes/filename.jpg",
  "url": "http://localhost:5288/uploads/dishes/filename.jpg",
  "fileName": "filename.jpg",
  "size": 123456
}
```

---

## Error Response

All errors follow `ApiErrorResponse`:

```json
{
  "code": "string",
  "message": "string",
  "traceId": "string",
  "errors": { "fieldName": ["error message"] } | null
}
```

Common codes: `not_found` (404), `conflict` (409), `validation_error` (400), `unauthorized` (401).
