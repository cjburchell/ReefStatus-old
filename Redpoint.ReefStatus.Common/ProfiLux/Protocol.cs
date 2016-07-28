// <copyright file="Protocol.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;

    using RedPoint.ReefStatus.Common.Communication;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.UI;

    /// <summary>
    /// State of sensor or socket
    /// </summary>
    public enum CurrentState
    {
        /// <summary>
        /// it is off
        /// </summary>
        [Description("strOff")]
        Off = 0,

        /// <summary>
        /// it is on
        /// </summary>
        [Description("strOn")]
        On = 1
    }

    /// <summary>
    /// Faceplate key commands
    /// </summary>
    public enum FaceplateKey
    {
        /// <summary>
        /// No Key??
        /// </summary>
        None = 0,

        /// <summary>
        /// Left Key
        /// </summary>
        Left = 1,

        /// <summary>
        /// Right Key
        /// </summary>
        Right = 2,

        /// <summary>
        /// Up Key
        /// </summary>
        Up = 3,

        /// <summary>
        /// Down Key
        /// </summary>
        Down = 4,

        /// <summary>
        /// Escape Key
        /// </summary>
        Esc = 5,

        /// <summary>
        /// Enter Key
        /// </summary>
        Enter = 6
    }

    /// <summary>
    /// This class allows you to send and recive commands to the ProfiLux controler
    /// </summary>
    public abstract class Protocol
    {
        /// <summary>
        /// The Text for the view lines
        /// </summary>
        private static readonly string[] ViewTextLines = new string[4];

        /// <summary>
        /// Locking object for the view lines
        /// </summary>
        private static readonly object ViewTextLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="Protocol"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        protected Protocol(IBasicProtocol protocol)
        {
            this.Controler = protocol;
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public double Version
        {
            get
            {
                return Math.Round(this.Controler.Version * 0.01, 2);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get { return this.Controler.IsConnected; }
        }

        /// <summary>
        /// Gets the display text.
        /// </summary>
        /// <value>The display text.</value>
        public string DisplayText
        {
            get
            {
                string temptext = string.Empty;
                return this.Display.Aggregate(temptext, (current, line) => current + (line + "\n")).TrimEnd();
            }
        }

        /// <summary>
        /// Gets the display.
        /// </summary>
        /// <value>The display.</value>
        public virtual string[] Display
        {
            get
            {
                return new[] { TranslateDispayText(this.DisplayLine1), TranslateDispayText(this.DisplayLine2) };
            }
        }

        /// <summary>
        /// Gets the display line1.
        /// </summary>
        /// <value>The display line1.</value>
        protected abstract string DisplayLine1
        { 
            get;
        }

        /// <summary>
        /// Gets the display line2.
        /// </summary>
        /// <value>The display line2.</value>
        protected abstract string DisplayLine2
        {
            get;
        }

        /// <summary>
        /// Gets the view text line.
        /// </summary>
        /// <value>The view text line.</value>
        public abstract string ViewTextLine
        {
            get;
        }

        /// <summary>
        /// Gets the view text.
        /// </summary>
        /// <value>The view text.</value>
        public string ViewText
        {
            get
            {
                lock (ViewTextLock)
                {
                    string viewText = string.Empty;
                    return this.View.Aggregate(viewText, (current, line) => current + (line + "\n")).TrimEnd();
                }
            }
        }

        public string[] View
        {
            get
            {
                lock (ViewTextLock)
                {
                    if (this.Controler is ProfiluxHttpProtocol)
                    {
                        return new string[0];
                    }

                    // sink with the first row
                    while (true)
                    {
                        string text = this.ViewTextLine;
                        if (text[0] == 0)
                        {
                            ViewTextLines[text[0]] = TranslateDispayText(text.Substring(1));
                            break;
                        }

                        Trace.WriteLine("Discarding View Text " + text);
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        string text = this.ViewTextLine;
                        ViewTextLines[text[0]] = TranslateDispayText(text.Substring(1));
                    }

                    return ViewTextLines;
                }
            }
        }

        /// <summary>
        /// Gets the basic protocol.
        /// </summary>
        /// <value>The basic protocol.</value>
        protected IBasicProtocol Controler { get; private set; }

        /// <summary>
        /// Connects the specified connection settings.
        /// </summary>
        /// <param name="connectionSettings">The connection settings.</param>
        /// <returns>the connected protocol</returns>
        public static IProfilux Connect(ConnectionSettings connectionSettings)
        {
            IBasicProtocol protocol = null;
            switch (connectionSettings.ConnectionType)
            {
                case ConnectionType.Network:
                    protocol = new ProfiLuxProtocol(new NetworkConnection(connectionSettings.Address, connectionSettings.Port, connectionSettings.Timeout), connectionSettings.ControllerAddress);
                    break;
                case ConnectionType.Web:
                    protocol = new ProfiluxHttpProtocol(connectionSettings.Address, connectionSettings.Port, connectionSettings.Password);
                    break;
#if (PocketPC != true)
                case ConnectionType.RS232:
                    protocol = new ProfiLuxProtocol(new PortConnection(connectionSettings.ComPort, connectionSettings.BaudRate, connectionSettings.Timeout), connectionSettings.ControllerAddress);
                    break;
#endif
            }

            if (protocol != null)
            {
                int version = protocol.GetData(0);

                if (version >= 500)
                {
                    return new Profilux5Controler(protocol);
                }

                return new Profilux4Controler(protocol);
            }

            return null;
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="probes">The probes.</param>
        /// <returns>a list of data points for the sensor</returns>
        public abstract Collection<ItemDataRow> GetDataPoints(IProgressCallback callback, ReadOnlyCollection<Probe> probes);

        /// <summary>
        /// Clears the level alarms.
        /// </summary>
        public void ClearLevelAlarms()
        {
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            this.Controler.Disconnect();
        }
        
        private int[] sensorCount = new int[15];
        Dictionary<int,string> sensorIds = new Dictionary<int, string>();

        /// <summary>
        /// Gets the sensor id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="type">The type.</param>
        /// <returns>The Id</returns>
        public string GetSensorId(int index, SensorType type)
        {
            if (!this.sensorIds.ContainsKey(index))
            {
                this.sensorCount[(int)type]++;
                string id = type.ToString() + this.sensorCount[(int)type];
                this.sensorIds.Add(index, id);
            }

            return this.sensorIds[index];
        }

        /// <summary>
        /// Gets the level id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Id</returns>
        public string GetLevelId(int index)
        {
            return "Level" + (1 + index);
        }

        /// <summary>
        /// Gets the digtial input id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Id</returns>
        public string GetDigtialInputId(int index)
        {
            return "DigitalInput" + (1 + index);
        }

        /// <summary>
        /// Gets the S port id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Id</returns>
        public string GetSPortId(int index)
        {
            return "S" + (1 + index);
        }

        /// <summary>
        /// Gets the L port id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Id</returns>
        public string GetLPortId(int index)
        {
            return "L" + (1 + index);
        }

        /// <summary>
        /// Gets the light id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Id</returns>
        public string GetLightId(int index)
        {
            return "Light" + (1 + index);
        }

        /// <summary>
        /// Gets the dousing pump id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Id</returns>
        public string GetDousingPumpId(int index)
        {
            return "Pump" + (1 + index);
        }

        /// <summary>
        /// Gets the current pump id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string GetCurrentPumpId(int index)
        {
            return "CurrentPump" + (1 + index);
        }

        /// <summary>
        /// Converts the mode to a socket type
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>The socket type of the mode</returns>
        protected static DeviceMode LPortModeToSocketType(int mode)
        {
            switch (mode)
            {
                case 0:
                    return DeviceMode.Lights;
                case 1:
                    return DeviceMode.CurrentPump;
                case 2:
                    return DeviceMode.Decrease;
                case 3:
                    return DeviceMode.VariableIllumination;
                case 31:
                    return DeviceMode.AlwaysOff;
                default:
                    return DeviceMode.AlwaysOff;
            }
        }

        /// <summary>
        /// Translates the display text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>the translated text</returns>
        private static string TranslateDispayText(IEnumerable<char> text)
        {
            string newString = string.Empty;
            foreach (char item in text)
            {
                if (char.IsPunctuation(item)
                    || char.IsLetterOrDigit(item)
                    || char.IsWhiteSpace(item)
                    || char.IsSymbol(item))
                {
                    newString += item;
                }
                else if (item == 128 || item == 0)
                {
                    newString += 'O';
                }
                else if (item == 129 || item == 1)
                {
                    newString += 'N';
                }
                else if (item == 135)
                {
                    newString += '↓';
                }
                else if (item == 130)
                {
                    newString += '┐';
                }
                else
                {
                    ////newString += "{" + (int)item + "}";
                    newString += " ";
                }
            }

            return newString;
        }
    }
}

