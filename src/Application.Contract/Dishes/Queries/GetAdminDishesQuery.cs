using MediatR;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.Application.Contract.Dishes.Queries;

public sealed class GetAdminDishesQuery : IRequest<IReadOnlyCollection<DishResponse>>
{
    public Guid? CategoryId { get; set; }
}
