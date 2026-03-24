using MediatR;
using PlovCenter.Application.Contract.Content.Responses;

namespace PlovCenter.Application.Contract.Content.Commands;

public sealed class UpdateAboutContentCommand : IRequest<AdminSiteContentResponse>
{
    public string? Text { get; set; }

    public string? PhotoPath { get; set; }
}
