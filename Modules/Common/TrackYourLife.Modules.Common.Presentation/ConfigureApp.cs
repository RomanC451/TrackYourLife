using FastEndpoints;
using FastEndpoints.Swagger;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using TrackYourLife.Modules.Common.Presentation.Middlewares;

namespace TrackYourLife.Modules.Common.Presentation;

public static class ConfigureApp
{
    public static void ConfigureCommonPresentationApp(this IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseCors("CORSPolicy");

        //app.UseHttpsRedirection();


        app.UseMiddleware<RequestLogContextMiddleware>();

        app.UseSerilogRequestLogging();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
            {
                endpoints.MapFastEndpoints(c =>
                {
                    c.Endpoints.ShortNames = true;
                });
                endpoints.MapHealthChecks(
                    "health",
                    new HealthCheckOptions
                    {
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    }
                );
            })
            .UseSwaggerGen(s =>
            {
                s.PostProcess = (document, request) =>
                {
                    document.Servers.Clear();
                    document.Servers.Add(new() { Url = "http://192.168.1.12:5001/" });
                };
            });
    }
}
