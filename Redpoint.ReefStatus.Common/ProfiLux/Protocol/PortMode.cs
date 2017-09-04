namespace RedPoint.ReefStatus.Common.ProfiLux.Protocol
{
    using System;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using RedPoint.ReefStatus.Common.ProfiLux.Data;

    /// <summary>
    /// The Port Mode
    /// </summary>
    public class PortMode
    {
        /// <summary>
        /// Device Mode
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceMode DeviceMode { get; set; }

        /// <summary>
        /// Port number of the divice
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Blackout Time
        /// </summary>
        public int BlackOut { get; set; }

        /// <summary>
        /// If the port is inverted
        /// </summary>
        public bool Invert { get; set; }

        public string Id { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is probe.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is probe; otherwise, <c>false</c>.
        /// </value>
        public bool IsProbe => this.DeviceMode == DeviceMode.Decrease
                                  || this.DeviceMode == DeviceMode.Increase
                                  || this.DeviceMode == DeviceMode.Substrate
                                  || this.DeviceMode == DeviceMode.ProbeAlarm;


        /// <summary>
        ///     The get associated mode item.
        /// </summary>
        /// <param name="mode">
        ///     The input.
        /// </param>
        /// <param name="controller">
        ///     The items.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public static void UpdateAssociatedModeItem(PortMode mode, IController controller)
        {
            if (mode.IsProbe)
            {
                var probe = controller.Probes.FirstOrDefault(p => p.Index == mode.Port - 1);
                if (probe != null)
                {
                    mode.Id = probe.Id;
                }

                return;
            }

            switch (mode.DeviceMode)
            {
                case DeviceMode.Lights:
                    var light = controller.Lights.FirstOrDefault(item => item.Channel == mode.Port - 1);
                    if (light != null)
                    {
                        mode.Id = light.Id;
                    }

                    return;

                case DeviceMode.Timer:
                    var timer = controller.DosingPumps.FirstOrDefault(item => item.Channel == mode.Port - 1);
                    if (timer != null)
                    {
                        mode.Id = timer.Id;
                    }
                    return;

                case DeviceMode.Water:
                    var water = controller.LevelSensors.FirstOrDefault(item => item.Index == mode.Port - 1);
                    if (water != null)
                    {
                        mode.Id = water.Id;
                    }

                    return;

                case DeviceMode.CurrentPump:
                    var pump = controller.Pumps.FirstOrDefault(item => item.Index == mode.Port - 1);
                    if (pump != null)
                    {
                        mode.Id = pump.Id;
                    }
                    return;

                case DeviceMode.ProgrammableLogic:
                    var logic = controller.ProgrammableLogic.FirstOrDefault(item => item.Index == mode.Port - 1);
                    if (logic != null)
                    {
                        mode.Id = logic.Index.ToString();
                    }
                    return;
            }
        }
    }
}