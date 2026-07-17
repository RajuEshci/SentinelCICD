using Microsoft.IdentityModel.Tokens.Experimental;
using System.Net;

namespace SentinelApp.Application.Core
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string ResponseMessage { get; set; }
        public T DataResult { get; internal set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public int HttpStatusCodeNumber { get; set; }
        public ServiceResponseException Exception { get; internal set; }
        public string ExceptionNumber { get; internal set; }
        public Dictionary<string, string[]> ValidationErrors { get; set; }

        public static ServiceResponse<T> ValidationErrorResponse(string message, Dictionary<string, string[]> validationErrors)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                HttpStatusCode = HttpStatusCode.BadRequest,
                HttpStatusCodeNumber = (int)HttpStatusCode.BadRequest,
                ResponseMessage = message,
                ValidationErrors = validationErrors
            };
        }
        public static Task<ServiceResponse<T>> SuccessResponse(Task<T> result)
        {
            return TaskRunnerFactory.RunTask<ServiceResponse<T>>(() =>
                new ServiceResponse<T>
                {
                    Success = true,
                    HttpStatusCode = HttpStatusCode.OK,
                    HttpStatusCodeNumber = (int)HttpStatusCode.OK,
                    DataResult = result.Result,
                    ResponseMessage = "DONE"
                }
            );
        }
        public static ServiceResponse<T> SuccessResponse(Task<T> result, string successMessage)
        {
            return new ServiceResponse<T>
            {
                Success = true,
                HttpStatusCode = HttpStatusCode.OK,
                HttpStatusCodeNumber = (int)HttpStatusCode.OK,
                DataResult = result.Result,
                ResponseMessage = successMessage
            };
        }
        public static ServiceResponse<T> SuccessResponse(T result, string successMessage)
        {
            return new ServiceResponse<T>
            {
                Success = true,
                HttpStatusCode = HttpStatusCode.OK,
                HttpStatusCodeNumber = (int)HttpStatusCode.OK,
                DataResult = result,
                ResponseMessage = successMessage
            };
        }
        public static ServiceResponse<T> SuccessResponse(T result)
        {
            return new ServiceResponse<T>
            {
                Success = true,
                HttpStatusCode = HttpStatusCode.OK,
                HttpStatusCodeNumber = (int)HttpStatusCode.OK,
                DataResult = result,
                ResponseMessage = "DONE"
            };
        }
        public static ServiceResponse<T> ErrorResponse(HttpStatusCode httpStatusCode, string errorMessage, Dictionary<string, string[]> validationErrors = null)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                HttpStatusCode = httpStatusCode,
                HttpStatusCodeNumber = (int)httpStatusCode,
                ResponseMessage = errorMessage,
                ValidationErrors = validationErrors
            };
        }
        public static ServiceResponse<T> ErrorResponse(HttpStatusCode httpStatusCode, string errorMessage, Exception exception, Dictionary<string, string[]> validationErrors = null)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                HttpStatusCode = httpStatusCode,
                HttpStatusCodeNumber = (int)httpStatusCode,
                ResponseMessage = errorMessage,
                Exception = new ServiceResponseException
                {
                    ExceptionNumber = String.Empty,
                    CustomMessage = exception?.Message ?? errorMessage,
                    ExceptionType = exception?.GetType().Name,
                    StackTraceSummary = exception?.StackTrace
                },
                ValidationErrors = validationErrors
            };
        }
        public static ServiceResponse<T> ErrorResponse(HttpStatusCode httpStatusCode, string errorMessage, Exception exception, string exceptionNumber, Dictionary<string, string[]> validationErrors = null)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                HttpStatusCode = httpStatusCode,
                HttpStatusCodeNumber = (int)httpStatusCode,
                ResponseMessage = errorMessage,
                ExceptionNumber = exceptionNumber,
                Exception = new ServiceResponseException
                {
                    ExceptionNumber = String.Empty,
                    CustomMessage = exception?.Message ?? errorMessage,
                    ExceptionType = exception?.GetType().Name,
                    StackTraceSummary = exception?.StackTrace
                },
                ValidationErrors = validationErrors
            };
        }
    }
}
