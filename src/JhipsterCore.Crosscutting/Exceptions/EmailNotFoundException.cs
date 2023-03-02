using JhipsterCore.Crosscutting.Constants;

namespace JhipsterCore.Crosscutting.Exceptions;

public class EmailNotFoundException : BaseException
{
    public EmailNotFoundException() : base(ErrorConstants.EmailNotFoundType, "Email address not registered")
    {
    }
}
