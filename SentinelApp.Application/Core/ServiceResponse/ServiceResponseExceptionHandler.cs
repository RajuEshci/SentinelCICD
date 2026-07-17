using Serilog;
using System.Net;

namespace SentinelApp.Application.Core
{
    public static class ServiceResponseExceptionHandler
    {
        //public static ServiceResponse<T> HandleWithoutReturningException<T>(Action action, T responseObject,  params object[] logObjects)
        //{
        //    try
        //    {
        //        action?.Invoke();
        //    }
        //    catch (ServiceResponseException exception)
        //    {
        //        Log.Fatal(exception, $"{nameof(ServiceResponseExceptionHandler)} Unhandled {nameof(ServiceResponseException)} {nameof(T)}");
        //        return ServiceResponse<T>.ErrorResponse(exception.HttpStatusCode, exception.Message);
        //    }
        //    return ServiceResponse<T>.SuccessResponse(responseObject);
        //}

        public static async Task<ServiceResponse<T>> Handle<T>(Func<Task<T>> action, string successMessage = "Done", params object[] logObjects)
        {
            try
            {
                var result = await action(); // Note the await here
                return ServiceResponse<T>.SuccessResponse(result,successMessage);
            }
            catch (ServiceResponseException exception)
            {
                Log.Fatal(exception, $"{nameof(ServiceResponseExceptionHandler)} Unhandled {nameof(ServiceResponseException)} {nameof(T)}");
                return ServiceResponse<T>.ErrorResponse(exception.HttpStatusCode, exception.CustomMessage);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, $"{nameof(ServiceResponseExceptionHandler)} Unhandled exception {nameof(T)}, LogObjects: {string.Join("{@}", logObjects)}");
                return ServiceResponse<T>.ErrorResponse(HttpStatusCode.InternalServerError, exception.Message, exception);
            }
            finally
            {
                Log.Information($"{nameof(ServiceResponseExceptionHandler)}, Service call completed. {nameof(T)}");
            }
        }
    }
}
