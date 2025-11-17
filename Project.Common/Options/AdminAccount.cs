namespace Project.Common.Options
{
    public class AdminAccount
    {
        public Account? Account { get; set; }
    }
    public class Account
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
