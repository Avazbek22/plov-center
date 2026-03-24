namespace PlovCenter.Application.Contract.Categories.Responses;

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    int SortOrder,
    bool IsVisible,
    int DishCount,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);
