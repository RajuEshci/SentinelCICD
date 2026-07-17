using System.Net;

public class ServiceResponseException : Exception
{
    public HttpStatusCode HttpStatusCode { get; set; }
    public string ExceptionNumber { get; set; }
    public string CustomMessage { get; set; }

    // Replace with simple, serializable properties
    public string ExceptionType { get; set; }
    public string StackTraceSummary { get; set; }

    public ServiceResponseException()
    {
        this.ExceptionNumber = "system.exception";
        this.CustomMessage = base.Message;
        this.ExceptionType = this.GetType().Name;
        this.StackTraceSummary = this.StackTrace;
    }

    public ServiceResponseException(HttpStatusCode httpStatusCode, string message)
    {
        this.HttpStatusCode = httpStatusCode;
        this.CustomMessage = message;
        this.ExceptionType = this.GetType().Name;
        this.StackTraceSummary = this.StackTrace;
    }
}