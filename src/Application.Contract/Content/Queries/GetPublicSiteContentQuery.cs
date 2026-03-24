using MediatR;
using PlovCenter.Application.Contract.Content.Responses;

namespace PlovCenter.Application.Contract.Content.Queries;

public sealed record GetPublicSiteContentQuery : IRequest<PublicSiteContentResponse>;
