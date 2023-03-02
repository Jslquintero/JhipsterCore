using System.Security.Authentication;

namespace JhipsterCore.Crosscutting.Exceptions;

public class UsernameNotFoundException : AuthenticationException
{
    public UsernameNotFoundException(string message) : base(message)
    {
    }
}
