// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomGraphSettings.cs" company="Redpoint">
//   2009
// </copyright>
// <summary>
//   Custom Graph Settings
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Settings
{
    using System.Collections.ObjectModel;
    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// Custom Graph Settings
    /// </summary>
    public class CustomGraphSettings
    {
        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        public GraphRange Range { get; set; } = GraphRange.Week;

        /// <summary>
        /// Gets or sets the show curves.
        /// </summary>
        /// <value>The show curves.</value>
        public Collection<string> ShowCurves { get; set; } = new Collection<string>();
    }
}
