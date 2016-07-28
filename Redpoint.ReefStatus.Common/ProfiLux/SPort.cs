// <copyright file="SPort.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using System.Threading;

    using RedPoint.ReefStatus.Common.ViewModel;

    /// <summary>
    /// Socket Status
    /// </summary>
    public class SPort : DeviceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SPort"/> class.
        /// </summary>
        public SPort()
            : base("strSPorts")
        {
            this.CurrentColour = Color.Purple;
        }

        private double current;

        private bool displayCurrentGraph;

        /// <summary>
        /// Gets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public override double DoubleValue
        {
            get
            {
                return this.Value != null ? (int)this.Value : 0;
            }
        }

        public override double? OldDoubleValue
        {
            get
            {
                return this.OldValue != null ? (double?)((int)this.OldValue) : null;
            }
        }

        public override object ConvertedValue
        {
            get { return this.Value; }
        }

        public override string DisplayNameValue
        {
            get
            {
                return base.DisplayNameValue;
            }

            set
            {
                if (value != base.DisplayNameValue)
                {
                    this.Commands.SendSPortText(this, value);
                    base.DisplayNameValue = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public int CurrentColourValue
        {
            get
            {
                return this.currentColourValue;
            }

            set
            {
                if (this.currentColourValue != value)
                {
                    this.currentColourValue = value;
                    this.OnPropertyChanged(() => this.CurrentMediaColour);
                    this.OnPropertyChanged(() => this.CurrentColourValue);
                    this.OnPropertyChanged(() => this.CurrentColour);
                }
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        [System.Xml.Serialization.XmlIgnore]
        public Color CurrentColour
        {
            get
            {
                return Color.FromArgb(this.CurrentColourValue);
            }

            set
            {
                this.CurrentColourValue = value.ToArgb();
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public System.Windows.Media.Color CurrentMediaColour
        {
            get
            {
                return System.Windows.Media.Color.FromArgb(this.CurrentColour.A, this.CurrentColour.R, this.CurrentColour.G, this.CurrentColour.B);
            }

            set
            {
                this.CurrentColour = Color.FromArgb(value.A, value.R, value.G, value.B);
            }
        }

        /// <summary>
        /// Updates the mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="items">The items.</param>
        /// <param name="logics">The logics.</param>
        public void UpdateMode(PortMode mode, IEnumerable<BaseInfo> items, IEnumerable<ProgramableLogic> logics)
        {
            this.DeviceMode = mode.DeviceMode;
            this.Port = mode.Port;
            this.Blackout = mode.BlackOut;
            this.Invert = mode.Invert;
            this.ModeString = this.SocketModeString(items);
            this.DefaultUnits = "State";
            this.ModeItem = this.GetAssociatedModeItem(items, logics);
        }

        /// <summary>
        /// Gets the graph id.
        /// </summary>
        /// <value>The graph id.</value>
        public override string GraphId
        {
            get
            {
                return this.Id;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public override GraphViewModel Graph
        {
            get
            {
                return this.graph ?? (this.graph = new SPortGraphViewModel(this));
            }
        }

        /// <summary>
        /// Gets the current id.
        /// </summary>
        /// <value>The current id.</value>
        public string CurrentId
        {
            get
            {
                return this.Id + "_Current";
            }
        }

        private int currentColourValue;


        /// <summary>
        /// Gets the value with units.
        /// </summary>
        /// <value>The value with units.</value>
        public override string ValueWithUnits
        {
            get
            {
                var baseItem = this.ModeItem as BaseInfo;
                if (baseItem == null)
                {
                    return Language.GetFrendyName(this.Value);
                }

                return Language.GetFrendyName(this.Value) + " (" + baseItem.ValueWithUnits + ")";
            }
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetValue(object value)
        {
            this.OldValue = this.Value;
            this.Value = (CurrentState) value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [System.Xml.Serialization.XmlIgnore]
        public bool IsActive
        {
            get
            {
                if (Value == null)
                    return false;

                return (CurrentState)this.Value == CurrentState.On;
            }

            set
            {
                if (((CurrentState)this.Value == CurrentState.On) != value)
                {
                    Commands.SendSocketState(this, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public double Current
        {
            get
            {
                return this.current;
            }

            set
            {
                if (this.current != value)
                {
                    this.current = value;
                    this.OnPropertyChanged(() => this.Current);
                }
            }
        }

        /// <summary>
        /// Gets or sets the old current value.
        /// </summary>
        /// <value>
        /// The old current value.
        /// </value>
        public double? OldCurrentValue { get; set; }

        public bool DisplayCurrentGraph
        {
            get
            {
                return this.displayCurrentGraph;
            }

            set
            {
                if (this.displayCurrentGraph != value)
                {
                    this.displayCurrentGraph = value;
                    this.OnPropertyChanged(() => this.DisplayCurrentGraph);
                }
            }
        }
        
        /// <summary>
        /// Gets Sockets the mode.
        /// </summary>
        /// <param name="probes">The probes.</param>
        /// <param name="items"></param>
        /// <returns>the socket mode string</returns>
        private string SocketModeString(IEnumerable<BaseInfo> items)
        {
            var invertText = string.Empty;
            if (this.Invert)
            {
                invertText = " (" + Language.GetResource("strInvert") + ")";
            }

            var deviceMode = Language.GetFrendyName(this.DeviceMode);

            if (this.IsUsePort)
            {
                return deviceMode + " " + this.Port + invertText;
            }

            if (this.IsProbe)
            {
                var probe = items.OfType<Probe>().FirstOrDefault(p => p.Index == (this.Port - 1));
                if (probe != null)
                {
                    return probe.Id + " " + deviceMode + invertText;
                }

                return Language.GetResource("strSensor") + this.Port + " " + deviceMode + invertText;
            }

            return deviceMode + invertText;
        }
    }
}
