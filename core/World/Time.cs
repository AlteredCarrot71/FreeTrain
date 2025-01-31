#region LICENSE
/*
 * Copyright (C) 2007 - 2008 FreeTrain Team (http://freetrain.sourceforge.net)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
#endregion LICENSE

using System;
using System.Diagnostics;

using FreeTrain.Util;

namespace FreeTrain.World
{
    /// <summary>
    /// Time instant.
    /// </summary>
    [Serializable]
    public class Time
    {
        internal Time(long timeVal)
        {
            this.currentTime = timeVal;
        }


        /// <summary>
        /// Current time in minutes from 01/01/01 00:00am
        /// </summary>
        protected long currentTime;

        // well-defined time units.
        /// <summary>
        /// 
        /// </summary>
        public const long MINUTE_INITIAL = 1;
        /// <summary>
        /// 
        /// </summary>
        public const long HOUR_INITIAL = MINUTE_INITIAL * 60;
        /// <summary>
        /// 
        /// </summary>
        public const long DAY_INITIAL = HOUR_INITIAL * 24;
        /// <summary>
        /// 
        /// </summary>
        public const long YEAR_INITIAL = DAY_INITIAL * 365;
        // the initial time when a game starts
        /// <summary>
        /// 
        /// 
        /// </summary>
        public const long START_TIME = (31 + 28 + 31) * DAY_INITIAL + 8 * HOUR_INITIAL;



        /// <summary> Returns a string formatter for the display. </summary>
        public string displayString
        {
            get
            {
                return string.Format(Translation.GetString("CLOCK_FORMAT"),
                    year, month, day,
                    daysOfWeek[dayOfWeek],
                    hour, minutes / 10);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected static readonly int[] daysOfMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        /// <summary>
        /// Total minutes from the start of the game.
        /// </summary>
        public long totalMinutes { get { return currentTime - START_TIME; } }

        //
        // Get the current date/time
        //

        /// <summary>
        /// the current year. from 1.
        /// </summary>
        public int year { get { return (int)(currentTime / YEAR_INITIAL) + 1; } }
        /// <summary>
        /// the current month. from 1.
        /// </summary>
        public int month
        {
            get
            {
                long days = currentTime / DAY_INITIAL;
                days %= 365;	// 1 year = 365 days. No leap year.

                for (int i = 0; i < 12; i++)
                {
                    days -= daysOfMonth[i];
                    if (days < 0)
                        return i + 1;
                }
                Debug.Assert(false);
                return -1;
            }
        }
        /// <summary>
        /// the current day of the month. from 1.
        /// </summary>
        public int day
        {
            get
            {
                long days = currentTime / DAY_INITIAL;
                days %= 365;	// 1 year = 365 days. No leap year.

                for (int i = 0; i < 12; i++)
                {
                    if (days < daysOfMonth[i])
                        return (int)days + 1;
                    days -= daysOfMonth[i];
                }
                Debug.Assert(false);
                return -1;
            }
        }
        /// <summary>
        /// the current day of the week. from 0 to 6.
        /// </summary>
        public int dayOfWeek
        {
            get
            {
                long days = currentTime / DAY_INITIAL;
                return (int)days % 7;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int hour { get { return (int)((currentTime / HOUR_INITIAL) % 24); } }
        /// <summary>
        /// 
        /// </summary>
        public int minutes { get { return (int)((currentTime / MINUTE_INITIAL) % 60); } }
        /// <summary>
        /// 
        /// </summary>
        public DayNight dayOrNight
        {
            get
            {
                int h = hour;
                if (6 <= h && h < 18) return DayNight.DayTime;
                else return DayNight.Night;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Season season
        {
            get
            {
                int mon = month;
                return (Season)(((mon + 9 /*effectively -3*/) % 12) / 3);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool isWeekend
        {
            get
            {
                int dow = dayOfWeek;
                return dow == 0 || dow == 6;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected static readonly string[] daysOfWeek = {Translation.GetString("SUNDAY"),
                                                        Translation.GetString("MONDAY"),
                                                        Translation.GetString("TUESDAY"),
                                                        Translation.GetString("WEDNESDAY"),
                                                        Translation.GetString("THURSDAY"),
                                                        Translation.GetString("FRIDAY"),
                                                        Translation.GetString("SATURDAY")};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ta"></param>
        /// <param name="tb"></param>
        /// <returns></returns>
        public static TimeLength operator -(Time ta, Time tb)
        {
            return TimeLength.fromMinutes(ta.currentTime - tb.currentTime);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ta"></param>
        /// <param name="tb"></param>
        /// <returns></returns>
        public static Time operator +(Time ta, TimeLength tb)
        {
            return new Time(ta.currentTime + tb.totalMinutes);
        }
    }
}
