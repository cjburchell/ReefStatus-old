// <copyright file="DataAccessException.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Database Access Excption
    /// </summary>
    [Serializable]
    public class DataAccessException : ReefStatusException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        public DataAccessException(int code, string message)
            : base(code, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DataAccessException(int code, string message, System.Exception inner)
            : base(code, message, inner)
        {
        }
    }
}
