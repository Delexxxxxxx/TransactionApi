using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransactionApi.Data;

namespace TransactionApi.Extensions
{
    public static class DbExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
            services.AddDbContextFactory<TransactionDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            return services;
        }
    }
}
