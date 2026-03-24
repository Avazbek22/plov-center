using MediatR;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.Application.Contract.Dishes.Commands;

public sealed class CreateDishCommand : IRequest<DishResponse>
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? PhotoPath { get; set; }

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }
}
