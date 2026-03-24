using System.Text.Json.Serialization;
using MediatR;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.Application.Contract.Dishes.Commands;

public sealed class UpdateDishCommand : IRequest<DishResponse>
{
    [JsonIgnore]
    public Guid DishId { get; set; }

    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? PhotoPath { get; set; }

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }
}
