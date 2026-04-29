namespace PlovCenter.Application.Contract.Menu.Responses;

public sealed record PublicMenuDishResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    IReadOnlyList<string> Photos,
    int SortOrder);
