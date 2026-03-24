using MediatR;

namespace PlovCenter.Application.Contract.Categories.Commands;

public sealed class ReorderCategoriesCommand : IRequest<Unit>
{
    public IReadOnlyCollection<ReorderCategoryItem> Items { get; set; } = [];
}
