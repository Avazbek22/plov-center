using MediatR;

namespace PlovCenter.Application.Contract.Dishes.Commands;

public sealed record DeleteDishCommand(Guid DishId) : IRequest<Unit>;
