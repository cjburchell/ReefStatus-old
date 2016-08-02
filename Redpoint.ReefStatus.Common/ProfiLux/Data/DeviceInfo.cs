// <copyright file="DeviceInfo.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

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
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceMode DeviceMode { get; set; }

        /// <summary>
        /// Gets or sets the port number.
        /// </summary>
        /// <value>The port number.</value>
        public int PortNumber { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the blackout.
        /// </summary>
        /// <value>The blackout.</value>
        public int Blackout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DeviceInfo"/> is invert.
        /// </summary>
        /// <value><c>true</c> if invert; otherwise, <c>false</c>.</value>
        public bool Invert { get; set; }

        /// <summary>
        /// Gets or sets the mode item.
        /// </summary>
        /// <value>
        /// The mode item.
        /// </value>
        [JsonIgnore]
        public object ModeItem { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is constant.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is constant; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool IsConstant => this.DeviceMode == DeviceMode.AlwaysOn
                                           || this.DeviceMode == DeviceMode.AlwaysOff;

        /// <summary>
        /// Gets a value indicating whether this instance is use port.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is use port; otherwise, <c>false</c>.
        /// </value>
        protected bool IsUsePort => this.DeviceMode == DeviceMode.Lights
                                    || this.DeviceMode == DeviceMode.Timer
                                    || this.DeviceMode == DeviceMode.Water
                                    || this.DeviceMode == DeviceMode.ProgrammableLogic
                                    || this.DeviceMode == DeviceMode.CurrentPump
                                    || this.DeviceMode == DeviceMode.VariableIllumination;

        /// <summary>
        /// Gets a value indicating whether this instance is probe.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is probe; otherwise, <c>false</c>.
        /// </value>
        protected bool IsProbe => this.DeviceMode == DeviceMode.Decrease
                                  || this.DeviceMode == DeviceMode.Increase
                                  || this.DeviceMode == DeviceMode.Substrate
                                  || this.DeviceMode == DeviceMode.ProbeAlarm;

        /// <summary>
        /// Gets the associated mode item.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="logics"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object GetAssociatedModeItem(Controller controller)
        {
            if (this.IsProbe)
            {
                return controller.Probes.FirstOrDefault(p => p.Index == (this.Port - 1));
            }

            switch (this.DeviceMode)
            {
                case DeviceMode.Lights:
                    return controller.Lights.FirstOrDefault(item => item.Channel == (this.Port - 1));
                case DeviceMode.Timer:
                    return controller.DosingPumps.FirstOrDefault(item => item.Channel == (this.Port - 1));
                case DeviceMode.Water:
                    return controller.LevelSensors.FirstOrDefault(item => item.Index == (this.Port - 1));
                case DeviceMode.CurrentPump:
                    return controller.Pumps.FirstOrDefault(item => item.Index == (this.Port - 1));
                case DeviceMode.ProgrammableLogic:
                    return controller.ProgrammableLogic.FirstOrDefault(item => item.Index == (this.Port - 1));
            }

            return null;
        }
    }
}