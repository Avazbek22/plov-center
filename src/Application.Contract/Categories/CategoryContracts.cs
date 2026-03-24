namespace PlovCenter.Application.Contract.Categories;

public sealed record CreateCategoryRequest(string Name, int SortOrder, bool IsVisible);

public sealed record UpdateCategoryRequest(string Name, int SortOrder, bool IsVisible);

public sealed record SetCategoryVisibilityRequest(bool IsVisible);

public sealed record ReorderCategoriesRequest(IReadOnlyCollection<ReorderCategoryItemRequest> Items);

public sealed record ReorderCategoryItemRequest(Guid CategoryId, int SortOrder);

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    int SortOrder,
    bool IsVisible,
    int DishCount,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);
