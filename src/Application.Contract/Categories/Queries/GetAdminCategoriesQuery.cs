using MediatR;
using PlovCenter.Application.Contract.Categories.Responses;

namespace PlovCenter.Application.Contract.Categories.Queries;

public sealed record GetAdminCategoriesQuery : IRequest<IReadOnlyCollection<CategoryResponse>>;
