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
    using System;
    using System.Globalization;

    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common.UI.ViewModel;

    /// <summary>
    /// The connection Type
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// Network Connection Type
        /// </summary>
        Network,

        /// <summary>
        /// RS232 Connection Type
        /// </summary>
        RS232
    }

    /// <summary>
    /// Connection Settings
    /// </summary>
    public class ConnectionSettings : BindableBase
    {
        /// <summary>
        /// the type of connection
        /// </summary>
        private ConnectionType connectionType;

        /// <summary>
        /// the baud rate of the port
        /// </summary>
        private int baudRate;

        /// <summary>
        /// connection timeout
        /// </summary>
        private int timeout;

        /// <summary>
        /// Address to connect to
        /// </summary>
        private string address;

        /// <summary>
        /// network port
        /// </summary>
        private int port;

        /// <summary>
        /// controller Address
        /// </summary>
        private int controllerAddress;

        /// <summary>
        /// com Port
        /// </summary>
        private int comPort;

        /// <summary>
        /// connection password
        /// </summary>
        private string password;

        /// <summary>
        /// Get all the data
        /// </summary>
        private bool getAll;

        /// <summary>
        /// User Name
        /// </summary>
        private string userName;

        /// <summary>
        /// Web Server Port
        /// </summary>
        private int webServerPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionSettings"/> class.
        /// </summary>
        public ConnectionSettings()
        {
            this.Address = "192.168.2.5";
            this.Port = 10001;
            this.ComPort = 1;
            this.ControllerAddress = 1;
            this.Password = string.Empty;
            this.BaudRate = 9600;
            this.Timeout = 5000;
            this.ConnectionType = ConnectionType.Network;
            this.WebServerPort = 80;
        }

        /// <summary>
        /// Gets or sets the web server port.
        /// </summary>
        /// <value>The web server port.</value>
        public int WebServerPort
        {
            get
            {
                return this.webServerPort;
            }

            set
            {
                if (this.webServerPort != value)
                {
                    this.webServerPort = value;
                    this.OnPropertyChanged(() => this.WebServerPort);
                    this.OnPropertyChanged(() => this.WebAddress);
                }
            }
        }

        /// <summary>
        /// Gets the web address.
        /// </summary>
        /// <value>The web address.</value>
        public string WebAddress
        {
            get
            {
                UriBuilder uri = new UriBuilder("http", this.Address, this.WebServerPort);
                return uri.Uri.ToString();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [network connection].
        /// </summary>
        /// <value><c>true</c> if [network connection]; otherwise, <c>false</c>.</value>
        public ConnectionType ConnectionType
        {
            get
            {
                return this.connectionType;
            }

            set
            {
                this.connectionType = value;
                this.OnPropertyChanged(() => this.ConnectionType);
                this.OnPropertyChanged(() => this.IsNetwork);
                this.OnPropertyChanged(() => this.IsPort);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is network.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is network; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnore]
        public bool IsNetwork
        {
            get
            {
                return ConnectionType != Settings.ConnectionType.RS232;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is port.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is port; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnore]
        public bool IsPort
        {
            get
            {
                return !this.IsNetwork;
            }
        }

        /// <summary>
        /// Gets or sets the baud rate.
        /// </summary>
        /// <value>The baud rate.</value>
        public int BaudRate
        {
            get
            {
                return this.baudRate;
            }

            set
            {
                this.baudRate = value;
                this.OnPropertyChanged(() => this.BaudRate);
            }
        }

        /// <summary>
        /// Gets or sets the time out for connection.
        /// </summary>
        /// <value>The time out.</value>
        public int Timeout
        {
            get
            {
                return this.timeout;
            }

            set
            {
                this.timeout = value;
                this.OnPropertyChanged(() => this.Timeout);
            }
        }

        /// <summary>
        /// Gets or sets the Address.
        /// </summary>
        /// <value>The Address.</value>
        public string Address
        {
            get
            {
                return this.address;
            }

            set
            {
                this.address = value;
                this.OnPropertyChanged(() => this.Address);
                this.OnPropertyChanged(() => this.NetworkConntectionText);
                this.OnPropertyChanged(() => this.WebAddress);
            }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port
        {
            get
            {
                return this.port;
            }

            set
            {
                this.port = value;
                this.OnPropertyChanged(() => this.Port);
                this.OnPropertyChanged(() => this.NetworkConntectionText);
            }
        }

        /// <summary>
        /// Gets or sets the Controller address.
        /// </summary>
        /// <value>The Controller address.</value>
        public int ControllerAddress
        {
            get
            {
                return this.controllerAddress;
            }

            set
            {
                this.controllerAddress = value;
                this.OnPropertyChanged(() => this.ControllerAddress);
            }
        }

        /// <summary>
        /// Gets or sets the COM port.
        /// </summary>
        /// <value>The COM port.</value>
        public int ComPort
        {
            get
            {
                return this.comPort;
            }

            set
            {
                this.comPort = value;
                this.OnPropertyChanged(() => this.ComPort);
                this.OnPropertyChanged(() => this.NetworkConntectionText);
            }
        }

        /// <summary>
        /// Gets or sets the pin.
        /// </summary>
        /// <value>The pin.</value>
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                this.password = value;
                this.OnPropertyChanged(() => this.Password);
            }
        }

        /// <summary>
        /// Gets the network conntection text.
        /// </summary>
        /// <value>The network conntection text.</value>
        public string NetworkConntectionText
        {
            get
            {
                if (ConnectionType != ConnectionType.RS232)
                {
                    return string.Format(CultureInfo.CurrentCulture, "{0}:{1}", this.Address, this.Port);
                }

                return string.Format(CultureInfo.CurrentCulture, "COM{0}", this.ComPort);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [get all].
        /// </summary>
        /// <value><c>true</c> if [get all]; otherwise, <c>false</c>.</value>
        public bool GetAll
        {
            get
            {
                return this.getAll;
            }

            set
            {
                if (this.getAll != value)
                {
                    this.getAll = value;
                    this.OnPropertyChanged(() => this.GetAll);
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName
        {
            get
            {
                return this.userName;
            }

            set
            {
                if (this.userName != value)
                {
                    this.userName = value;
                    this.OnPropertyChanged(() => this.UserName);
                }
            }
        }
    }
}
