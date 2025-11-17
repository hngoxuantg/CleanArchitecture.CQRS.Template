using System.Net;

namespace Project.Application.Exceptions
{
    public class NotFoundException : BaseCustomException
    {
        public override HttpStatusCode HttpStatusCode => HttpStatusCode.NotFound;
        public override string ErrorCode => "NOT_FOUND";

        public NotFoundException() { }
        public NotFoundException(string? message) : base(message) { }
        public NotFoundException(string message, Exception? innerException) : base(message, innerException) { }
    }
}