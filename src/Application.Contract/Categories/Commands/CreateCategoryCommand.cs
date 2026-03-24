using MediatR;
using PlovCenter.Application.Contract.Categories.Responses;

namespace PlovCenter.Application.Contract.Categories.Commands;

public sealed class CreateCategoryCommand : IRequest<CategoryResponse>
{
    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }
}
