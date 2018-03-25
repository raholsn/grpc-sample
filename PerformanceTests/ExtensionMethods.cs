using System;

namespace PerformanceTests
{
    public static class ExtensionMethods
    {
        public static string ToElapsedTime(this TimeSpan ts)
        {
            var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";

            return elapsedTime;
        }
    }
}
