using MediatR;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.Application.Contract.Dishes.Commands;

public sealed class SetDishVisibilityCommand : IRequest<DishResponse>
{
    public Guid DishId { get; set; }

    public bool IsVisible { get; set; }
}
