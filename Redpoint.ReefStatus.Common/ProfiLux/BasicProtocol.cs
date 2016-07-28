// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicProtocol.cs" company="Redpoint Apps">
//   2009
// </copyright>
// <summary>
//   Defines the BasicProtocol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Basic Protocol class
    /// </summary>
    public abstract class BasicProtocol
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public int Version { get; protected set; }

        /// <summary>
        /// Converts to date.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The converted date</returns>
        public static DateTime ConvertToDate(int value)
        {
            string timeString = value.ToString(CultureInfo.CurrentCulture);

            DateTime result;
            if (!DateTimeTryParseExact(timeString, new[] { "ddMMyyyy", "dMMyyyy", "ddMMyy", "dMMyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                try
                {
                    string yearValue = timeString.Substring(timeString.Length - 2, 2);
                    timeString = timeString.Substring(0, timeString.Length - 2);
                    string monthValue = timeString.Substring(timeString.Length - 2, 2);
                    timeString = timeString.Substring(0, timeString.Length - 2);
                    string dateValue = timeString;
                    result = new DateTime(int.Parse(yearValue) + 2000, int.Parse(monthValue), int.Parse(dateValue));
                }
                catch (ArgumentOutOfRangeException)
                {
                    timeString = value.ToString(CultureInfo.CurrentCulture);
                    string yearValue = timeString.Substring(timeString.Length - 3, 3);
                    timeString = timeString.Substring(0, timeString.Length - 3);
                    string monthValue = timeString.Substring(timeString.Length - 2, 2);
                    timeString = timeString.Substring(0, timeString.Length - 2);
                    string dateValue = timeString;

                    result = new DateTime(int.Parse(yearValue) + 2000, int.Parse(monthValue), int.Parse(dateValue));
                }
               
            }

            return result;
        }

        /// <summary>
        /// Dates the time try parse exact.
        /// </summary>
        /// <param name="timeString">The time string.</param>
        /// <param name="p">The p.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <param name="dateTimeStyles">The date time styles.</param>
        /// <param name="result">The result.</param>
        /// <returns>True if the format is valid</returns>
        private static bool DateTimeTryParseExact(string timeString, string[] p, CultureInfo cultureInfo, DateTimeStyles dateTimeStyles, out DateTime result)
        {
            return DateTime.TryParseExact(timeString, p, cultureInfo, dateTimeStyles, out result);
        }
    }
}
