namespace Project.Common.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime ToVietnamTime(this DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime,
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        }
    }
}
