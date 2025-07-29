namespace FelisBroker.Common.Extensions;

public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime dateTime) => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    
    public static DateTime FromUnixTimestamp(this long timestamp) => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp);
}