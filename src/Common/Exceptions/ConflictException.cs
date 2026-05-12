namespace Banana.Auth.Api.Common.Exceptions;

public class ConflictException : AppException
{
    public ConflictException(string message = "Resource already exists")
        : base(message, 409)
    {
    }
}
