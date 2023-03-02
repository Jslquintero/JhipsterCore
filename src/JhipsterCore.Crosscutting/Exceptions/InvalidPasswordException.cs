using JhipsterCore.Crosscutting.Constants;

namespace JhipsterCore.Crosscutting.Exceptions;

public class InvalidPasswordException : BaseException
{
    public InvalidPasswordException() : base(ErrorConstants.InvalidPasswordType, "Incorrect Password")
    {
        //Status = StatusCodes.Status400BadRequest
    }
}
