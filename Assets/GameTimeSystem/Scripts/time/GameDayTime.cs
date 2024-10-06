using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace DenisAlipov.GameTime
{
    /// <summary>
    ///   <para>Representation of Day and Time values.</para>
    /// </summary>
    public struct GameDayTime
    {
        /// <summary>
        /// Shortcut for the start of time (0 days, 0 hours, 0 minutes)
        /// </summary>
        public static GameDayTime Zero = new GameDayTime(0, 0, 0);
        /// <summary>
        /// Shortcut for the infinity (max possible days value)
        /// </summary>
        public static GameDayTime Infinity = new GameDayTime(int.MaxValue, 0, 0);

        /// <summary>
        /// Number of days that has passed.
        /// </summary>
        public int Days;
        /// <summary>
        /// Number of hours that has passed in the current day. From 0 to 23
        /// </summary>
        public int Hours;
        /// <summary>
        /// Number of minutes that has passed in the current hour. From 0 to 59
        /// </summary>
        public int Minutes;
        
        /// <summary>
        /// Number of minutes that has passed since the 0:0:0
        /// </summary>
        public int TotalMinutes
        {
            get { return Minutes + Hours * 60 + Days * 1440; }
        }

        /// <summary>
        /// Number of minutes that has passed since the 0:0:0
        /// </summary>
        public GameDayTime(int totalMinutes)
        {
            Hours = 0;
            Minutes = 0;
            Days = 0;

            AddMinutes(totalMinutes);
        }

        public GameDayTime(int hrs, int mins)
        {
            Hours = 0;
            Minutes = 0;
            Days = 0;
            
            AddHours(hrs);
            AddMinutes(mins);
        }

        public GameDayTime(int days, int hrs, int mins)
        {
            Hours = 0;
            Minutes = 0;
            Days = days;
            
            AddHours(hrs);
            AddMinutes(mins);
        }

        /// <summary>
        /// Create DayTime from specified string in format dd:hh:mm
        /// string example: 04:19:30 - Day 4, 19h and 30m (7:30 pm)
        /// </summary>
        /// <param name="timeString"></param>
        public GameDayTime(string timeString)
        {
            Hours = 0;
            Minutes = 0;
            Days = 0;
            SetupFromString(timeString);
        }

        private void SetupFromString(string timeString)
        {
            var split_values = timeString.Split(':');

            if (split_values.Length == 3)
            {
                bool parseSuccess = true;
                parseSuccess &= Int32.TryParse(split_values[0], out Days);
                parseSuccess &= Int32.TryParse(split_values[1], out Hours);
                parseSuccess &= Int32.TryParse(split_values[2], out Minutes);
                if (parseSuccess)
                {
                    return;
                }
            }
            
            Debug.LogError("Time String is not in correct format! Expected format: dd:hh:mm");
        }

        /// <summary>
        /// Adds @daytime values to the current day time
        /// </summary>
        /// <param name="daytime"></param>
        public void Add(GameDayTime daytime)
        {
            Days += daytime.Days;

            AddHours(daytime.Hours);
            AddMinutes(daytime.Minutes);
        }

        /// <summary>
        /// Substract @daytime values from the current day time
        /// </summary>
        /// <param name="daytime"></param>
        public void Subtract(GameDayTime daytime)
        {
            int minutes = 0 - (daytime.Hours * 60 + daytime.Minutes + daytime.Days * 1440);
            AddMinutes(minutes);
        }

        /// <summary>
        /// Adds the number of @days. Can accept negative values.
        /// </summary>
        /// <param name="days"></param>
        public void AddDays(int days)
        {
            Days += days;
        }

        /// <summary>
        /// Adds the number of @hours. Can accept negative values.
        /// </summary>
        /// <param name="hours"></param>
        public void AddHours(int hours)
        {
            int totalHours = Hours + hours;

            if (totalHours < 0)
            {
                totalHours -= 24;
            }

            Days += totalHours / 24;

            int rem = totalHours % 24;
            if (totalHours < 0 && rem == 0)
            {
                Days += 1;
            }

            Hours = rem >= 0 ? rem : 24 + rem;
        }

        /// <summary>
        /// Adds the number of @mintues. Can accept negative values.
        /// </summary>
        /// <param name="minutes"></param>
        public void AddMinutes(int minutes)
        {
            int totalMins = Minutes + minutes;
            int addHrs = 0;

            if (totalMins < 0)
            {
                totalMins -= 60;
            }

            addHrs += totalMins / 60;

            AddHours(addHrs);

            int rem = totalMins % 60;
            if (totalMins < 0 && rem == 0)
            {
                AddHours(1);
            }

            Minutes = rem >= 0 ? rem : 60 + rem;
        }

        /// <summary>
        /// Checks if current DayTime is later than @daytime 
        /// </summary>
        /// <param name="daytime"></param>
        /// <returns>true if current DayTime is later than specified @daytime</returns>
        public bool IsLaterThan(GameDayTime daytime)
        {
            if (Days > daytime.Days)
            {
                return true;
            }

            return TotalMinutes >= daytime.TotalMinutes;
        }

        /// <summary>
        /// Checks if current's DayTime time is later than @daytime's time. Ignores Day value
        /// e.g. 16:30 is later than 10:40
        /// </summary>
        /// <param name="daytime"></param>
        /// <returns></returns>
        public bool IsTimeLaterThan(GameDayTime daytime)
        {
            int currentTime = Hours * 60 + Minutes;
            int otherTime = daytime.Hours * 60 + daytime.Minutes;

            return currentTime >= otherTime;
        }

        public override string ToString()
        {
            string hrs = Hours.ToString();
            string mins = Minutes.ToString();

            if (hrs.Length == 1)
            {
                hrs = string.Format("{0}{1}", "0", Hours);
            }

            if (mins.Length == 1)
            {
                mins = string.Format("{0}{1}", "0", mins);
            }

            return string.Format("{0}:{1}:{2}", Days, hrs, mins);
        }
        
        public override bool Equals(object obj)
        {
            if ((obj == null) || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                GameDayTime other = (GameDayTime) obj;
                return Days == other.Days && Hours == other.Hours && Minutes == other.Minutes;
            }
        }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}