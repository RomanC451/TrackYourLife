using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TrackYourLife.Modules.Users.Infrastructure.Data;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Users.Infrastructure;

public static class ConfigureApp
{
    public static void ConfigureUsersInfrastructureApp(
        this IApplicationBuilder app,
        IWebHostEnvironment env
    )
    {
        if (env.IsDevelopment())
        {
            //Apply migrations
            app.ApplyMigrations<UsersWriteDbContext>();
        }
    }
}
