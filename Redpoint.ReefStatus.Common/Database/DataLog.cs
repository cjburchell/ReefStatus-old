using System;
using Microsoft.Practices.Prism.Mvvm;

namespace RedPoint.ReefStatus.Common.Database
{
    public class DataLog : BindableBase
	{
        /// <summary>
        /// The type
        /// </summary>
        private int type;

        /// <summary>
        /// The time
        /// </summary>
        private DateTime time;

        /// <summary>
        /// The value
        /// </summary>
        private double value;

        /// <summary>
        /// The index
        /// </summary>
        private int index;

        /// <summary>
        /// The controller
        /// </summary>
        private int controller;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public int Type
        {
            get
            {
                return this.type;
            }

            set
            {
                if (value == this.type)
                {
                    return;
                }

                this.type = value;
                this.OnPropertyChanged(() => this.Type);
            }
        }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public DateTime Time
        {
            get
            {
                return this.time;
            }

            set
            {
                if (value.Equals(this.time))
                {
                    return;
                }

                this.time = value;
                this.OnPropertyChanged(() => this.Time);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public double Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (value.Equals(this.value))
                {
                    return;
                }
                this.value = value;
                this.OnPropertyChanged(() => this.Value);
            }
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index
        {
            get
            {
                return this.index;
            }
            set
            {
                if (value == this.index)
                {
                    return;
                }
                this.index = value;
                this.OnPropertyChanged(() => this.Index);
            }
        }

        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        /// <value>
        /// The controller.
        /// </value>
        public int Controller
        {
            get
            {
                return this.controller;
            }
            set
            {
                if (value == this.controller)
                {
                    return;
                }
                this.controller = value;
                this.OnPropertyChanged(() => this.Controller);
            }
        }
	}
}

