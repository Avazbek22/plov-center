namespace PlovCenter.Application.Contract.Menu.Responses;

public sealed record PublicMenuResponse(IReadOnlyCollection<PublicMenuCategoryResponse> Categories);
