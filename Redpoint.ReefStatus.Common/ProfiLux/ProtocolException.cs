// <copyright file="ProtocolException.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;

    /// <summary>
    /// Protocol Exception
    /// </summary>
    [Serializable]
    public class ProtocolException : ReefStatusException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        public ProtocolException(int code, string message)
            : base(code, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public ProtocolException(int code, string message, System.Exception inner)
            : base(code, message, inner)
        {
        }
    }
}
