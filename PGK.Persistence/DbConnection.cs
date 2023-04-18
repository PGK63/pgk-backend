using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace PGK.Persistence
{
    public static class DbConnection
    {
        public static IServiceCollection MySqlConnection(this IServiceCollection services)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "cfif31.ru",
                Port = 3306,
                UserID = "ISPr24-39_BeluakovDS",
                Password = "ISPr24-39_BeluakovDS",
                Database = "ISPr24-39_BeluakovDS_PGK",
                CharacterSet = "utf8"
            };

            services.AddDbContext<PGKDbContext>(options =>
            {
                options.UseMySql(builder.ConnectionString,
                    ServerVersion.AutoDetect(builder.ConnectionString));
            });

            return services;
        }
    }
}
