// <copyright file="Info.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    ///     Controller version information
    /// </summary>
    public class Info
    {
        public List<Maintenance> Maintenance { get; set; } = new List<Maintenance>();

        /// <summary>
        ///     Gets or sets the operation mode.
        /// </summary>
        /// <value>The operation mode.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public OperationMode OperationMode
        {
            get; set; }

        /// <summary>
        ///     Gets or sets the product id.
        /// </summary>
        /// <value>The product id.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductId ProductId
        {
            get; set;
        }

        /// <summary>
        ///     Gets or sets the software date.
        /// </summary>
        /// <value>The software date.</value>
        public DateTime SoftwareDate
        {
            get; set;
        }

        /// <summary>
        ///     Gets or sets the device address.
        /// </summary>
        /// <value>The device address.</value>
        public int DeviceAddress
        {
            get; set;
        }

        /// <summary>
        ///     Gets or sets the latitude.
        /// </summary>
        /// <value>The latitude.</value>
        public double Latitude
        {
            get; set;
        }

        /// <summary>
        ///     Gets or sets the longitude.
        /// </summary>
        /// <value>The longitude.</value>
        public double Longitude
        {
            get; set;
        }

        /// <summary>
        ///     Gets or sets the moon phase.
        /// </summary>
        /// <value>The moon phase.</value>
        public double MoonPhase
        {
            get; set;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Info" /> is alarm.
        /// </summary>
        /// <value><c>true</c> if alarm; otherwise, <c>false</c>.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrentState Alarm
        {
            get; set;
        }

        /// <summary>
        ///     Gets or sets the software version.
        /// </summary>
        /// <value>The software version.</value>
        public double SoftwareVersion
        {
            get; set;
        }

        /// <summary>
        ///     Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public string Model => ModelString(this.ProductId);

        /// <summary>
        ///     Gets or sets the serial number.
        /// </summary>
        /// <value>The serial number.</value>
        public int SerialNumber
        {
            get; set;
        }

        /// <summary>
        ///     Gets or sets the last update.
        /// </summary>
        /// <value>The last update.</value>
        public DateTime LastUpdate
        {
            get; set;
        }

        /// <summary>
        ///     Gets or sets the reminders.
        /// </summary>
        /// <value>The reminders.</value>
        public Collection<Reminder> Reminders { get; set; } = new Collection<Reminder>();

        /// <summary>
        ///     Gets a value indicating whether this instance is p3.
        /// </summary>
        /// <value><c>true</c> if this instance is p3; otherwise, <c>false</c>.</value>
        public bool IsP3 => this.ProductId == ProductId.ProfiLuxIII || this.ProductId == ProductId.ProfiLuxIIIEx;

        /// <summary>
        ///     Models the string.
        /// </summary>
        /// <param name="productId">
        ///     The product id.
        /// </param>
        /// <returns>
        ///     the string repesentation of the product Id
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
                    return "Unknown" + " (" + (int)productId + "???)";
            }
        }
    }
}