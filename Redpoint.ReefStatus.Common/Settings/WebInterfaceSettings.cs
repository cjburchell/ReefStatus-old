// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebInterfaceSettings.cs" company="Redpoint">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Settings
{
    /// <summary>
    /// The web interface settings.
    /// </summary>
    public class WebInterfaceSettings
    {
        /// <summary>
        /// Gets or sets the web password.
        /// </summary>
        /// <value>The web password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [web protection].
        /// </summary>
        /// <value><c>true</c> if [web protection]; otherwise, <c>false</c>.</value>
        public bool Protection { get; set; }

        /// <summary>
        /// Gets or sets the web interface port.
        /// </summary>
        /// <value>The web interface port.</value>
        public int Port { get; set; } = 8081;
    }
}
