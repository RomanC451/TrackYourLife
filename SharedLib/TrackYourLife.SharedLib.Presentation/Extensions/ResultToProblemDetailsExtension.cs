using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Presentation.Extensions;

public static class ResultToProblemDetailsExtension
{
    private static ProblemDetails CreateProblemDetails<ErrorsType>(
        string title,
        int status,
        Error error,
        ErrorsType[]? errors = null
    )
    {
        ProblemDetails problemDetails = new ProblemDetails
        {
            Title = title,
            Type = error.Code,
            Detail = error.Message,
            Status = status,
        };

        if (errors is not null && errors.Length > 0)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        return problemDetails;
    }

    public static ProblemDetails ToBadRequestProblemDetails(this Result result)
    {
        return result switch
        {
            IValidationResult validationResult => CreateProblemDetails(
                "Validation Error",
                StatusCodes.Status400BadRequest,
                result.Error,
                validationResult.Errors.Select(e => e.ToValidationError()).ToArray()
            ),

            _ => CreateProblemDetails<Error>(
                "Bad Request",
                StatusCodes.Status400BadRequest,
                result.Error
            ),
        };
    }

    public static ProblemDetails ToNoFoundProblemDetails(this Result result)
    {
        return CreateProblemDetails<Error>(
            "Not Found",
            StatusCodes.Status404NotFound,
            result.Error
        );
    }

    public static ProblemDetails ToForbiddenProblemDetails(this Result result)
    {
        return CreateProblemDetails<Error>(
            "Forbidden",
            StatusCodes.Status403Forbidden,
            result.Error
        );
    }

    public static IResult ToActionResult(this Result result)
    {
        return result switch
        {
            { IsSuccess: true } => TypedResults.NoContent(),
            { Error.HttpStatus: 403 } => TypedResults.Problem(
                detail: result.ToForbiddenProblemDetails().Detail,
                statusCode: StatusCodes.Status403Forbidden
            ),
            { Error.HttpStatus: 404 } => TypedResults.NotFound(result.ToNoFoundProblemDetails()),
            _ => TypedResults.BadRequest(result.ToBadRequestProblemDetails()),
        };
    }

    public static async Task<IResult> ToActionResultAsync(this Task<Result> taskResult)
    {
        var result = await taskResult;

        return result switch
        {
            { IsSuccess: true } => TypedResults.NoContent(),
            { Error.HttpStatus: 403 } => TypedResults.Problem(
                detail: result.ToForbiddenProblemDetails().Detail,
                statusCode: StatusCodes.Status403Forbidden
            ),
            { Error.HttpStatus: 404 } => TypedResults.NotFound(result.ToNoFoundProblemDetails()),
            _ => TypedResults.BadRequest(result.ToBadRequestProblemDetails()),
        };
    }

    public static async Task<IResult> ToActionResultAsync<TValue, TResponse>(
        this Task<Result<TValue>> taskResult,
        Func<TValue, TResponse> successResponse
    )
    {
        var result = await taskResult;

        return result switch
        {
            { IsSuccess: true } => TypedResults.Ok(successResponse(result.Value)),
            { Error.HttpStatus: 403 } => TypedResults.Problem(
                detail: result.ToForbiddenProblemDetails().Detail,
                statusCode: StatusCodes.Status403Forbidden
            ),
            { Error.HttpStatus: 404 } => TypedResults.NotFound(result.ToNoFoundProblemDetails()),
            _ => TypedResults.BadRequest(result.ToBadRequestProblemDetails()),
        };
    }

    public static async Task<IResult> ToCreatedActionResultAsync<TId>(
        this Task<Result<TId>> taskResult,
        Func<TId, string> getRoute
    )
        where TId : IStronglyTypedGuid
    {
        var result = await taskResult;

        return result switch
        {
            { IsSuccess: true } => TypedResults.Created(
                getRoute(result.Value),
                new IdResponse(result.Value)
            ),
            { Error.HttpStatus: 403 } => TypedResults.Problem(
                detail: result.ToForbiddenProblemDetails().Detail,
                statusCode: StatusCodes.Status403Forbidden
            ),
            { Error.HttpStatus: 404 } => TypedResults.NotFound(result.ToNoFoundProblemDetails()),
            _ => TypedResults.BadRequest(result.ToBadRequestProblemDetails()),
        };
    }

    public static async Task<IResult> ToCreatedActionResultAsync(
        this Task<Result> taskResult,
        string route
    )
    {
        var result = await taskResult;

        return result switch
        {
            { IsSuccess: true } => TypedResults.Created(route),
            { Error.HttpStatus: 403 } => TypedResults.Problem(
                detail: result.ToForbiddenProblemDetails().Detail,
                statusCode: StatusCodes.Status403Forbidden
            ),
            { Error.HttpStatus: 404 } => TypedResults.NotFound(result.ToNoFoundProblemDetails()),
            _ => TypedResults.BadRequest(result.ToBadRequestProblemDetails()),
        };
    }
}
