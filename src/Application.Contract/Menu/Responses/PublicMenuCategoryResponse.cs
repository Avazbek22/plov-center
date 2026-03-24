namespace PlovCenter.Application.Contract.Menu.Responses;

public sealed record PublicMenuCategoryResponse(
    Guid Id,
    string Name,
    int SortOrder,
    IReadOnlyCollection<PublicMenuDishResponse> Dishes);
