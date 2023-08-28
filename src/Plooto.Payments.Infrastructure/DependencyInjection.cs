using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plooto.Payments.Domain.Interfaces;
using Plooto.Payments.Infrastructure.Storage.Repositories;
using Plooto.Payments.Infrastructure.Storage;
using Plooto.Payments.Infrastructure.Storage.Configuration;
using Plooto.Payments.Domain.Interfaces.Events;
using Plooto.Payments.Infrastructure.Events;

namespace Plooto.Payments.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PaymentDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("PaymentDbConnection")));

            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IBillRepository, BillRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            return services;
        }
    }
}

