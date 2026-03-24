using MediatR;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.Application.Contract.Dishes.Queries;

public sealed record GetDishByIdQuery(Guid DishId) : IRequest<DishResponse>;
