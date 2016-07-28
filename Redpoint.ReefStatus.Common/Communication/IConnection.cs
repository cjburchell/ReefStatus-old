// <copyright file="IConnection.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Communication
{
    using System;

    /// <summary>
    /// Connection interface
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <exception cref="ConnectionException">If error in writing</exception>
        void Write(string text);

        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <returns>A string that was read</returns>
        /// <exception cref="ConnectionException">If error in reading</exception>
        string Read();

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="ConnectionException">If error in writing</exception>
        void Write(byte[] data);

        /// <summary>
        /// Reads the specified packet size.
        /// </summary>
        /// <param name="packetSize">The packet size.</param>
        /// <param name="bytesReceived">The bytes received.</param>
        /// <returns>bytes that were read</returns>
        /// <exception cref="ConnectionException">If error in reading</exception>
        byte[] Read(int packetSize, out int bytesReceived);

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        void Disconnect();
    }
}
