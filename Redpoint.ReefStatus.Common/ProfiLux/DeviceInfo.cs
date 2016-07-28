// <copyright file="DeviceInfo.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Practices.Prism.Mvvm;

    /// <summary>
    /// Divice Information
    /// </summary>
    public abstract class DeviceInfo : BaseInfo
    {
        /// <summary>
        /// The blackout.
        /// </summary>
        private int blackout;

        /// <summary>
        /// The invert.
        /// </summary>
        private bool invert;

        /// <summary>
        /// The mode.
        /// </summary>
        private DeviceMode mode;

        /// <summary>
        /// The port.
        /// </summary>
        private int port;

        /// <summary>
        /// The port number.
        /// </summary>
        private int portNumber;

        /// <summary>
        /// the mode
        /// </summary>
        private string modeString;

        /// <summary>
        /// The mode item
        /// </summary>
        private BindableBase modeItem;

        protected DeviceInfo(string type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        public DeviceMode DeviceMode
        {
            get
            {
                return this.mode;
            }

            set
            {
                if (this.mode != value)
                {
                    this.mode = value;
                    this.OnPropertyChanged(() => this.DeviceMode);
                    this.OnPropertyChanged(() => this.IsConstant);
                }
            }
        }

        /// <summary>
        /// Gets the mode.
        /// </summary>
        /// <value>The mode.</value>
        public override string Mode
        {
            get
            {
                return this.ModeString;
            }
        }

        /// <summary>
        /// Gets or sets the mode string.
        /// </summary>
        /// <value>The mode string.</value>
        public string ModeString
        {
            get
            {
                return this.modeString;
            }

            set
            {
                if (this.modeString != value)
                {
                    this.modeString = value;
                    this.OnPropertyChanged(() => this.Mode);
                }
            }
        }

        /// <summary>
        /// Gets or sets the port number.
        /// </summary>
        /// <value>The port number.</value>
        public int PortNumber
        {
            get
            {
                return this.portNumber;
            }

            set
            {
                if (this.portNumber != value)
                {
                    this.portNumber = value;
                    this.OnPropertyChanged(() => this.PortNumber);
                }
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
                if (this.port != value)
                {
                    this.port = value;
                    this.OnPropertyChanged(() => this.Port);
                }
            }
        }

        /// <summary>
        /// Gets or sets the blackout.
        /// </summary>
        /// <value>The blackout.</value>
        public int Blackout
        {
            get
            {
                return this.blackout;
            }

            set
            {
                if (this.blackout != value)
                {
                    this.blackout = value;
                    this.OnPropertyChanged(() => this.Blackout);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DeviceInfo"/> is invert.
        /// </summary>
        /// <value><c>true</c> if invert; otherwise, <c>false</c>.</value>
        public bool Invert
        {
            get
            {
                return this.invert;
            }

            set
            {
                if (this.invert != value)
                {
                    this.invert = value;
                    this.OnPropertyChanged(() => this.Invert);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mode item.
        /// </summary>
        /// <value>
        /// The mode item.
        /// </value>
        [System.Xml.Serialization.XmlIgnore]
        public BindableBase ModeItem
        {
            get
            {
                return this.modeItem;
            }

            set
            {
                if (Equals(value, this.modeItem))
                {
                    return;
                }
                this.modeItem = value;
                this.OnPropertyChanged(() => this.ModeItem);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is constant.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is constant; otherwise, <c>false</c>.
        /// </value>
        public override bool IsConstant
        {
            get
            {
                return DeviceMode == DeviceMode.AlwaysOn
                       || DeviceMode == DeviceMode.AlwaysOff;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is use port.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is use port; otherwise, <c>false</c>.
        /// </value>
        protected bool IsUsePort
        {
            get
            {
                return DeviceMode == DeviceMode.Lights
                       || DeviceMode == DeviceMode.Timer
                       || DeviceMode == DeviceMode.Water
                       || DeviceMode == DeviceMode.ProgrammableLogic
                       || DeviceMode == DeviceMode.CurrentPump
                       || DeviceMode == DeviceMode.VariableIllumination;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is probe.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is probe; otherwise, <c>false</c>.
        /// </value>
        protected bool IsProbe
        {
            get
            {
                return DeviceMode == DeviceMode.Decrease
                       || DeviceMode == DeviceMode.Increase
                       || DeviceMode == DeviceMode.Substrate
                       || DeviceMode == DeviceMode.ProbeAlarm;
            }
        }

        /// <summary>
        /// Gets the associated mode item.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="logics"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public BindableBase GetAssociatedModeItem(IEnumerable<BaseInfo> items, IEnumerable<ProgramableLogic> logics = null)
        {
            if (this.IsProbe)
            {
                return items.OfType<Probe>().FirstOrDefault(p => p.Index == (this.Port - 1));
            }

            switch (this.DeviceMode)
            {
                case DeviceMode.Lights:
                    return items.OfType<Light>().FirstOrDefault(item => item.Channel == (this.Port - 1));
                case DeviceMode.Timer:
                    return items.OfType<DosingPump>().FirstOrDefault(item => item.Channel == (this.Port - 1));
                case DeviceMode.Water:
                    return items.OfType<LevelSensor>().FirstOrDefault(item => item.Index == (this.Port - 1));
                case DeviceMode.CurrentPump:
                    return items.OfType<CurrentPump>().FirstOrDefault(item => item.Index == (this.Port - 1));
                case DeviceMode.ProgrammableLogic:
                    if (logics != null)
                    {
                        return logics.FirstOrDefault(item => item.Index == (this.Port - 1));
                    }
                    break;
            }

            return null;
        }
    }
}