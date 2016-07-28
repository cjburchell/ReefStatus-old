// <copyright file="DataPoint.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Data Point
    /// </summary>
    public class DataPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataPoint"/> class.
        /// </summary>
        public DataPoint()
        {
            // used in serilization
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPoint"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="time">The time.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        public DataPoint(string id, DateTime time, double value, int index)
        {
            this.Time = time;
            this.Value = value;
            this.Index = index;
            this.Id = id;
        }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The time.</value>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the time string.
        /// </summary>
        /// <value>The time string.</value>
        public string TimeString
        {
            get
            {
                return this.Time.ToString("G", CultureInfo.CurrentCulture);
            }

// ReSharper disable ValueParameterNotUsed
            set
            {
                // used in serilization
            }

// ReSharper restore ValueParameterNotUsed
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public string Id { get; set; }
    }
}