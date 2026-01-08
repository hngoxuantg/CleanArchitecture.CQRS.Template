namespace Project.Common.Options
{
    public class EmailSettings
    {
        public string? SmtpHost { get; set; }

        public int SmtpPort { get; set; }

        public string? SmtpUser { get; set; }

        public string? Password { get; set; }

        public string? From { get; set; }

        public string? DisplayName { get; set; }
    }
}
