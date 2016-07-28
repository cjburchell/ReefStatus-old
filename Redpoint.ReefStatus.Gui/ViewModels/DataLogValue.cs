using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RedPoint.ReefStatus.Common.ProfiLux;

namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using System.Collections;

    public class DataLogValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataLogValue"/> class.
        /// </summary>
        /// <param name="sensors">The sensors.</param>
        /// <param name="devices">The devices.</param>
        /// <param name="now">The now.</param>
        public DataLogValue(IEnumerable sensors, IEnumerable devices, DateTime now)
        {
            Items = new ObservableCollection<object> {now};

            foreach(SensorInfo sensor in sensors)
            {
                Items.Add(sensor.Value);
            }

            foreach (DeviceInfo device in devices)
            {
                Items.Add(device.Value);
            }
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public ObservableCollection<object> Items {get; private set;}
    }
}