// <copyright file="Stats.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>\

namespace RedPoint.ReefStatus.Common.Database
{
    using System;

    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// Data Stats
    /// </summary>
    public class Stats
    {
        public void ApplyConverter(BaseInfo baseInfo)
        {
            if (baseInfo is Probe)
            {
                var probe = (Probe)baseInfo;
                this.Max = probe.ConvertValue(this.Max);
                this.Min = probe.ConvertValue(this.Min);
                this.Average = probe.ConvertValue(this.Average);
                this.StdDeviation = probe.ConvertValue(this.StdDeviation);
            }
            else
            {
                this.Max = Math.Round(this.Max, 1);
                this.Min = Math.Round(this.Min, 1);
                this.Average = Math.Round(this.Average, 1);
                this.StdDeviation = Math.Round(this.StdDeviation, 1);
            }
        }

        /// <summary>
        /// Gets or sets the max.
        /// </summary>
        /// <value>The max.</value>
        public double Max { get; set; }

        /// <summary>
        /// Gets or sets the min.
        /// </summary>
        /// <value>The min.</value>
        public double Min { get; set; }

        /// <summary>
        /// Gets or sets the average.
        /// </summary>
        /// <value>The average.</value>
        public double Average { get; set; }

        /// <summary>
        /// Gets or sets the STD deviation.
        /// </summary>
        /// <value>The STD deviation.</value>
        public double StdDeviation { get; set; }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        public string Range { get; set; }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        public DateTime Date { get; set; }
    }
}
