namespace Banana.Auth.Api.Infrastructure.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        throw new NotImplementedException();
    }

    public bool Verify(string password, string hash)
    {
        throw new NotImplementedException();
    }
}
