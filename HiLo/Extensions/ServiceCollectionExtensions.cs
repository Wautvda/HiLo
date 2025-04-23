using HiLo.Configuration;
using HiLo.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HiLo.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext<HiLoDbContext>(options => { options.UseInMemoryDatabase("HiLo"); });
        services.Configure<GameConfiguration>(configuration);

        return services;
    }
}