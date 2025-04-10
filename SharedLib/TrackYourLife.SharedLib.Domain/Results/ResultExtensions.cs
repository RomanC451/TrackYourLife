using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.SharedLib.Domain.Results;

public static class ResultExtensions
{
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Error error)
    {
        if (result.IsFailure)
            return result;

        return predicate(result.Value) ? result : Result.Failure<T>(error);
    }

    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mappingFunc)
    {
        return result.IsSuccess
            ? Result.Success(mappingFunc(result.Value))
            : Result.Failure<TOut>(result.Error);
    }

    public static async Task<TOut> MapAsync<TOut>(
        this Result result,
        Func<Result, Task<TOut>> matchFunc
    ) => await matchFunc(result);

    public static async Task<TOut> MapAsync<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<Result<TIn>, Task<TOut>> matchFunc
    )
    {
        Result<TIn> result = await resultTask;

        return await matchFunc(result);
    }

    public static async Task<TOut> MapAsync<TOut>(
        this Task<Result> resultTask,
        Func<Result, Task<TOut>> matchFunc
    )
    {
        Result result = await resultTask;

        return await matchFunc(result);
    }

    public static async Task<Result> BindAsync<TIn>(
        this Result<TIn> result,
        Func<TIn, Task<Result>> func
    ) => result.IsSuccess ? await func(result.Value) : Result.Failure(result.Error);

    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<Result<TOut>>> func
    ) => result.IsSuccess ? await func(result.Value) : Result.Failure<TOut>(result.Error);
}
