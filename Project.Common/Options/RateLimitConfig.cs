namespace Project.Common.Options
{
    public class RateLimitConfig
    {
        public PolicyOptions? AuthenticatedUser { get; set; }

        public PolicyOptions? AnonymousUser { get; set; }

        public PolicyOptions? LoginAttempts { get; set; }
    }
    public class PolicyOptions
    {
        public int PermitLimit { get; set; }

        public int WindowMinutes { get; set; }
    }
}
