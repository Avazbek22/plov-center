namespace PlovCenter.Application.Contract.Dishes.Responses;

public sealed record DishPhotoResponse(Guid Id, string RelativePath, int SortOrder);
