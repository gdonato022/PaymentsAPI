using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Plooto.Payments.Application.Behaviours;
using System.Reflection;

namespace Plooto.Payments.Application
{
    /// <summary>
    /// Configure dependencies for application
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Add application's dependencies
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(currentAssembly);
            });

            services.AddValidatorsFromAssembly(currentAssembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            return services;
        }
    }
}
