namespace PlovCenter.Application.Contract.Dishes.Responses;

public sealed record DishResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string Name,
    string? Description,
    decimal Price,
    IReadOnlyList<DishPhotoResponse> Photos,
    int SortOrder,
    bool IsVisible,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);
