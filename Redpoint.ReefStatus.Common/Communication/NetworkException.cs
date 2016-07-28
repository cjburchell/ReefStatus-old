// <copyright file="NetworkException.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Communication
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Network Exception caused when there is a network error
    /// </summary>
    [Serializable]
    public class NetworkException : ConnectionException
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        public NetworkException(int code,string message)
            : base(code, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public NetworkException(int code, string message, System.Exception inner)
            : base(code, message, inner)
        {
        }
    }
}
