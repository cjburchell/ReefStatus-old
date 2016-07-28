// <copyright file="PortConnection.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Communication
{
    using System;
    using System.Globalization;
    using System.IO.Ports;


    /// <summary>
    /// Network Connection allows the protocol to be run over a RS232/USB port
    /// </summary>
    public class PortConnection : Connection
    {
        private readonly SerialPort port;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortConnection"/> class.
        /// </summary>
        /// <param name="portNumber">The port number.</param>
        /// <param name="baudRate">The baud rate.</param>
        /// <param name="timeout">The time out.</param>
        /// <exception cref="PortException">If unable to conenct to port</exception>
        public PortConnection(int portNumber, int baudRate, int timeout)
        {
            try
            {
                port = new SerialPort("COM" + portNumber.ToString(CultureInfo.CurrentCulture), baudRate)
                           {
                               ReadTimeout = timeout,
                               WriteTimeout = timeout
                           };
                port.Open();
            }
            catch (System.UnauthorizedAccessException ex)
            {
                throw new PortException(50, "Unable to connect to COM" + portNumber + " " + ex.Message);
            }
            catch (System.IO.IOException ex)
            {
                throw new PortException(50, "Unable to connect to COM" + portNumber + " " + ex.Message);
            }
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="PortException">If error in writing to port</exception>
        public override void Write(byte[] data)
        {
            try
            {
                port.Write(data, 0, data.Length);
            }
            catch (TimeoutException ex)
            {
                throw new PortException(52, "Write Timed out\n" + ex.Message);
            }
        }

        /// <summary>
        /// Reads the specified packet size.
        /// </summary>
        /// <param name="packetSize">The packet size.</param>
        /// <param name="bytesReceived">The bytes received.</param>
        /// <returns>the bytes that were read</returns>
        /// <exception cref="PortException">If error in reading from port</exception>
        public override byte[] Read(int packetSize, out int bytesReceived)
        {
            try
            {
                byte[] data = new byte[packetSize];
                bytesReceived = port.Read(data, 0, packetSize);
                return data;
            }
            catch (TimeoutException ex)
            {
                throw new PortException(53, "Read Timed out\n" + ex.Message);
            }
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public override void Disconnect()
        {
            port.Close();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
                if (port != null)
                {
                    port.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
