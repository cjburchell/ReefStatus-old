// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionSettings.cs" company="Redpoint Apps">
//   2010
// </copyright>
// <summary>
//   Defines the ConnectionType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Settings
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Connection Settings
    /// </summary>
    public class ConnectionSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether [network connection].
        /// </summary>
        /// <value><c>true</c> if [network connection]; otherwise, <c>false</c>.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public ConnectionType ConnectionType { get; set; } = ConnectionType.Network;

        /// <summary>
        /// Gets a value indicating whether this instance is network.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is network; otherwise, <c>false</c>.
        /// </value>
        public bool IsNetwork => this.ConnectionType != ConnectionType.Rs232;

        /// <summary>
        /// Gets a value indicating whether this instance is port.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is port; otherwise, <c>false</c>.
        /// </value>
        public bool IsPort => !this.IsNetwork;

        /// <summary>
        /// Gets or sets the baud rate.
        /// </summary>
        /// <value>The baud rate.</value>
        public int BaudRate { get; set; } = 9600;

        /// <summary>
        /// Gets or sets the time out for connection.
        /// </summary>
        /// <value>The time out.</value>
        public int Timeout { get; set; } = 5000;

        /// <summary>
        /// Gets or sets the Address.
        /// </summary>
        /// <value>The Address.</value>
        public string Address { get; set; } = "192.168.2.5";

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; } = 10001;

        /// <summary>
        /// Gets or sets the Controller address.
        /// </summary>
        /// <value>The Controller address.</value>
        public int ControllerAddress { get; set; } = 1;

        /// <summary>
        /// Gets or sets the COM port.
        /// </summary>
        /// <value>The COM port.</value>
        public int ComPort { get; set; } = 1;

        /// <summary>
        /// Gets or sets the pin.
        /// </summary>
        /// <value>The pin.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [get all].
        /// </summary>
        /// <value><c>true</c> if [get all]; otherwise, <c>false</c>.</value>
        public bool GetAll { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }
    }
}
