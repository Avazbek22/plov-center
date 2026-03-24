using System.Text.Json.Serialization;
using MediatR;
using PlovCenter.Application.Contract.Categories.Responses;

namespace PlovCenter.Application.Contract.Categories.Commands;

public sealed class UpdateCategoryCommand : IRequest<CategoryResponse>
{
    [JsonIgnore]
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }
}
