using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using Common.ProfiLux;

    interface IInfoItem
    {
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item to display the data.</value>
        BaseInfo Item { get; }
    }
}
