// <copyright file="ConnectionException.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Communication
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Connection Exception
    /// </summary>
    [Serializable]
    public class ConnectionException : ReefStatusException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        public ConnectionException(int code, string message)
            : base(code, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public ConnectionException(int code, string message, System.Exception inner)
            : base(code, message, inner)
        {
        }
    }
}
