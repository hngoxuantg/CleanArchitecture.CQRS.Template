using System.Net;

namespace Project.Application.Exceptions
{
    public class BusinessRuleException : BaseCustomException
    {
        public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
        public override string ErrorCode => "BUSINESS_RULE_VIOLATION";

        public BusinessRuleException() { }
        public BusinessRuleException(string message) : base(message) { }
        public BusinessRuleException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}