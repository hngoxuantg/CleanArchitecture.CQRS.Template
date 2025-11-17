using System.Net;

namespace Project.Application.Exceptions
{
    public class ValidatorException : BaseCustomException
    {
        public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
        public override string ErrorCode => "VALIDATION_ERROR";
        public Dictionary<string, List<string>> ValidationErrors { get; } = new Dictionary<string, List<string>>();

        public ValidatorException() { }
        public ValidatorException(string? message) : base(message) { }
        public ValidatorException(string? message, Exception? innerException) : base(message, innerException) { }
        public ValidatorException(string fieldName, string errorMessage) : base("Validation failed")
        {
            ValidationErrors = new Dictionary<string, List<string>>
            {
                { fieldName.ToLower(), new List<string> { errorMessage } }
            };
        }
        public ValidatorException(Dictionary<string, List<string>> validationErrors) : base("errors")
        {
            ValidationErrors = validationErrors;
        }
        public ValidatorException(string fieldName, List<string> errorMessages) : base("Validation failed")
        {
            ValidationErrors = new Dictionary<string, List<string>>
            {
                { fieldName.ToLower(), errorMessages }
            };
        }
    }
}