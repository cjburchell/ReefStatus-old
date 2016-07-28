// <copyright file="ErrorCodeException.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The error code exception.
    /// </summary>
    public class ErrorCodeException : ProtocolException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        public ErrorCodeException(int code, string message, int errorCode)
            : base(code, message)
        {
            this.ErrorCode = errorCode;
            Trace.WriteLine(string.Format("Exception Code: {0} ErrorCode {1} Message: {2}", code, errorCode, message));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="errorCode">The error code.</param>
        public ErrorCodeException(int code, string message, Exception inner, int errorCode)
            : base(code, message, inner)
        {
            this.ErrorCode = errorCode;
            Trace.WriteLine(string.Format("Exception Code: {0} ErrorCode {1} Message: {2}", code, errorCode, message));
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public int ErrorCode { get; private set; }
    }
}