using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace TrackYourLife.Common.Infrastructure;

public static class ConfigureApp
{
    public static void ConfigureInfrastructureApp(
        this IApplicationBuilder app,
        IWebHostEnvironment env
    )
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add(
                    (swagger, httpReq) =>
                    {
                        swagger.Servers = new List<OpenApiServer>
                        {
                            new OpenApiServer { Url = "https://localhost:7196" }
                        };
                    }
                );
            });

            app.UseSwaggerUI(
                swaggerUiOptions =>
                    swaggerUiOptions.SwaggerEndpoint(
                        "/swagger/v1/swagger.json",
                        "TrackYourLife API"
                    )
            );
        }
    }
}
