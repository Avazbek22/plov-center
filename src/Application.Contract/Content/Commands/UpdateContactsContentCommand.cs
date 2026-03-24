using MediatR;
using PlovCenter.Application.Contract.Content.Responses;

namespace PlovCenter.Application.Contract.Content.Commands;

public sealed class UpdateContactsContentCommand : IRequest<AdminSiteContentResponse>
{
    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Hours { get; set; }

    public string? MapEmbed { get; set; }
}
