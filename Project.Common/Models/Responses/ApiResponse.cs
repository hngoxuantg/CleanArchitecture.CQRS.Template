namespace Project.Common.Models.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public ApiResponse() { }
        public ApiResponse(bool success, string? message, T? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public ApiResponse() { }
        public ApiResponse(bool success, string? message)
        {
            Success = success;
            Message = message;
        }
    }
    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public AuthResult() { }
        public AuthResult(bool success, string? message, string? accessToken, string? refreshToken)
        {
            Success = success;
            Message = message;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
