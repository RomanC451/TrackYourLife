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

    // public static Task<TOut> Map<TOut>(this Result result, Func<Result, TOut> mappingFunc)
    // {
    //     return result.IsSuccess
    //         ? Result.Success(await mappingFunc(result))
    //         : Result.Failure<TOut>(result.Error);
    // }

    // public static async Task<T> Match<T>(
    //     this Task<Result> resultTask,
    //     Func<T> onSuccess,
    //     Func<Error, T> onFailure
    // )
    // {
    //     Result result = await resultTask;

    //     return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    // }

    // public static async Task<TOut> Match<TIn, TOut>(
    //     this Task<Result<TIn>> resultTask,
    //     Func<TIn, TOut> onSuccess,
    //     Func<Error, TOut> onFailure
    // )
    // {
    //     Result<TIn> result = await resultTask;

    //     return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
    // }

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
