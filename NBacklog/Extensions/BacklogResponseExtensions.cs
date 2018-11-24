using System.Linq;

namespace NBacklog.Extensions
{
    internal static class BacklogResponseExtensions
    {
        public static bool CanContinueBatchJobs<T>(this BacklogResponse<T> response, ErrorHandler onError)
        {
            if (onError == null || !response.Errors.Any())
            {
                return true;
            }

            foreach (var error in response.Errors)
            {
                if (!onError(error))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
