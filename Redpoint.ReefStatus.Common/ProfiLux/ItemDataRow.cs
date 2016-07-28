// <copyright file="ItemDataRow.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Data Row item from the data plot
    /// </summary>
    [Serializable]
    public class ItemDataRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDataRow"/> class.
        /// </summary>
        public ItemDataRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDataRow"/> class.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="values">The values.</param>
        public ItemDataRow(DateTime time, Collection<Item> values)
        {
            this.Time = time;
            this.Values = values;
        }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The time.</value>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public Collection<Item> Values { get; private set; }

/*
        /// <summary>
        /// Gets the data point.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>The data point for the column</returns>
        private DataPoint GetDataPoint(int column)
        {
            return new DataPoint(Time, Values[column].Value, 0);
        }
*/
        #region Nested type: Item

        /// <summary>
        /// The item.
        /// </summary>
        public class Item
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Item"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="id">The id.</param>
            public Item(double value, string id)
            {
                this.Value = value;
                this.Id = id;
            }

            /// <summary>
            /// Gets Value.
            /// </summary>
            public double Value { get; private set; }

            /// <summary>
            /// Gets ID.
            /// </summary>
            public string Id { get; private set; }
        }

        #endregion
    }
}