using System.Net;

namespace Project.Application.Exceptions
{
    public class ForbiddenAccessException : BaseCustomException
    {
        public override HttpStatusCode HttpStatusCode => HttpStatusCode.Forbidden;
        public override string ErrorCode => "FORBIDDEN_ACCESS";

        public ForbiddenAccessException() { }
        public ForbiddenAccessException(string? message) : base(message) { }
        public ForbiddenAccessException(string message, Exception? innerException) : base(message, innerException) { }
    }
}
