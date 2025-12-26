using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SAS.Backend.Application.Mapping;
using System.Reflection;

namespace SAS.Backend.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });
            return services;
        }
    }
}

