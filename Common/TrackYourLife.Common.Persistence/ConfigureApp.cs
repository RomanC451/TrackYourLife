using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TrackYourLife.Common.Persistence;

public static class ConfigureApp
{
    public static void ConfigurePersistenceApp(this IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices.CreateScope();

        using ApplicationWriteDbContext dbContext =
            serviceScope.ServiceProvider.GetRequiredService<ApplicationWriteDbContext>();

        dbContext.Database.Migrate();
    }
}
