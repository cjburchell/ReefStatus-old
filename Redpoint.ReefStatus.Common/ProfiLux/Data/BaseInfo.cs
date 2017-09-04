// <copyright file="BaseInfo.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    /// <summary>
    /// Protocol item
    /// </summary>
    public abstract class BaseInfo
    {
        /// <summary>
        /// Id of the object
        /// </summary>
        private string id;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseInfo"/> class.
        /// </summary>
        protected BaseInfo(string type)
        {
            this.Type = type;
        }

        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        /// <value>The units.</value>
        public string DefaultUnits { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Id
        {
            get
            {
                return this.id;
            }

            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    if (string.IsNullOrEmpty(this.DisplayName))
                    {
                        this.DisplayName = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        /// <value>The units.</value>
        public string Units { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}