using ETS.Domain.Common;

namespace ETS.Domain.Extensions
{
    public static class ResultExtension
    {
        public static TOut Match<T, TOut>(
            this Result result,
            Func<TOut> onSuccess, 
            Func<Result, TOut> onFailure)
        {
            return result.IsSuccess ? onSuccess() : onFailure(result);
        }

        public static async Task<TOut> MatchAsync<TIn, TOut>(
            this Task<Result<TIn>> resultTask,
            Func<TIn, TOut> onSuccess,
            Func<Result<TIn>, TOut> onFailure)
        {
            var result = await resultTask;
            return result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
        }
    }
}
