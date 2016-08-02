namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System.ComponentModel;

    /// <summary>
    /// The range of the graph
    /// </summary>
    public enum GraphRange
    {
        /// <summary>
        /// Show all the graph
        /// </summary>
        [Description("strAll")]
        All = 0, 

        /// <summary>
        /// Show data points from the last year
        /// </summary>
        [Description("strYear")]
        Year = 4, 

        /// <summary>
        /// Show data points from the last month
        /// </summary>
        [Description("strMonth")]
        Month = 3, 

        /// <summary>
        /// Show data points from the last day
        /// </summary>
        [Description("strDay")]
        Day = 1, 

        /// <summary>
        /// Show data points from the last week
        /// </summary>
        [Description("strWeek")]
        Week = 2
    }
}