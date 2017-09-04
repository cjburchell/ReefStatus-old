// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingSettings.cs" company="Redpoint Apps">
//   2010
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Settings
{
    using RedPoint.ReefStatus.Common.Database;

    /// <summary>
    /// The logging settings.
    /// </summary>
    public class LoggingSettings : CouchDocument
    {
        /// <summary>
        /// Gets or sets the log interval.
        /// </summary>
        /// <value>The log interval.</value>
        public int LogInterval { get; set; } = 5;
    }
}