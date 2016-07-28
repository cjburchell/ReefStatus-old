namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using RedPoint.ReefStatus.Common.Communication;

    public static class ProfiLux
    {
        // ReSharper disable InconsistentNaming
        private const byte SOH = 0x01;
        public const byte STX = 0x02;
        private const byte ENQ = 0x05;
        private const byte ETX = 0x03;
        public const byte EOT = 0x04;
        public const byte ACK = 0x06;
        public const byte NAK = 0x15;

        private const byte DataOffset = 0x30;
        private const byte CodeOffset = 0x40;
        /*
                public const byte CodeOffset2 = 0x60;
                public const byte AddressOffset = 0x50;
         */
        private const byte ChecksomeOffset = 0x20;
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Converts a Collection to byte array.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>list of bytes</returns>
        private static byte[] CollectionToByteArray(ICollection<byte> data)
        {
            var result = new byte[data.Count];
            var index = 0;
            foreach (var b in data)
            {
                result[index] = b;
                index++;
            }

            return result;
        }

        /// <summary>
        /// Calculates the checksum.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>the checksum</returns>
        private static byte CalculateChecksum(IEnumerable<byte> data)
        {
            var dataSum = 0;
            foreach (var b in data)
            {
                dataSum += b;
            }

            var bca = dataSum % 256;
            if (bca < ChecksomeOffset)
            {
                bca = bca + ChecksomeOffset;
            }

            return (byte)bca;
        }

        /// <summary>
        /// Adds the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="command">The command.</param>
        private static void AddData(int data, ICollection<byte> command)
        {
            if (data == 0)
            {
                command.Add(DataOffset); // the value of the data is zero we then add 0
            }
            else
            {
                while (data != 0)
                {
                    var value = (byte)(data & 0xf);
                    data >>= 4;
                    command.Add((byte)(DataOffset | value));
                }
            }
        }

        private static void AddTextData(string data, ICollection<byte> command)
        {
            foreach (var c in data)
            {
                var value1 = (byte)(c & 0xf);
                var value2 = (byte)(c >> 4);
                command.Add((byte)(DataOffset | value1));
                command.Add((byte)(DataOffset | value2));
            }
        }

        /// <summary>
        /// Adds the code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="command">The command.</param>
        private static void AddCode(int code, ICollection<byte> command)
        {
            var codeValue = code;
            while (codeValue != 0)
            {
                var value = (byte)(codeValue & 0xf);
                codeValue >>= 4;
                command.Add((byte)(0x40 + value));
            }
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="address">The address.</param>
        /// <exception cref="ConnectionException">If error in writing</exception>
        public static void SendCommand(int code, IConnection connection, int address)
        {
            var command = new Collection<byte> { SOH, (byte)(0x50 + address), 0x70 };
            command.Add(CalculateChecksum(command)); // BCA
            command.Add(STX); // STX
            AddCode(code, command);
            command.Add(ENQ); // ENQ
            command.Add(ETX); // ETX
            command.Add(CalculateChecksum(command)); // BCC
            command.Add(EOT); // EOT
            command.Add(0xFF); // EOT
            command.Add(0xFF); // EOT

            connection.Write(CollectionToByteArray(command));

            command = new Collection<byte> { SOH, (byte)(0x50 + address), 0x70 };
            command.Add(CalculateChecksum(command)); // BCA
            command.Add(EOT); // EOT
            command.Add(0xFF); // EOT
            command.Add(0xFF); // EOT

            connection.Write(CollectionToByteArray(command));
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="data">The data.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="address">The address.</param>
        /// <exception cref="ConnectionException">If error in writing</exception>
        public static void SendCommand(int code,int data, IConnection connection, int address)
        {
            var command = new Collection<byte> { SOH, (byte)(0x50 + address), 0x70 };
            command.Add(CalculateChecksum(command)); // BCA
            command.Add(STX); // STX
            AddCode(code, command);
            AddData(data, command);
            command.Add(ETX); // ETX
            command.Add(CalculateChecksum(command)); // BCC
            command.Add(EOT); // EOT
            command.Add(0xFF); // EOT
            command.Add(0xFF); // EOT

            connection.Write(CollectionToByteArray(command));

            command = new Collection<byte> { SOH, (byte)(0x50 + address), 0x70 };
            command.Add(CalculateChecksum(command)); // BCA
            command.Add(EOT); // EOT
            command.Add(0xFF); // EOT
            command.Add(0xFF); // EOT

            connection.Write(CollectionToByteArray(command));
        }

        public static void SendTextCommand(int code, string data, IConnection connection, int address)
        {
            var command = new Collection<byte> { SOH, (byte)(0x50 + address), 0x70 };
            command.Add(CalculateChecksum(command)); // BCA
            command.Add(STX); // STX
            AddCode(code, command);
            AddTextData(data, command);
            command.Add(ETX); // ETX
            command.Add(CalculateChecksum(command)); // BCC
            command.Add(EOT); // EOT
            command.Add(0xFF); // EOT
            command.Add(0xFF); // EOT

            connection.Write(CollectionToByteArray(command));

            command = new Collection<byte> { SOH, (byte)(0x50 + address), 0x70 };
            command.Add(CalculateChecksum(command)); // BCA
            command.Add(EOT); // EOT
            command.Add(0xFF); // EOT
            command.Add(0xFF); // EOT

            connection.Write(CollectionToByteArray(command));
        }

        /// <summary>
        /// Ats the end of packet.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <returns>true if it is the end of the packet</returns>
        public static bool AtEndOfPacket(IList<byte> reply)
        {
            if (reply.Count > 3)
            {
                return reply[reply.Count - 3] == EOT
                    && reply[reply.Count - 2] == 0xFF
                    && reply[reply.Count - 1] == 0xFF;
            }

            return false;
        }

        /// <summary>
        /// Gets the get message code.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <returns>The messages code</returns>
        public static int GetGetMessageCode(IList<byte> reply)
        {
            int code = 0;
            int offset = 0;
            for (int i = 5; i < reply.Count; i++)
            {
                if (reply[i] == ETX)
                {
                    break;
                }

                if ((reply[i] & 0xF0) == CodeOffset)
                {
                    int value = reply[i] & 0x0F;
                    value <<= offset;
                    code += value;
                    offset += 4;
                }
            }

            return code;
        }

        /// <summary>
        /// Gets the message string.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <returns>the messages data as a string</returns>
        public static string GetMessageString(IList<byte> reply)
        {
            string data = string.Empty;
            int offset = 0;
            var tempValue = (char)0;
            for (var i = 5; i < reply.Count; i++)
            {
                if (reply[i] == ETX)
                {
                    break;
                }

                if ((reply[i] & 0xF0) == DataOffset)
                {
                    int value = reply[i] & 0xF;
                    value <<= offset;
                    tempValue += (char)value;

                    if (offset == 4)
                    {
                        if (tempValue != 0)
                        {
                            data += tempValue;
                        }

                        offset = 0;
                        tempValue = (char)0;
                    }
                    else
                    {
                        offset = 4;
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Gets the message data short array.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <returns>an array of data</returns>
        public static short[] GetMessageDataShortArray(IList<byte> reply)
        {
            var replyItems = new Collection<short>();
            var offset = 0;
            short tempValue = 0;
            for (var i = 5; i < reply.Count; i++)
            {
                if (reply[i] == ETX)
                {
                    break;
                }

                if ((reply[i] & 0xF0) == ProfiLux.DataOffset)
                {
                    var value = (short)(reply[i] & 0x0F);
                    value <<= offset;
                    tempValue += value;
                    if (offset == 12)
                    {
                        replyItems.Add(tempValue);
                        tempValue = 0;
                        offset = 0;
                    }
                    else
                    {
                        offset += 4;
                    }
                }
            }

            if (offset != 0)
            {
                replyItems.Add(tempValue);
            }

            return CollectionToShortArray(replyItems);
        }

        public static short[] GetMessageDataTwoByteArray(Collection<byte> reply)
        {
            var replyItems = new Collection<short>();
            var offset = 0;
            short tempValue = 0;
            for (var i = 5; i < reply.Count; i++)
            {
                if (reply[i] == ETX)
                {
                    break;
                }

                if ((reply[i] & 0xF0) == DataOffset)
                {
                    var value = (short)(reply[i] & 0x0F);
                    value <<= offset;
                    tempValue += value;
                    if (offset == 4)
                    {
                        replyItems.Add(tempValue);
                        tempValue = 0;
                        offset = 0;
                    }
                    else
                    {
                        offset += 4;
                    }
                }
            }

            if (offset != 0)
            {
                replyItems.Add(tempValue);
            }

            return CollectionToShortArray(replyItems);
        }

        /// <summary>
        /// Gets the message data.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <returns>
        /// the messages data
        /// </returns>
        public static int GetMessageData(IList<byte> reply)
        {
            int data = 0;
            int offset = 0;
            for (int i = 5; i < reply.Count; i++)
            {
                if (reply[i] == ETX)
                {
                    break;
                }

                if ((reply[i] & 0xF0) == DataOffset)
                {
                    int value = reply[i] & 0x0F;
                    value <<= offset;
                    data += value;
                    offset += 4;
                }
            }

            return data;
        }

        /// <summary>
        /// Gets the message bytes.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <returns>the messages data</returns>
        public static byte[] GetMessageBytes(IList<byte> reply)
        {
            var replyItems = new Collection<byte>();
            for (var i = 5; i < reply.Count; i++)
            {
                if (reply[i] == ETX)
                {
                    break;
                }

                if ((reply[i] & 0xF0) == DataOffset)
                {
                    replyItems.Add((byte)(reply[i] & 0x0F));
                }
            }

            return CollectionToByteArray(replyItems);
        }

        /// <summary>
        /// Converts a Collection to byte array.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>list of bytes</returns>
        private static short[] CollectionToShortArray(ICollection<short> data)
        {
            var result = new short[data.Count];
            var index = 0;
            foreach (var b in data)
            {
                result[index] = b;
                index++;
            }

            return result;
        }

        /// <summary>
        /// Gets the message bools.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <returns></returns>
        public static bool[] GetMessageBools(Collection<byte> reply)
        {
            var replyItems = new Collection<bool>();
            for (int i = 5; i < reply.Count; i++)
            {
                if (reply[i] == ETX)
                {
                    break;
                }

                if ((reply[i] & 0xF0) == DataOffset)
                {
                    replyItems.Add((reply[i] & 0x1) != 0);
                    replyItems.Add((reply[i] & 0x2) != 0);
                    replyItems.Add((reply[i] & 0x4) != 0);
                    replyItems.Add((reply[i] & 0x8) != 0);
                }
            }

            return CollectionToBoolArray(replyItems);
        }

        private static bool[] CollectionToBoolArray(Collection<bool> data)
        {
            var result = new bool[data.Count];
            var index = 0;
            foreach (bool b in data)
            {
                result[index] = b;
                index++;
            }

            return result;
        }      
    }

}
