namespace Project.Common.Options
{
    public class AppSettings
    {
        public JwtConfig? JwtConfig { get; set; }
    }
    public class JwtConfig
    {
        public string? Secret { get; set; }
        public string? ValidAudience { get; set; }
        public string? ValidIssuer { get; set; }
        public int TokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
    }
}