namespace PlovCenter.Application.Contract.Dishes;

public sealed record CreateDishRequest(
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    string? PhotoPath,
    int SortOrder,
    bool IsVisible);

public sealed record UpdateDishRequest(
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    string? PhotoPath,
    int SortOrder,
    bool IsVisible);

public sealed record SetDishVisibilityRequest(bool IsVisible);

public sealed record DishResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string Name,
    string? Description,
    decimal Price,
    string? PhotoPath,
    int SortOrder,
    bool IsVisible,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);
