using System;

namespace BumpDetector.Shared
{
    public static class Extensions
    {
        public static double ToMiliSecondsSince1970(this DateTime dateTime)
        {
            return Math.Round (
                dateTime.ToUniversalTime()
                .Subtract (
                    new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds);
        }
    }
}

