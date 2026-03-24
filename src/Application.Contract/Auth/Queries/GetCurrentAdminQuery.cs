using MediatR;
using PlovCenter.Application.Contract.Auth.Responses;

namespace PlovCenter.Application.Contract.Auth.Queries;

public sealed record GetCurrentAdminQuery : IRequest<CurrentAdminResponse>;
