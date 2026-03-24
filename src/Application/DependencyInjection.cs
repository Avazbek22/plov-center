using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PlovCenter.Application.Common.Cqrs;

namespace PlovCenter.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddScoped<IRequestSender, RequestSender>();

        RegisterClosedGenerics(services, assembly, typeof(IApplicationRequestHandler<,>));
        RegisterClosedGenerics(services, assembly, typeof(IValidator<>));

        return services;
    }

    private static void RegisterClosedGenerics(IServiceCollection services, Assembly assembly, Type openGenericType)
    {
        var implementations = assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Select(type => new
            {
                Type = type,
                Services = type.GetInterfaces()
                    .Where(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == openGenericType)
                    .ToArray()
            })
            .Where(candidate => candidate.Services.Length > 0);

        foreach (var implementation in implementations)
        {
            foreach (var serviceType in implementation.Services)
            {
                services.AddScoped(serviceType, implementation.Type);
            }
        }
    }
}
