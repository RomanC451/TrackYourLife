using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TrackYourLife.SharedLib.Infrastructure.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations<DbType>(this IApplicationBuilder app)
        where DbType : DbContext
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using DbType context = scope.ServiceProvider.GetRequiredService<DbType>();

        context.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS pgcrypto;");

        context.Database.Migrate();
    }
}
