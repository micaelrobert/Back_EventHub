using GestaoEventos.API.Data;
using GestaoEventos.API.Repositories;
using GestaoEventos.API.Services;
using GestaoEventos.API.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoEventos.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IEventoRepository, EventoRepository>();
            services.AddScoped<IIngressoRepository, IngressoRepository>();

            services.AddScoped<IEventoService, EventoService>();
            services.AddScoped<IIngressoService, IngressoService>();

            return services;
        }

        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                                         ?? new[] { "http://localhost:4200" };

                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            return services;
        }
    }
}