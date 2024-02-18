using esplay.core;
using esplay.persistence;
using esplay.processing;
using esplay.stream;
using Microsoft.Extensions.DependencyInjection;
using ITimer = esplay.core.ITimer;
using Timer = esplay.core.Timer;

namespace esplay.common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEsPlay(this IServiceCollection services)
    {
        services.AddSingleton<ILogger, Logger>();
        services.AddSingleton<ITimer, Timer>();
        services.AddTransient<IFingerprintService, FingerprintService>();
        services.AddSingleton<IEventStream, EventStream>();
        services.AddScoped<IAggregateBuilder, AggregateBuilder>();
        return services;
    }
    
    public static IServiceCollection AddAggregate<T>(this IServiceCollection services)
        where T: IAggregate<T>, new()
    {
        services.AddSingleton<IRepository<T>, Repository<T>>();
        services.AddScoped<ICommand<T>, Command<T>>();
        services.AddScoped<IQuery<T>, Query<T>>();

        return services;
    }
}