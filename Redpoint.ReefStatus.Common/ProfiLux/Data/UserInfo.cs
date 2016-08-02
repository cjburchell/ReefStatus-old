// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserInfo.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    ///     User Info
    /// </summary>
    public class UserInfo : BaseInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserInfo" /> class.
        /// </summary>
        public UserInfo()
            : base("UserInfo")
        {
        }

        /// <summary>
        ///     Gets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public double Value { get; set; }

        /// <summary>
        ///     Gets OldDoubleValue.
        /// </summary>
        [JsonIgnore]
        public double? OldValue { get; set; }

        /// <summary>
        ///     Gets or sets the time.
        /// </summary>
        /// <value>The time.</value>
        public DateTime Time { get; set; }
    }
}