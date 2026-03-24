using MediatR;

namespace PlovCenter.Application.Contract.Categories.Commands;

public sealed record DeleteCategoryCommand(Guid CategoryId) : IRequest<Unit>;
