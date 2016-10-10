using System;
using System.Text;

namespace Cnaws
{
    public sealed class Countdown
    {
        private int _day;
        private int _hour;
        private int _minute;
        private int _second;

        public Countdown()
        {
            _day = 0;
            _hour = 0;
            _minute = 0;
            _second = 0;
        }
        public Countdown(TimeSpan span)
            : this()
        {
            decimal seconds = new decimal(span.TotalSeconds);
            for (decimal i = 0; i < seconds; ++i)
            {
                if (++_second == 60)
                {
                    _second = 0;
                    if (++_minute == 60)
                    {
                        _minute = 0;
                        if (++_hour == 24)
                        {
                            _hour = 0;
                            ++_day;
                        }
                    }
                }
            }
        }

        public int Day
        {
            get { return _day; }
        }
        public int Hour
        {
            get { return _hour; }
        }
        public int Minute
        {
            get { return _minute; }
        }
        public int Second
        {
            get { return _second; }
        }

        public TimeSpan ToTimeSpan()
        {
            return new TimeSpan(Day, Hour, Minute, Second);
        }

        public static implicit operator Countdown(TimeSpan value)
        {
            return new Countdown(value);
        }
        public static implicit operator TimeSpan(Countdown value)
        {
            return value.ToTimeSpan();
        }

        public string FormatString(string format)
        {
            return ToString(format);
        }
        public override string ToString()
        {
            return string.Concat(Day, '天', Hour, '时', Minute, '分', Second, '秒');
        }
        public unsafe string ToString(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");
            StringBuilder sb = new StringBuilder();
            fixed (char* ptr = format)
            {
                char* end = ptr + format.Length;
                for (char* begin = ptr; begin != end; ++begin)
                {
                    switch (*begin)
                    {
                        case 'd':
                            sb.Append(Day);
                            break;
                        case 'h':
                            sb.Append(Hour);
                            break;
                        case 'm':
                            sb.Append(Minute);
                            break;
                        case 's':
                            sb.Append(Second);
                            break;
                        default:
                            sb.Append(*begin);
                            break;
                    }
                }
            }
            if (args != null)
                return string.Format(sb.ToString(), args);
            return sb.ToString();
        }
    }
}
