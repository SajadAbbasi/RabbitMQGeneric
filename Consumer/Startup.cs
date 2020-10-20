using Consumer.EventHandlers;
using Domain.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using RabbitMQGeneric;
using System;

namespace Consumer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "subscriber Api", Version = "v1" });
            });

            services.AddControllers();
            services.RegisterMessageBus(Configuration);
            services.AddTransient<TestDomainEventHandler>();
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            var sa = serviceProvider.GetRequiredService<IMessageBus>();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "subscriber Api V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ConfigureMessageBus(serviceProvider);
        }

        private void ConfigureMessageBus(IServiceProvider serviceProvider)
        {
            var messageBus = serviceProvider.GetRequiredService<IMessageBus>();
            messageBus.Subscribe<TestDomainEvent, TestDomainEventHandler>();
        }
    }
}
