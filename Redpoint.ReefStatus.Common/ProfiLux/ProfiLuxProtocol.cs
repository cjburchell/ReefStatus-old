// <copyright file="ProfiLuxProtocol.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using RedPoint.ReefStatus.Common.Communication;

    /// <summary>
    /// This class allows you to send and recive commands to the ProfiLux Controller
    /// </summary>
    public class ProfiLuxProtocol : BasicProtocol, IBasicProtocol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfiLuxProtocol"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="address">The address.</param>
        /// <example>
        /// <code>
        /// using(Protocol protocol = new Protocol(new NetworkConnection("192.168.0.3", 10001, 5000), 1, 0))
        /// {
        /// int version = protocol.GetData(Code.SOFTWAREVERSION);
        /// }
        /// </code>
        /// </example>
        /// <exception cref="ProtocolException">If there is an error in the protocol</exception>
        public ProfiLuxProtocol(IConnection connection, int address)
        {
            this.Connection = connection;
            this.Address = address;
            this.Version = this.GetData(0);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get { return this.Connection != null; }
        }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        private int Address { get; set; }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        private IConnection Connection { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            if (this.Connection != null)
            {
                this.Connection.Disconnect();
            }

            this.Connection = null;
        }

        /// <summary>
        /// Sends the data.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="data">The data.</param>
        /// <exception cref="ConnectionException">If error in writing</exception>
        /// <exception cref="ProtocolException">If there is an error in the protocol</exception>
        public void SendData(int code, int data)
        {
            if (!this.IsConnected)
            {
                throw new ProtocolException(500, "Unable to Send Data Not Connected!");
            }

            ProfiLux.SendCommand(code, data, this.Connection, this.Address);
            while (true) 
            {
                Collection<byte> reply = this.ReadPacket();
                try
                {
                    VerifyAckPacket(reply);
                    break;
                }
                catch (ProtocolException ex)
                {
                    Logger.Instance.LogError(ex);
                }
            }
        }

        /// <summary>
        /// Gets the data text.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>the reply as text</returns>
        /// <exception cref="ConnectionException">If error in writing</exception>
        /// <exception cref="ProtocolException">If there is an error in the protocol</exception>
        public string GetDataText(int code)
        {
            if (!this.IsConnected)
            {
                throw new ProtocolException(501, "Unable to Get Data Not Connected!");
            }

            ProfiLux.SendCommand(code, this.Connection, this.Address);
            Collection<byte> reply;
            while (true)
            {
                reply = this.ReadPacket();
                try
                {
                    VerifyDataPacket(reply, code);
                    break;
                }
                catch (ErrorCodeException)
                {
                    throw;
                }
                catch (ProtocolException ex)
                {
                    Logger.Instance.LogError(ex);
                }
            }

            return ProfiLux.GetMessageString(reply);
        }

        public void SendText(int code, string data)
        {
            if (!this.IsConnected)
            {
                throw new ProtocolException(500, "Unable to Send Text Not Connected!");
            }

            ProfiLux.SendTextCommand(code, data, this.Connection, this.Address);

            while (true)
            {
                Collection<byte> reply = this.ReadPacket();
                try
                {
                    VerifyAckPacket(reply);
                    break;
                }
                catch (ProtocolException ex)
                {
                    Logger.Instance.LogError(ex);
                }
            }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The reply as a number</returns>
        /// <exception cref="ConnectionException">If error in writing</exception>
        /// <exception cref="ProtocolException">If there is an error in the protocol</exception>
        public int GetData(int code)
        {
            if (!this.IsConnected)
            {
                throw new ProtocolException(502, "Unable to Get Data Not Connected!");
            }

            ProfiLux.SendCommand(code, this.Connection, this.Address);
            Collection<byte> reply;
            while (true)
            {
                reply = this.ReadPacket();
                try
                {
                    VerifyDataPacket(reply, code);
                    break;
                }
                catch (ErrorCodeException)
                {
                    throw;
                }
                catch (ProtocolException ex)
                {
                    Logger.Instance.LogError(ex);
                }
            }

            int data = ProfiLux.GetMessageData(reply);
            return data;
        }

        /// <summary>
        /// Gets the data short array.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The reply as an array of shorts</returns>
        /// <exception cref="ConnectionException">If error in writing</exception>
        /// <exception cref="ProtocolException">If there is an error in the protocol</exception>
        public short[] GetDataShortArray(int code)
        {
            if (!this.IsConnected)
            {
                throw new ProtocolException(503, "Unable to Get Data Not Connected!");
            }

            ProfiLux.SendCommand(code, this.Connection, this.Address);
            Collection<byte> reply;
            while (true)
            {
                reply = this.ReadPacket();
                try
                {
                    VerifyDataPacket(reply, code);
                    break;
                }
                catch (ErrorCodeException)
                {
                    throw;
                }
                catch (ProtocolException ex)
                {
                    Logger.Instance.LogError(ex);
                }
            }

            return ProfiLux.GetMessageDataShortArray(reply);
        }

        public byte[] GetDataByteArray(int code)
        {
            if (!this.IsConnected)
            {
                throw new ProtocolException(503, "Unable to Get Data Not Connected!");
            }

            ProfiLux.SendCommand(code, this.Connection, this.Address);
            Collection<byte> reply;
            while (true)
            {
                reply = this.ReadPacket();
                try
                {
                    VerifyDataPacket(reply, code);
                    break;
                }
                catch (ErrorCodeException)
                {
                    throw;
                }
                catch (ProtocolException ex)
                {
                    Logger.Instance.LogError(ex);
                }
            }

            return ProfiLux.GetMessageBytes(reply);
        }

        /// <summary>
        /// Verifies the data packet.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <param name="code">The code.</param>
        private static void VerifyDataPacket(IList<byte> reply, int code)
        {
            if (reply.Count < 4)
            {
                // strange packet size!
                throw new ProtocolException(504, "Expecting Packet size of at least 4");
            }

            if (reply[4] == ProfiLux.EOT)
            {
                throw new ProtocolException(505, "Unexpected Message: Empty Reply");
            }

            if (reply[4] == ProfiLux.STX)
            {
                if (reply[5] == ProfiLux.NAK)
                {
                    // error message
                    byte errorCode = reply[6];
                    throw new ErrorCodeException(506, string.Format("Error with code {0} Reply Code {1}", errorCode, code), errorCode);
                }

                if (reply[5] == ProfiLux.ACK)
                {
                    throw new ProtocolException(507, "Unexpected Message: ACK");
                }

                // should be ok we must now look for the code and verify it
                int replyCode = ProfiLux.GetGetMessageCode(reply);
                if (replyCode != code)
                {
                    throw new ProtocolException(508, string.Format(CultureInfo.CurrentCulture, "Unexpected Message: Wrong Code Expecting {0} Got {1}", code, replyCode));
                }
            }
            else
            {
                throw new ProtocolException(509, "Unknown message type");
            }
        }

        /// <summary>
        /// Verifies the ack packet.
        /// </summary>
        /// <param name="reply">The reply.</param>
        private static void VerifyAckPacket(IList<byte> reply)
        {
            if (reply.Count < 4)
            {
                // strange packet size!
                throw new ProtocolException(510, "Expecting Packet size of at least 4");
            }

            if (reply[4] == ProfiLux.EOT)
            {
                throw new ProtocolException(511, "Unexpected Message: Empty Reply");
            }

            if (reply[4] == ProfiLux.STX)
            {
                if (reply[5] == ProfiLux.NAK)
                {
                    // error message
                    byte errorCode = reply[6];
                    throw new ErrorCodeException(512, string.Format("Error with code {0}", errorCode), errorCode);
                }

                if (reply[5] == ProfiLux.ACK)
                {
                    // this is what we are expecting
                }
                else
                {
                    // should be ok we must now look for the code and verify it
                    int replyCode = ProfiLux.GetGetMessageCode(reply);
                    throw new ProtocolException(513, string.Format(CultureInfo.CurrentCulture, "Unexpected Message: Code {0}", replyCode));
                }
            }
            else
            {
                throw new ProtocolException(514, "Unknown message type");
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Disconnect();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <returns>the packet read</returns>
        /// <exception cref="ConnectionException">If error in writing</exception>
        private Collection<byte> ReadPacket()
        {
            Collection<byte> reply = new Collection<byte>();
            do
            {
                int dataSize;
                byte[] data = this.Connection.Read(256, out dataSize);

                if (dataSize == 0)
                {
                    break;
                }

                for (int i = 0; i < dataSize; i++)
                {
                    reply.Add(data[i]);
                }
            } 
            while (!ProfiLux.AtEndOfPacket(reply));

            return reply;
        }

        #region IBasicProtocol Members


        public bool[] GetDataBoolArray(int code)
        {
            if (!this.IsConnected)
            {
                throw new ProtocolException(503, "Unable to Get Data Not Connected!");
            }

            ProfiLux.SendCommand(code, this.Connection, this.Address);
            Collection<byte> reply;
            while (true)
            {
                reply = this.ReadPacket();
                try
                {
                    VerifyDataPacket(reply, code);
                    break;
                }
                catch (ErrorCodeException)
                {
                    throw;
                }
                catch (ProtocolException ex)
                {
                    Logger.Instance.LogError(ex);
                }
            }

            return ProfiLux.GetMessageBools(reply);
        }

        #endregion

        #region IBasicProtocol Members


        /// <summary>
        /// Gets the data two bite array.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public short[] GetDataTwoByteArray(int code)
        {
            if (!this.IsConnected)
            {
                throw new ProtocolException(503, "Unable to Get Data Not Connected!");
            }

            ProfiLux.SendCommand(code, this.Connection, this.Address);
            Collection<byte> reply;
            while (true)
            {
                reply = this.ReadPacket();
                try
                {
                    VerifyDataPacket(reply, code);
                    break;
                }
                catch (ErrorCodeException)
                {
                    throw;
                }
                catch (ProtocolException ex)
                {
                    Logger.Instance.LogError(ex);
                }
            }

            return ProfiLux.GetMessageDataTwoByteArray(reply);
        }

        #endregion
    }
}

