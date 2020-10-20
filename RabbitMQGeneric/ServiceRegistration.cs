using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace RabbitMQGeneric
{
    public static class ServiceRegistration
    {
        private static RabbitMQConfig _rabbitMQConfig;
        public static IServiceCollection RegisterMessageBus(this IServiceCollection services, IConfiguration configuration)
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

            services.AddSingleton<IMessageBus, MessageBus>(serviceProvider =>
            {
                return new MessageBus(factory, serviceProvider);
            });

            return services;
        }
    }
}
