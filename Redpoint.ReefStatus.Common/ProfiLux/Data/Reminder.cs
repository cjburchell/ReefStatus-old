// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reminder.cs" company="RedpointGames">
//   2010
// </copyright>
// <summary>
//   The Controllers Reminders
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;

    /// <summary>
    /// The Controllers Reminders
    /// </summary>
    public class Reminder
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is overdue.
        /// </summary>
        public bool IsOverdue { get; set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public string Type => "Reminder";

        /// <summary>
        /// Gets or sets a value indicating whether [sent mail].
        /// </summary>
        public bool SentMail { get; set; }

        /// <summary>
        /// Gets or sets the next.
        /// </summary>
        public DateTime Next { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets Index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the period.
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is repeating.
        /// </summary>
        public bool IsRepeating { get; set; }
    }
}
