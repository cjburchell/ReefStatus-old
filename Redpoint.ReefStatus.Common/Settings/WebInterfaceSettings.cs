// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebInterfaceSettings.cs" company="Redpoint">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Settings
{
    using System.Xml.Serialization;

    using Microsoft.Practices.Prism.Mvvm;

    /// <summary>
    /// The web interface settings.
    /// </summary>
    public class WebInterfaceSettings : BindableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebInterfaceSettings"/> class.
        /// </summary>
        public WebInterfaceSettings()
        {
            this.Enable = false;
            this.Port = 8081;
            this.Password = string.Empty;
            this.Protection = false;
        }

        /// <summary>
        /// The password.
        /// </summary>
        private string password;

        /// <summary>
        /// Gets or sets the web password.
        /// </summary>
        /// <value>The web password.</value>
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                this.OnPropertyChanged(() => this.Password);
            }
        }

        /// <summary>
        /// The protection.
        /// </summary>
        private bool protection;

        /// <summary>
        /// Gets or sets a value indicating whether [web protection].
        /// </summary>
        /// <value><c>true</c> if [web protection]; otherwise, <c>false</c>.</value>
        public bool Protection
        {
            get { return protection; }
            set
            {
                protection = value;
                this.OnPropertyChanged(() => this.Protection);
            }
        }

        /// <summary>
        /// The enable.
        /// </summary>
        private bool enable;

        /// <summary>
        /// Gets or sets a value indicating whether [web interface enable].
        /// </summary>
        /// <value><c>true</c> if [web interface enable]; otherwise, <c>false</c>.</value>
        public bool Enable
        {
            get { return enable; }
            set
            {
                enable = value;
                this.OnPropertyChanged(() => this.Enable);
            }
        }

        /// <summary>
        /// The port.
        /// </summary>
        private int port;

        /// <summary>
        /// Gets or sets the web interface port.
        /// </summary>
        /// <value>The web interface port.</value>
        public int Port
        {
            get { return port; }
            set
            {
                port = value;
                this.OnPropertyChanged(() => this.Port);
                this.OnPropertyChanged(() => this.Url);
            }
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [XmlIgnore]
        public string Url
        {
            get
            {
                return "http:\\\\localhost:" + this.Port;
            }
        }
    }
}
