// <copyright file="NetworkConnection.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Communication
{
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.Diagnostics;

    /// <summary>
    /// Network Connection allows the protocol to be run over a network
    /// </summary>
    public class NetworkConnection : Connection
    {
        private readonly string address;

        private readonly int port;

        private Socket socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkConnection"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="timeout">The timeout.</param>
        /// <exception cref="NetworkException">if unable to connect</exception>
        public NetworkConnection(string address, int port, int timeout)
        {
            this.address = address;
            this.port = port;
            try
            {
                this.socket = ConnectSocket(address, port);
                if (this.socket == null)
                {
                    throw new NetworkException(10, string.Format(CultureInfo.CurrentCulture, "Unable to connect to {0}:{1}", address, port));
                }

                this.socket.ReceiveTimeout = timeout;
                this.socket.SendTimeout = timeout;

                Logger.Instance.Log(new LogMessage(1, "Connected to " + address + ":" + port));
            }
            catch (SocketException ex)
            {
                throw new NetworkException(11, string.Format(CultureInfo.CurrentCulture, "Unable to connect to {0}:{1} {2}", address, port, ex.Message), ex);
            }
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="NetworkException">if unable to write data</exception>
        public override void Write(byte[] data)
        {
            try
            {
                socket.Send(data);
            }
            catch (SocketException ex)
            {
                throw new NetworkException(12, "Unable to Send Data Packet: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Reads the specified packet size.
        /// </summary>
        /// <param name="packetSize">Size of the packet.</param>
        /// <param name="bytesReceived">The bytes received.</param>
        /// <returns>the bytes read</returns>
        /// <exception cref="NetworkException">if unable to read data</exception>
        public override byte[] Read(int packetSize, out int bytesReceived)
        {
            try
            {
                byte[] data = new byte[packetSize];
                bytesReceived = socket.Receive(data, data.Length, 0);
                return data;
            }
            catch (SocketException ex)
            {
                throw new NetworkException(13, "Unable to Read Data Packet: " + ex.Message, ex);
            }
        
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public override void Disconnect()
        {
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Disconnect(true);
                    socket.Close();

                    Logger.Instance.Log(new LogMessage(1, "Diconnected from " + this.address + ":" + this.port));
                }
                catch(SocketException ex)
                {
                    Trace.WriteLine(ex);
                }
            }

            socket = null;
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
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Connects the socket.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="port">The port.</param>
        /// <returns>The connected socket</returns>
        private static Socket ConnectSocket(string server, int port)
        {
            IPAddress serverIp;
            if (IpAddressTryParse(server, out serverIp))
            {
                IPEndPoint ipe = new IPEndPoint(serverIp, port);
                Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    return tempSocket;
                }
            }

            // Get host related information.
            IPHostEntry hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    return tempSocket;
                }
            }

            return null;
        }

        /// <summary>
        /// IPs the address try parse.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="serverIp">The server ip.</param>
        /// <returns>true if the IPAddress is valid</returns>
        private static bool IpAddressTryParse(string server, out IPAddress serverIp)
        {
            return IPAddress.TryParse(server, out serverIp);
        }
    }
}
