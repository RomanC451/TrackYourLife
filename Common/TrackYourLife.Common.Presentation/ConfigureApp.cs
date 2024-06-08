using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using TrackYourLife.Common.Presentation.Middlewares;

namespace TrackYourLife.Common.Presentation;

public static class ConfigureApp
{
    public static void ConfigurePresentationApp(this IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseCors("CORSPolicy");

        app.UseHttpsRedirection();

        app.UseMiddleware<RequestLogContextMiddleware>();

        app.UseSerilogRequestLogging();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks(
                "health",
                new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                }
            );
        });
    }
}
