// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgramableLogic.cs" company="Redpoint">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common.UI.ViewModel;

    /// <summary>
    ///     The programmable logic.
    /// </summary>
    public class ProgramableLogic : BindableBase
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
        public BindableBase Input2Item { get; set; }

        /// <summary>
        ///     Gets or sets the input 1 item.
        /// </summary>
        public BindableBase Input1Item { get; set; }

        /// <summary>
        /// The get associated mode item.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="logics">
        /// The logics.
        /// </param>
        /// <returns>
        /// The <see cref="BindableBase"/>.
        /// </returns>
        protected static BindableBase GetAssociatedModeItem(
            PortMode input, 
            IEnumerable<BaseInfo> items, 
            IEnumerable<ProgramableLogic> logics = null)
        {
            if (((input.DeviceMode == DeviceMode.Decrease) || (input.DeviceMode == DeviceMode.Increase))
                || ((input.DeviceMode == DeviceMode.Substrate) || (input.DeviceMode == DeviceMode.ProbeAlarm)))
            {
                return items.OfType<Probe>().FirstOrDefault(p => p.Index == (input.Port - 1));
            }

            switch (input.DeviceMode)
            {
                case DeviceMode.Lights:
                    return items.OfType<Light>().FirstOrDefault(item => item.Channel == (input.Port - 1));

                case DeviceMode.Timer:
                    return items.OfType<DosingPump>().FirstOrDefault(item => item.Channel == (input.Port - 1));

                case DeviceMode.Water:
                    return items.OfType<LevelSensor>().FirstOrDefault(item => item.Index == (input.Port - 1));

                case DeviceMode.CurrentPump:
                    return items.OfType<CurrentPump>().FirstOrDefault(item => item.Index == (input.Port - 1));

                case DeviceMode.ProgrammableLogic:
                    if (logics == null)
                    {
                        break;
                    }

                    return logics.FirstOrDefault(item => item.Index == (input.Port - 1));
            }

            return null;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="logics">
        /// The logics.
        /// </param>
        public void Update(SafeObservableCollection<BaseInfo> items, SafeObservableCollection<ProgramableLogic> logics)
        {
            this.Input1Item = GetAssociatedModeItem(this.Input1, items, logics);
            this.Input2Item = GetAssociatedModeItem(this.Input2, items, logics);
        }
    }
}