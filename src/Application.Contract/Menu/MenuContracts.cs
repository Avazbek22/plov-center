namespace PlovCenter.Application.Contract.Menu;

public sealed record PublicMenuDishResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string? PhotoPath,
    int SortOrder);

public sealed record PublicMenuCategoryResponse(
    Guid Id,
    string Name,
    int SortOrder,
    IReadOnlyCollection<PublicMenuDishResponse> Dishes);

public sealed record PublicMenuResponse(IReadOnlyCollection<PublicMenuCategoryResponse> Categories);
