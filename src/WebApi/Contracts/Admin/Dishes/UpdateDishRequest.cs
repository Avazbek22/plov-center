using PlovCenter.Application.Contract.Dishes;

namespace PlovCenter.WebApi.Contracts.Admin.Dishes;

public sealed class UpdateDishRequest
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public IReadOnlyList<DishPhotoInput> Photos { get; set; } = [];

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }
}
