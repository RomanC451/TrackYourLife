using TrackYourLife.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MapsterMapper;
using Microsoft.FeatureManagement;
using Microsoft.AspNetCore.Authorization;
using TrackYourLife.Common.Presentation;

namespace TrackYourLife.Common.Presentation.Abstractions;

[ApiController]
[Authorize]
public abstract class ApiController : ControllerBase
{
    protected readonly ISender _sender;

    protected readonly IMapper _mapper;

    protected readonly IFeatureManager _featureManager;

    protected ApiController(ISender sender, IMapper mapper, IFeatureManager featureManager)
    {
        _sender = sender;
        _mapper = mapper;
        _featureManager = featureManager;
    }

    protected async Task<IActionResult> HandleFailure(Result result)
    {
        return result switch
        {
            { Error.IsInternal: true }
                => StatusCode(
                    StatusCodes.Status500InternalServerError,
                    CreateProblemDetails(
                        "Internal Server Error",
                        StatusCodes.Status500InternalServerError,
                        await _featureManager.IsEnabledAsync(FeatureFlags.ExposeInternalErrors)
                            ? result.Error
                            : new Error("Internal", "An internal error occurred.")
                    )
                ),
            IValidationResult validationResult
                => BadRequest(
                    CreateProblemDetails(
                        "Validation Error",
                        StatusCodes.Status400BadRequest,
                        result.Error,
                        validationResult.Errors
                    )
                ),
            { Error.Code: string code } when code.Contains("NotFound")
                => NotFound(
                    CreateProblemDetails("Not Found", StatusCodes.Status404NotFound, result.Error)
                ),
            _
                => BadRequest(
                    CreateProblemDetails(
                        "Bad Request",
                        StatusCodes.Status400BadRequest,
                        result.Error
                    )
                )
        };
    }

    protected static ProblemDetails CreateProblemDetails(
        string title,
        int status,
        Error error,
        Error[]? errors = null
    ) =>
        new()
        {
            Title = title,
            Type = error.Code,
            Detail = error.Message,
            Status = status,
            Extensions = { { nameof(errors), errors } }
        };

    protected async Task<IActionResult> MatchResponse(Result result) =>
        result.IsFailure ? await HandleFailure(result) : Ok();

    protected async Task<IActionResult> MatchResponse<TOut>(Result<TOut> result) =>
        result.IsFailure ? await HandleFailure(result) : Ok(result.Value);
}
