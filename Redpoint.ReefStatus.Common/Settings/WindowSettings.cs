// <copyright file="WindowSettings.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Settings
{
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Window Settings
    /// </summary>
    public class WindowSettings
    {
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public Size Size { get; set; } = default(Size);

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location { get; set; } = default(Point);

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        /// <value>The state of the window.</value>
        public FormWindowState WindowState { get; set; }
    }
}
