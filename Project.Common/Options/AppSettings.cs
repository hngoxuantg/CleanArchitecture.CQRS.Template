namespace Project.Common.Options
{
    public class AppSettings
    {
        public JwtConfig? JwtConfig { get; set; }

        public RateLimitConfig? RateLimitConfig { get; set; }
    }
}