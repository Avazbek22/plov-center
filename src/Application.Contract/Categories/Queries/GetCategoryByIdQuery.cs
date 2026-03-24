using MediatR;
using PlovCenter.Application.Contract.Categories.Responses;

namespace PlovCenter.Application.Contract.Categories.Queries;

public sealed record GetCategoryByIdQuery(Guid CategoryId) : IRequest<CategoryResponse>;
