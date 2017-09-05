using System;
using System.Collections.Generic;
using System.Linq;

namespace RedPoint.ReefStatus.Common.Database
{
    public static class StatsUtils
    {
        /// <summary>
        /// Gets the stats.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="points">The points.</param>
        /// <param name="getStdDev">if set to <c>true</c> [get STD dev].</param>
        /// <returns>The status</returns>
        public static Stats GetStats(DateTime endTime, DateTime startTime, IEnumerable<DataLog> points, bool getStdDev = false)
        {
            var dataPoints = points as IList<DataLog> ?? points.ToList();
            return new Stats
                       {
                           Max = MaxDataPoint(endTime, startTime, dataPoints),
                           Min = MinDataPoint(endTime, startTime, dataPoints),
                           Average = AverageDataPoint(endTime, startTime, dataPoints),
                           StdDeviation = getStdDev ? StdDevDataPoint(endTime, startTime, dataPoints) : 0
                       };
        }

        /// <summary>
        /// Averages the data point.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="points">The points.</param>
        /// <returns>the average of a data point</returns>
        private static double AverageDataPoint(DateTime endTime, DateTime startTime, IEnumerable<DataLog> points)
        {
            try
            {
                var average = (from item in points
                               where item.Time <= endTime && item.Time > startTime
                               select (double?)item.Value).Average();

                return average ?? 0;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(119, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Maxes the data point.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        private static double MaxDataPoint(DateTime endTime, DateTime startTime, IEnumerable<DataLog> points)
        {
            try
            {
                var max = (from item in points where item.Time <= endTime && item.Time > startTime select (double?)item.Value).Max();
                return max ?? 0;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(121, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Mins the data point.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        private static double MinDataPoint(DateTime endTime, DateTime startTime, IEnumerable<DataLog> points)
        {
            try
            {
                var min = (from item in points
                           where item.Time <= endTime && item.Time > startTime
                           select (double?)item.Value).Min();

                return min.HasValue ? min.Value : 0;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(120, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// STDs the dev data point.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        private static double StdDevDataPoint(DateTime endTime, DateTime startTime, IEnumerable<DataLog> points)
        {
            try
            {
                return CalculateStdDev(
                    from item in points
                    where item.Time <= endTime && item.Time > startTime
                    select item.Value);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(118, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Calculates the STD dev.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>the standard deviation of the values</returns>
        private static double CalculateStdDev(IEnumerable<double> values)
        {
            var enumerable = values as IList<double> ?? values.ToList();
            var count = enumerable.Count;
            if (count <= 1)
            {
                return 0;
            }

            // Compute the Average
            var avg = enumerable.Average();

            // Perform the Sum of (value-avg)^2
            var sum = enumerable.Sum(d => Math.Pow(d - avg, 2));

            // Put it all together
            return Math.Sqrt(sum / ((double)count - 1));
        }
    }
}
