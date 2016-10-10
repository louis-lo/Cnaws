using Cnaws.Templates;
using System;
using System.Globalization;

namespace Cnaws.ExtensionMethods
{
    public enum DateDiffType
    {
        Year = 0,
        Month,
        Week,
        Day
    }

    public static class DateTimeExtensions
    {
        public static int ToTimestamp(this DateTime dt)
        {
            return (int)(dt - TimeZone.CurrentTimeZone.ToLocalTime((DateTime)Types.GetDefaultValue(TType<DateTime>.Type))).TotalSeconds;
        }
        public static DateTime ToDateTime(this int ts)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime((DateTime)Types.GetDefaultValue(TType<DateTime>.Type)).AddSeconds(ts);
        }
        public static int GetWeekOfYear(this DateTime dt, bool startWithMonday = false)
        {
            return (new GregorianCalendar()).GetWeekOfYear(dt, CalendarWeekRule.FirstDay, startWithMonday ? DayOfWeek.Monday : DayOfWeek.Sunday);
        }
        public static int DateDiff(this DateTime left, DateTime right, DateDiffType type = DateDiffType.Day)
        {
            if (type == DateDiffType.Year)
                return left.Year - right.Year;
            if (type == DateDiffType.Month)
                return ((left.Year - right.Year) * 12) + (left.Month - right.Month);
            if (left.Year == right.Year && type == DateDiffType.Week)
                return left.GetWeekOfYear() - right.GetWeekOfYear();
            int days = (int)Math.Floor((left - right).TotalDays);
            if (type == DateDiffType.Week)
                return (int)Math.Floor(days / 7.0);
            return days;
        }
    }
}
