using System;

namespace foundation
{
    public class DateUtils
    {
        private static readonly uint[] _daysInMonth = new uint[] {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
 
        /// <summary>
        /// 一分种
        /// </summary>
        public const int ONE_MINUTE=60000;
        /// <summary>
        ///  一小时;
        /// </summary>
        public const int ONE_HOURS =3600000;

        /// <summary>
        /// 半小时
        /// </summary>
        public const int HALF_HOUR=1800000;

        /// <summary>
        ///  一天
        /// </summary>
        public const int ONE_DAY=86400000;

        public static bool isLeapYear(DateTime d)
        {
            return (d.Year%4 == 0) ? true : false;
        }

        public static uint daysInMonth(DateTime d)
        {
            uint i = 0;
            if (isLeapYear(d) && d.Month == 1)
            {
                i = 1;
            }

            i += _daysInMonth[d.Month];
            return i;
        }


        protected static string addZero(int num, int len)
        {
            string str = num.ToString();
            while (str.Length < len)
            {
                str = "0" + str;
            }

            return str;
        }

        /// <summary>
        /// 比较日期
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static int compareDates(DateTime d1, DateTime d2)
        {
            long d1ms = d1.Ticks;
            long d2ms = d2.Ticks;

            if (d1ms > d2ms)
            {
                return -1;
            }
            else if (d1ms < d2ms)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        public static long getDayStartTimeByTime(long now)
        {
            DateTime sharedDate = new DateTime(now);

            DateTime temp = new DateTime(sharedDate.Year, sharedDate.Month, sharedDate.Day, 0, 0, 0);

            return temp.Ticks;
        }

        static public long getDayEndTimeByDate(long now)
		{
            DateTime sharedDate = new DateTime(now);

            DateTime temp = new DateTime(sharedDate.Year, sharedDate.Month, sharedDate.Day, 23,59, 59);

            return temp.Ticks;
        }

        public static int getDayTime(long now)
        {
            DateTime sharedDate = new DateTime(now);
            return sharedDate.Hour*ONE_HOURS + sharedDate.Minute*ONE_MINUTE + sharedDate.Second*1000;
        }
    }
}