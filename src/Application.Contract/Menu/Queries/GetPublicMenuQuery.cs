using MediatR;
using PlovCenter.Application.Contract.Menu.Responses;

namespace PlovCenter.Application.Contract.Menu.Queries;

public sealed record GetPublicMenuQuery : IRequest<PublicMenuResponse>;
