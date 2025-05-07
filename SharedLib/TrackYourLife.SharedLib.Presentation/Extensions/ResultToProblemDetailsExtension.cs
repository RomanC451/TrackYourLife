using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Presentation.Extensions;

public static class ResultToProblemDetailsExtension
{
    private static ProblemDetails CreateProblemDetails(
        string title,
        int status,
        Error error,
        Error[]? errors = null
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
                validationResult.Errors
            ),

            _ => CreateProblemDetails("Bad Request", StatusCodes.Status400BadRequest, result.Error),
        };
    }

    public static ProblemDetails ToNoFoundProblemDetails(this Result result)
    {
        return CreateProblemDetails("Not Found", StatusCodes.Status404NotFound, result.Error);
    }

    public static ProblemDetails ToForbiddenProblemDetails(this Result result)
    {
        return CreateProblemDetails("Forbidden", StatusCodes.Status403Forbidden, result.Error);
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
}
