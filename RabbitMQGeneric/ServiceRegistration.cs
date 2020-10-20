using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace RabbitMQGeneric
{
    public static class ServiceRegistration
    {
        private static RabbitMQConfig _rabbitMQConfig;
        public static IServiceCollection RegisterEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMQConfig>(configuration.GetSection(nameof(RabbitMQConfig)));
            services.AddSingleton(st => st.GetRequiredService<IOptions<RabbitMQConfig>>().Value);

            _rabbitMQConfig = services.BuildServiceProvider().GetService<RabbitMQConfig>();

            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMQConfig.HostName,
                UserName = _rabbitMQConfig.UserName,
                Password = _rabbitMQConfig.Password,
                DispatchConsumersAsync = true
            };

            services.AddSingleton<IEventBus, EventBus>(serviceProvider =>
            {
                return new EventBus(factory, serviceProvider);
            });

            return services;
        }
    }
}
