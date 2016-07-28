// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBasicProtocol.cs" company="Redpoint">
//   2009
// </copyright>
// <summary>
//   The i basic protocol.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;

    /// <summary>
    /// The i basic protocol.
    /// </summary>
    public interface IBasicProtocol : IDisposable
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        int Version { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The data value</returns>
        int GetData(int code);

        /// <summary>
        /// Gets the data text.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The data value</returns>
        string GetDataText(int code);

        /// <summary>
        /// Gets the data short array.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The data value</returns>
        short[] GetDataShortArray(int code);

        /// <summary>
        /// Sends the data.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="data">The data.</param>
        void SendData(int code, int data);

        /// <summary>
        /// Sends the text.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="data">The data.</param>
        void SendText(int code, string data);

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        void Disconnect();

        byte[] GetDataByteArray(int code);

        bool[] GetDataBoolArray(int code);

        short[] GetDataTwoByteArray(int code);
    }
}