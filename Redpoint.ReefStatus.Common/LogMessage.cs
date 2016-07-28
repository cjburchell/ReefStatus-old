﻿// <copyright file="LogMessage.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common
{
    using System;

    /// <summary>
    /// A message to log
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        public LogMessage(int code, string message)
        {
            this.Time = DateTime.Now;
            this.Code = code;
            this.Message = message;
        }

        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <value>The time.</value>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Gets or sets the excption.
        /// </summary>
        /// <value>The excption.</value>
        public System.Exception Exception { get; set; }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public int Code { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", this.Time, this.Code, this.Message);
        }
    }
}
