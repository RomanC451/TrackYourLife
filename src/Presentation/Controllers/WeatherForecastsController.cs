using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.WeatherForecasts.CreateWeatherForecast;

using TrackYourLifeDotnet.Application.WeatherForecasts.Queries;
using TrackYourLifeDotnet.Application.WeatherForecasts.Queries.GetWeatherForecast;
using TrackYourLifeDotnet.Presentation.Abstractions;

using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Application.WeatherForecasts.Queries.GetWeatherForecastById;

namespace TrackYourLifeDotnet.Presentation.Controllers;

[Route("api/[controller]")]
public class WeatherForecastsController : ApiController
{
    public WeatherForecastsController(ISender sender)
        : base(sender) { }

    [HttpGet]
    public async Task<IActionResult> GetWeatherForecastAsync(CancellationToken cancellationToken)
    {
        GetWeatherForecastQuery query = new();

        Result<IEnumerable<GetWeatherForecastResponse>> forecastList = await Sender.Send(
            query,
            cancellationToken
        );

        if (forecastList.IsFailure)
        {
            return BadRequest(forecastList.Error);
        }

        return Ok(forecastList.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWeatherForecastByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        GetWeatherForecastByIdQuery query = new(id);
        Result<GetWeatherForecastResponse> forecast = await Sender.Send(query, cancellationToken);
        if (forecast.IsFailure)
        {
            return BadRequest(forecast.Error);
        }

        return Ok(forecast.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWeatherForecastAsync(
        CreateWeatherForecastCommand request,
        CancellationToken cancellationToken
    )
    {
        Unit result = await Sender.Send(request, cancellationToken);

        return Ok(result);
    }
}
