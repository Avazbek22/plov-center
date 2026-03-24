using System.Text.Json.Serialization;
using MediatR;
using PlovCenter.Application.Contract.Categories.Responses;

namespace PlovCenter.Application.Contract.Categories.Commands;

public sealed class SetCategoryVisibilityCommand : IRequest<CategoryResponse>
{
    [JsonIgnore]
    public Guid CategoryId { get; set; }

    public bool IsVisible { get; set; }
}
