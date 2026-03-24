# Data Model: Admin CRUD Panel

**Note**: Backend entities are already implemented. This documents the frontend TypeScript types that map to backend DTOs.

## Frontend Types

### CategoryResponse

Maps to: `GET/POST/PUT /api/admin/categories`

```
CategoryResponse
├── id: string (GUID)
├── name: string
├── sortOrder: number
├── isVisible: boolean
├── dishCount: number
├── createdUtc: string (ISO 8601)
└── updatedUtc: string (ISO 8601)
```

### CreateCategoryRequest / UpdateCategoryRequest

```
CategoryFormData
├── name: string (required, non-empty)
├── sortOrder: number (required, >= 0)
└── isVisible: boolean (default: true)
```

### DishResponse

Maps to: `GET/POST/PUT /api/admin/dishes`

```
DishResponse
├── id: string (GUID)
├── categoryId: string (GUID)
├── categoryName: string
├── name: string
├── description: string | null
├── price: number (decimal)
├── photoPath: string | null
├── sortOrder: number
├── isVisible: boolean
├── createdUtc: string (ISO 8601)
└── updatedUtc: string (ISO 8601)
```

### CreateDishRequest / UpdateDishRequest

```
DishFormData
├── categoryId: string (required, GUID)
├── name: string (required, non-empty)
├── description: string | null (optional)
├── price: number (required, > 0)
├── photoPath: string | null (optional)
├── sortOrder: number (required, >= 0)
└── isVisible: boolean (default: true)
```

### AdminSiteContentResponse

Maps to: `GET/PUT /api/admin/content`

```
AdminSiteContentResponse
├── about
│   ├── text: string | null
│   └── photoPath: string | null
└── contacts
    ├── address: string | null
    ├── phone: string | null
    ├── hours: string | null
    └── mapEmbed: string | null
```

### UploadImageResponse

Maps to: `POST /api/admin/uploads/image`

```
UploadImageResponse
├── relativePath: string
├── url: string
├── fileName: string
└── size: number
```

## Relationships

```
Category 1──∞ Dish
  └── dishCount reflects count

SiteContent is key-value based:
  "about_text", "about_photo" → AboutContent
  "contacts_address", "contacts_phone", "contacts_hours", "contacts_map_embed" → ContactsContent
```

## Validation Rules (Client-Side)

### Category
- `name`: required, min 1 char, max 200 chars
- `sortOrder`: required, integer >= 0

### Dish
- `name`: required, min 1 char, max 200 chars
- `categoryId`: required, valid GUID
- `price`: required, number > 0
- `sortOrder`: required, integer >= 0
- `description`: optional, max 2000 chars

### Image Upload
- File types: JPEG, PNG, WebP
- Max size: 5MB (backend enforced)
