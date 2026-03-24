using System.Text.Json.Serialization;
using MediatR;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.Application.Contract.Dishes.Commands;

public sealed class SetDishVisibilityCommand : IRequest<DishResponse>
{
    [JsonIgnore]
    public Guid DishId { get; set; }

    public bool IsVisible { get; set; }
}
