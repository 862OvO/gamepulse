using GamePulse.Application.Abstractions;
using GamePulse.Infrastructure.Data;
using GamePulse.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GamePulse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<GamePulseDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IGameEventRepository, GameEventRepository>();
        return services;
    }
}
