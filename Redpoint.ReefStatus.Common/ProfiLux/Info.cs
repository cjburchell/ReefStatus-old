// <copyright file="Info.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Linq;

    using Microsoft.Practices.Prism.Mvvm;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    public class Maintenance : BindableBase
    {

        public Maintenance( int index )
        {
            this.Index = index;
        }

        public int Index
        {
            get;
            set;
        }
        

        private bool isActive;

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                if (this.isActive != value)
                {
                    if (value)
                    {
                        this.TimeLeft = this.Duration;
                    }

                    this.isActive = value;
                    this.OnPropertyChanged(() => this.IsActive);
                }
            }
        }

        private int duration;

        public int Duration
        {
            get
            {
                return this.duration;
            }

            set
            {
                if (this.duration != value)
                {
                    this.duration = value;
                    this.OnPropertyChanged(() => this.Duration);
                    this.OnPropertyChanged(() => this.TimeOn);
                }
            }
        }

        public int TimeOn
        {
            get
            {
                return this.Duration - this.TimeLeft;
            }

            set
            {
            }
        }

        private int timeLeft;

        public int TimeLeft
        {
            get
            {
                return this.timeLeft;
            }

            set
            {
                if (this.timeLeft != value)
                {
                    this.timeLeft = value;
                    this.OnPropertyChanged(() => this.TimeLeft);
                    this.OnPropertyChanged(() => this.TimeOn);
                }
            }
        }
    }


    /// <summary>
    /// Controller version information
    /// </summary>
    public class Info : BindableBase
    {
        /// <summary>
        /// The alarm.
        /// </summary>
        private CurrentState alarm;

        /// <summary>
        /// The device address.
        /// </summary>
        private int deviceAddress;

        /// <summary>
        /// The last update.
        /// </summary>
        private DateTime lastUpdate;

        /// <summary>
        /// The product id.
        /// </summary>
        private ProductId productId;

        /// <summary>
        /// The serial number.
        /// </summary>
        private int serialNumber;

        /// <summary>
        /// The software date.
        /// </summary>
        private DateTime softwareDate;

        /// <summary>
        /// The software version.
        /// </summary>
        private double softwareVersion;

        /// <summary>
        /// The Operation Mode
        /// </summary>
        private OperationMode operationMode;

        /// <summary>
        /// the latitude of the Controller
        /// </summary>
        private double latitude;

        /// <summary>
        /// longitude of the Controller
        /// </summary>
        private double longitude;

        /// <summary>
        /// the current moon phase
        /// </summary>
        private double moonPhase;

        /// <summary>
        /// Initializes a new instance of the <see cref="Info"/> class.
        /// </summary>
        public Info()
        {
            this.Reminders = new SafeObservableCollection<Reminder>();
            this.Maintenance = new Maintenance[4];
            this.Maintenance[0] = new Maintenance(0);
            this.Maintenance[1] = new Maintenance(1);
            this.Maintenance[2] = new Maintenance(2);
            this.Maintenance[3] = new Maintenance(3);
        }

        [System.Xml.Serialization.XmlIgnore]
        public Maintenance[] Maintenance { get; set; }

        /// <summary>
        /// Gets or sets the operation mode.
        /// </summary>
        /// <value>The operation mode.</value>
        public OperationMode OperationMode
        {
            get
            {
                return this.operationMode;
            }

            set
            {
                if (this.operationMode != value)
                {
                    this.operationMode = value;
                    this.OnPropertyChanged(() => this.OperationMode);
                }
            }
        }

        /// <summary>
        /// Gets or sets the product id.
        /// </summary>
        /// <value>The product id.</value>
        public ProductId ProductId
        {
            get
            {
                return this.productId;
            }

            set
            {
                if (this.productId != value)
                {
                    this.productId = value;
                    this.OnPropertyChanged(() => this.ProductId);
                    this.OnPropertyChanged(() => this.Model);
                    this.OnPropertyChanged(() => this.IsP3);
                }
            }
        }

        /// <summary>
        /// Gets or sets the software date.
        /// </summary>
        /// <value>The software date.</value>
        public DateTime SoftwareDate
        {
            get
            {
                return this.softwareDate;
            }

            set
            {
                if (this.softwareDate != value)
                {
                    this.softwareDate = value;
                    this.OnPropertyChanged(() => this.SoftwareDate);
                }
            }
        }

        /// <summary>
        /// Gets or sets the device address.
        /// </summary>
        /// <value>The device address.</value>
        public int DeviceAddress
        {
            get
            {
                return this.deviceAddress;
            }

            set
            {
                if (this.deviceAddress != value)
                {
                    this.deviceAddress = value;
                    this.OnPropertyChanged(() => this.DeviceAddress);
                }
            }
        }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>The latitude.</value>
        public double Latitude
        {
            get
            {
                return this.latitude;
            }

            set
            {
                if (this.latitude != value)
                {
                    this.latitude = value;
                    this.OnPropertyChanged(() => this.Latitude);
                }
            }
        }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>The longitude.</value>
        public double Longitude
        {
            get
            {
                return this.longitude;
            }

            set
            {
                if (this.longitude != value)
                {
                    this.longitude = value;
                    this.OnPropertyChanged(() => this.Longitude);
                }
            }
        }

        /// <summary>
        /// Gets or sets the moon phase.
        /// </summary>
        /// <value>The moon phase.</value>
        public double MoonPhase
        {
            get
            {
                return this.moonPhase;
            }

            set
            {
                if (this.moonPhase != value)
                {
                    this.moonPhase = value;
                    this.OnPropertyChanged(() => this.MoonPhase);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Info"/> is alarm.
        /// </summary>
        /// <value><c>true</c> if alarm; otherwise, <c>false</c>.</value>
        public CurrentState Alarm
        {
            get
            {
                return this.alarm;
            }

            set
            {
                if (this.alarm != value)
                {
                    this.alarm = value;
                    this.OnPropertyChanged(() => this.Alarm);
                }
            }
        }

        /// <summary>
        /// Gets or sets the software version.
        /// </summary>
        /// <value>The software version.</value>
        public double SoftwareVersion
        {
            get
            {
                return this.softwareVersion;
            }

            set
            {
                if (this.softwareVersion != value)
                {
                    this.softwareVersion = value;
                    this.OnPropertyChanged(() => this.SoftwareVersion);
                }
            }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        [System.Xml.Serialization.XmlIgnore]
        public string Model
        {
            get
            {
                return ModelString(this.ProductId);
            }
        }

        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        /// <value>The serial number.</value>
        public int SerialNumber
        {
            get
            {
                return this.serialNumber;
            }

            set
            {
                this.serialNumber = value;
                this.OnPropertyChanged(() => this.SerialNumber);
            }
        }

        /// <summary>
        /// Gets or sets the last update.
        /// </summary>
        /// <value>The last update.</value>
        public DateTime LastUpdate
        {
            get
            {
                return this.lastUpdate;
            }

            set
            {
                this.lastUpdate = value;
                this.OnPropertyChanged(() => this.LastUpdate);
            }
        }

        /// <summary>
        /// Gets or sets the reminders.
        /// </summary>
        /// <value>The reminders.</value>
        public SafeObservableCollection<Reminder> Reminders { get; set; }

        /// <summary>
        /// Gets the info text.
        /// </summary>
        /// <value>The info text.</value>
        [System.Xml.Serialization.XmlIgnore]
        public string InfoText
        {
            get
            {
                string versionText = "Software Version: " + this.SoftwareVersion + "\n";
                versionText += "Software Date: " + this.SoftwareDate.ToShortDateString() + "\n";
                versionText += "Model: " + this.Model + "\n";
                versionText += "Serial Number: " + this.SerialNumber + "\n";
                versionText += "Device Address: " + this.DeviceAddress + "\n";
                versionText += "Alarm: " + this.Alarm + "\n";
                versionText += "Mode: " + Language.GetFrendyName(this.OperationMode) + "\n";
                versionText += "Last Update: " + this.LastUpdate + "\n";

                versionText += "\n";

                return this.Reminders.Aggregate(versionText, (current, reminder) => current + (reminder.Text + " Reminder at " + reminder.Next.ToShortDateString() + "\n"));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is p3.
        /// </summary>
        /// <value><c>true</c> if this instance is p3; otherwise, <c>false</c>.</value>
        public bool IsP3
        {
            get
            {
                return ProductId == ProductId.ProfiLuxIII || ProductId == ProductId.ProfiLuxIIIEx;
            }
        }

        /// <summary>
        /// Models the string.
        /// </summary>
        /// <param name="productId">
        /// The product id.
        /// </param>
        /// <returns>
        /// the string repesentation of the product Id
        /// </returns>
        private static string ModelString(ProductId productId)
        {
            switch (productId)
            {
                case ProductId.ProfiLuxII:
                    return "ProfiLux II";
                case ProductId.ProfiLuxIIEx:
                    return "ProfiLux II Ex";
                case ProductId.ProfiLuxPlusII:
                    return "ProfiLux Plus II";
                case ProductId.ProfiLuxPlusIIEx:
                    return "ProfiLux Plus II Ex";
                case ProductId.ProfiLuxIII:
                    return "ProfiLux III";
                case ProductId.ProfiLuxIIIEx:
                    return "ProfiLux III Ex";
                case ProductId.ProfiLuxIIOutdoor:
                    return "ProfiLux II Outdoor";
                case ProductId.ProfiLuxIITerra:
                    return "ProfiLux II Terra";
                case ProductId.ProfiLuxLightII:
                    return "ProfiLux II Light";
                default:
                    return "Unknown" + " (" + (int) productId + "???)";
            }
        }
    }
}