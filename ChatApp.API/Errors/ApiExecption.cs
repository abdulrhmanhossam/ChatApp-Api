namespace ChatApp.API.Errors;

public class ApiExecption
{
    public int StatusCode { get; set; }
    public string Message { get; set;}
    public string Details { get; set; }

    public ApiExecption(int statusCode, string message, string details)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }
}
