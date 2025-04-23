using HiLo.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HiLo.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddDbContext<HiLoDbContext>(options =>
            {
                options.UseInMemoryDatabase("HiLo");
            });
    }
}