// <copyright file="DeviceInfo.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using Newtonsoft.Json;

    using RedPoint.ReefStatus.Common.ProfiLux.Protocol;

    /// <summary>
    /// Divice Information
    /// </summary>
    public abstract class DeviceInfo : BaseInfo
    {
        protected DeviceInfo(string type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets or sets the port number.
        /// </summary>
        /// <value>The port number.</value>
        public int PortNumber { get; set; }

        /// <summary>
        /// Gets or sets the mode item.
        /// </summary>
        /// <value>
        /// The mode item.
        /// </value>
        public PortMode Mode { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is constant.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is constant; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool IsConstant => this.Mode.DeviceMode == DeviceMode.AlwaysOn
                                           || this.Mode.DeviceMode == DeviceMode.AlwaysOff;
    }
}