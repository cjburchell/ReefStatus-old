
// <copyright file="Connection.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Communication
{
    using System;
    using System.Text;

    /// <summary>
    /// Basic Connection Class
    /// Provides common funcunality to all Connections
    /// </summary>
    public abstract class Connection : IConnection
    {
        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="ConnectionException">If error in writing</exception>
        public abstract void Write(byte[] data);

        /// <summary>
        /// Reads the specified packet size.
        /// </summary>
        /// <param name="packetSize">The packet size.</param>
        /// <param name="bytesReceived">The bytes received.</param>
        /// <returns>The bytes read</returns>
        /// <exception cref="ConnectionException">If error in reading</exception>
        public abstract byte[] Read(int packetSize, out int bytesReceived);

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <exception cref="ConnectionException">If error in writing</exception>
        public void Write(string text)
        {
            this.Write(Encoding.ASCII.GetBytes(text));
        }

        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <returns>the string read</returns>
        /// <exception cref="ConnectionException">If error in reading</exception>
        public string Read()
        {
            // The following will block until te page is transmitted.
            byte[] data;
            string text = string.Empty;
            do
            {
                int dataCount;
                data = Read(256, out dataCount);
                text = text + Encoding.ASCII.GetString(data, 0, dataCount);
            }
            while (data.Length > 0);

            return text;
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public abstract void Disconnect();

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}