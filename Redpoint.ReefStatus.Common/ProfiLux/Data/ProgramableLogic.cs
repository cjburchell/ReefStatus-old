// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgramableLogic.cs" company="Redpoint">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using System.Linq;

    /// <summary>
    ///     The programmable logic.
    /// </summary>
    public class ProgramableLogic
    {
        /// <summary>
        ///     Gets or sets the input 1.
        /// </summary>
        public PortMode Input1 { get; set; }

        /// <summary>
        ///     Gets or sets the input 2.
        /// </summary>
        public PortMode Input2 { get; set; }

        /// <summary>
        ///     Gets or sets the function.
        /// </summary>
        public LogicFunction Function { get; set; }

        /// <summary>
        ///     Gets or sets the index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///     Gets or sets the input 1 text.
        /// </summary>
        public string Input1Text { get; set; }

        /// <summary>
        ///     Gets or sets the input 2 text.
        /// </summary>
        public string Input2Text { get; set; }

        /// <summary>
        ///     Gets or sets the input 2 item.
        /// </summary>
        public object Input2Item { get; set; }

        /// <summary>
        ///     Gets or sets the input 1 item.
        /// </summary>
        public object Input1Item { get; set; }

        /// <summary>
        ///     The get associated mode item.
        /// </summary>
        /// <param name="input">
        ///     The input.
        /// </param>
        /// <param name="items">
        ///     The items.
        /// </param>
        /// <param name="logics">
        ///     The logics.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        protected static object GetAssociatedModeItem(PortMode input, Controller controller)
        {
            if ((input.DeviceMode == DeviceMode.Decrease) || (input.DeviceMode == DeviceMode.Increase) || (input.DeviceMode == DeviceMode.Substrate) || (input.DeviceMode == DeviceMode.ProbeAlarm))
            {
                return controller.Probes.FirstOrDefault(p => p.Index == input.Port - 1);
            }

            switch (input.DeviceMode)
            {
                case DeviceMode.Lights:
                    return controller.Lights.FirstOrDefault(item => item.Channel == input.Port - 1);

                case DeviceMode.Timer:
                    return controller.DosingPumps.FirstOrDefault(item => item.Channel == input.Port - 1);

                case DeviceMode.Water:
                    return controller.LevelSensors.FirstOrDefault(item => item.Index == input.Port - 1);

                case DeviceMode.CurrentPump:
                    return controller.Pumps.FirstOrDefault(item => item.Index == input.Port - 1);

                case DeviceMode.ProgrammableLogic:
                    return controller.ProgrammableLogic.FirstOrDefault(item => item.Index == input.Port - 1);
            }

            return null;
        }

        /// <summary>
        ///     The update.
        /// </summary>
        /// <param name="items">
        ///     The items.
        /// </param>
        /// <param name="logics">
        ///     The logics.
        /// </param>
        public void Update(Controller controller)
        {
            this.Input1Item = GetAssociatedModeItem(this.Input1, controller);
            this.Input2Item = GetAssociatedModeItem(this.Input2, controller);
        }
    }
}