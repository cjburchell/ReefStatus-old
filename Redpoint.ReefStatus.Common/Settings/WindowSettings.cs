// <copyright file="WindowSettings.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

using RedPoint.ReefStatus.Common.UI.ViewModel;

namespace RedPoint.ReefStatus.Common.Settings
{
    using System.Drawing;
    using System.Windows.Forms;

    using Microsoft.Practices.Prism.Mvvm;

    /// <summary>
    /// Window Settings
    /// </summary>
    public class WindowSettings : BindableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowSettings"/> class.
        /// </summary>
        public WindowSettings()
        {
            Size = new Size();
            Location = new Point();
        }

        private Size size;

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public Size Size
        {
            get { return size; }
            set
            {
                size = value;
                this.OnPropertyChanged(() => this.Size);
            }
        }

        private Point location;

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location
        {
            get { return location; }
            set
            {
                location = value;
                this.OnPropertyChanged(() => this.Location);
            }
        }

        private FormWindowState windowState;

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        /// <value>The state of the window.</value>
        public FormWindowState WindowState
        {
            get { return windowState; }
            set
            {
                windowState = value; 
                this.OnPropertyChanged(() => this.WindowState);
            }
        }
    }
}
