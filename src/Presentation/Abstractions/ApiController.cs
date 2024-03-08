using TrackYourLifeDotnet.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MapsterMapper;

namespace TrackYourLifeDotnet.Presentation.Abstractions;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected readonly ISender _sender;

    protected readonly IMapper _mapper;

    protected ApiController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    protected IActionResult HandleFailure(Result result) =>
        result switch
        {
            { IsSuccess: true } => throw new InvalidOperationException(),
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

    protected IActionResult MatchResponse<TResult, TResponse>(Result<TResult> result)
    {
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        if (result.Value is null)
        {
            return Ok();
        }

        return Ok(_mapper.Map<TResponse>(result.Value));
    }

    protected IActionResult MatchResponse(Result result)
    {
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok();
    }
}
