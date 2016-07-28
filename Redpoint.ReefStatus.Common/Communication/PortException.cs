// <copyright file="PortException.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Communication
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// PortException for errors when using a port connection
    /// </summary>
    [Serializable]
    public class PortException : ConnectionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PortException(int code, string message)
            : base(code, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public PortException(int code, string message, System.Exception inner)
            : base(code, message, inner)
        {
        }
    }
}
