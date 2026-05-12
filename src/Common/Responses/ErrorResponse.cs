namespace Banana.Auth.Api.Common.Responses;

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}
