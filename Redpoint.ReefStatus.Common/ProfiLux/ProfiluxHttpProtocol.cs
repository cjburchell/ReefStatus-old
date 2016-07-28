// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfiluxHttpProtocol.cs" company="RedpointGames">
//   2009
// </copyright>
// <summary>
//   Defines the ProfiluxHttpProtocol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading;

    /// <summary>
    /// The profilux http protocol.
    /// </summary>
    public class ProfiluxHttpProtocol : BasicProtocol, IBasicProtocol
    {
        /// <summary>
        /// Maximum number of retries
        /// </summary>
        private const int MaxAttempts = 20;

        /// <summary>
        /// The address.
        /// </summary>
        private readonly string address;

        /// <summary>
        /// The client.
        /// </summary>
        //private readonly WebClient client = new WebClient();

        /// <summary>
        /// The port.
        /// </summary>
        private readonly int port;

        /// <summary>
        /// The password
        /// </summary>
        private readonly string password;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfiluxHttpProtocol"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="password">The password.</param>
        public ProfiluxHttpProtocol(string address, int port, string password)
        {
            //this.client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            this.address = address;
            this.port = port;
            this.password = password;

            if (this.Login() != AccessRights.Full)
            {
                throw new ProtocolException(6111, "Unable to login to controler Reef Status needs full access rights");
            }

            this.Version = this.GetData(0);
        }

        /// <summary>
        /// The Access rights
        /// </summary>
        private enum AccessRights
        {
            /// <summary>
            /// We have no right
            /// </summary>
            NoRights = 0,

            /// <summary>
            /// Read Only
            /// </summary>
            ReadOnly = 1,

            /// <summary>
            /// Full Access
            /// </summary>
            Full = 2,
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The get data.</returns>
        public int GetData(int code)
        {
            string reply = this.GetRawData(code);
            try
            {
                return int.Parse(reply);
            }
            catch (FormatException ex)
            {
                throw new ProtocolException(6001, "Unable to convert", ex);
            }
        }

        /// <summary>
        /// Gets the data text.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The get data text.</returns>
        public string GetDataText(int code)
        {
            int reply = this.GetData(code);
            string data = string.Empty;

            while (reply != 0)
            {
                char value = (char)(reply & 0xFF);
                reply >>= sizeof(char) * 4;
                data += value;
            }

            return data;
        }

        /// <summary>
        /// Gets the data short array.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>short array</returns>
        public short[] GetDataShortArray(int code)
        {
            string reply = this.GetRawData(code);

            long value = long.Parse(reply);

            Collection<short> values = new Collection<short>();

            while (value != 0)
            {
                values.Add((short)(value & 0x1FF));
                value >>= 9;
            }

            short[] valueArray = new short[values.Count];
            int i = 0;
            foreach (short val in values)
            {
                valueArray[i] = val;
                i++;
            }

            return valueArray;
        }

#if PocketPC
        private class UriBuilder
        {
            private string protocol;
            private string address;
            private int port;
            private string path;

            public UriBuilder(string protocol, string address, int port, string path)
            {
                this.protocol = protocol;
                this.address = address;
                this.port = port;
                this.path = path;
            }

            public string Uri
            {
                get
                {
                    if(string.IsNullOrEmpty(Query))
                    {
                        if (port == 80)
                        {
                            return protocol + "://" + address + "/" + path;
                        }
                        return this.protocol + "://" + this.address + ":" + this.port + "/" + this.path;
                    }
                    if (port == 80)
                    {
                        return this.protocol + "://" + this.address + "/" + this.path + "?" + this.Query;
                    }
                    return this.protocol + "://" + this.address + ":" + this.port + "/" + this.path + "?" + this.Query;
                }
            }

            public string Query
            {
                get; set;
            }
        }
#endif

        /// <summary>
        /// Sends the data.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="data">The data.</param>
        public void SendData(int code, int data)
        {
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                try
                {
                    UriBuilder uri = new UriBuilder("http", this.address, this.port, "communication.php")
                                         {
                                             Query = string.Format("dir=sel&code={0}&data={1}", code, data)
                                         };

                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(uri.Uri);
                    httpRequest.Credentials = CredentialCache.DefaultCredentials;

                    using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                    {
                        using (Stream stream = httpResponse.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string reply = reader.ReadToEnd();
                                reader.Close();

                                if (reply == "Access Denied")
                                {
                                    if (this.Login() != AccessRights.Full)
                                    {
                                        throw new ProtocolException(6120,
                                                                    "Unable to login to controler Reef Status needs full access rights");
                                    }

                                    continue;
                                }

                                string command = GetCommandFrom(reply);
                                string dataParam = GetDataFrom(reply);

                                if (command != code.ToString())
                                {
                                    throw new ProtocolException(6112, "Unexpected comand reply: " +  command);
                                }

                                if (dataParam.StartsWith("NACK"))
                                {
                                    throw new ProtocolException(6105, "Error in command " + code);
                                }

                                if (!dataParam.StartsWith("ACK"))
                                {
                                    throw new ProtocolException(6106, "Unexpected message: Missing ACK");
                                }

                                return; // command sent!
                            }
                        }
                    }
                }
                catch (WebException e)
                {
                    if (!e.Message.Contains("503"))
                    {
                        throw new ProtocolException(6103, "Unable to send command " + code, e);
                    }

                    Thread.Sleep(100 * attempt);
                }
            }

            throw new ProtocolException(6103, "Unable to send command " + code);
        }

        /// <summary>
        /// Gets the data from.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <returns></returns>
        private static string GetDataFrom(string reply)
        {
#if PocketPC
            string[] parameters = reply.Split('&');
            string dataParameter = reply.Substring(parameters[0].Length + 1);
            string[] items = dataParameter.Split('=');
            return dataParameter.Substring(items[0].Length + 1);
#else
            string[] parameters = reply.Split(new[] { '&' }, 2);
            return parameters[1].Split(new[] { '=' }, 2)[1];
#endif
        }

        /// <summary>
        /// Gets the command from a reply.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <returns>The command</returns>
        private static string GetCommandFrom(string reply)
        {
            return reply.Split('&')[0].Split('=')[1];
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            ////this.LogOut();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Logins the specified user name.
        /// </summary>
        /// <returns>the Access Rights</returns>
        private AccessRights Login()
        {
            const string UserName = "Admin";
            UriBuilder uri = new UriBuilder("http", this.address, this.port, "security.php")
                                 {
                                     Query =
                                         string.Format("action=login&name={0}&pass={1}", UserName, this.password)
                                 };

            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                try
                {
                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(uri.Uri);
                    httpRequest.Credentials = CredentialCache.DefaultCredentials;

                    using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                    {
                        using (Stream stream = httpResponse.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string reply = reader.ReadToEnd();
                                return (AccessRights) int.Parse(reply);
                            }
                        }
                    }
                }
                catch (WebException e)
                {
                    if (!e.Message.Contains("503"))
                    {
                        throw new ProtocolException(6110, "Unable to login using " + UserName, e);
                    }

                    Thread.Sleep(100 * attempt);
                }
            }

            throw new ProtocolException(6110, "Unable to login using " + UserName);
        }

        /// <summary>
        /// Logins the specified user name.
        /// </summary>
        private void LogOut()
        {
            UriBuilder uri = new UriBuilder("http", this.address, this.port, "logout.html");

            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                try
                {
                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(uri.Uri);
                    httpRequest.Credentials = CredentialCache.DefaultCredentials;

                    using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                    {
                        using (Stream data = httpResponse.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(data))
                            {
                                reader.ReadToEnd();
                                return;
                            }
                        }
                    }
                }
                catch (WebException e)
                {
                    if (!e.Message.Contains("503"))
                    {
                        throw new ProtocolException(6115, "Unable to logout", e);
                    }

                    Thread.Sleep(100 * attempt);
                }
            }

            throw new ProtocolException(6115, "Unable to logout");
        }

        /// <summary>
        /// Gets the raw data.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The get raw data.</returns>
        private string GetRawData(int code)
        {
            UriBuilder uri = new UriBuilder("http", this.address, this.port, "communication.php")
                                 {
                                     Query =
                                         string.Format("dir=enq&code={0}", code)
                                 };

            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                try
                {
                    string rawData = string.Empty;
                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(uri.Uri);
                    httpRequest.Credentials = CredentialCache.DefaultCredentials;

                    using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                    {
                        using (Stream data = httpResponse.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(data))
                            {
                                string reply = reader.ReadToEnd();
                                reader.Close();
                                if (reply == "Access Denied")
                                {
                                    if (this.Login() != AccessRights.Full)
                                    {
                                        throw new ProtocolException(6120,
                                                                    "Unable to login to controler Reef Status needs full access rights");
                                    }

                                    continue;
                                }

                                try
                                {
                                    string command = GetCommandFrom(reply);
                                    rawData = GetDataFrom(reply);

                                    if (command != code.ToString())
                                    {
                                        throw new ProtocolException(6112, "Unexpected comand reply: " + command);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Trace.WriteLine(e);
                                }

                                if (rawData.StartsWith("NACK"))
                                {
                                    throw new ProtocolException(6105, "Error in command " + code);
                                }

                                if (rawData.StartsWith("ACK"))
                                {
                                    throw new ProtocolException(6106, "Unexpected message: ACK");
                                }
                            }

                            data.Close();
                        }
                    }

                    ////Trace.WriteLine("Command " + code + " Attempt:" + attempt);
                    return rawData;
                }
                catch (WebException e)
                {
                    if (!e.Message.Contains("503"))
                    {
                        throw new ProtocolException(6104, "Unable to get data for " + code, e);
                    }

                    Thread.Sleep(100 * attempt);
                }
            }

            throw new ProtocolException(6104, "Unable to get data for " + code);
        }

        #region IBasicProtocol Members


        public byte[] GetDataByteArray(int code)
        {
            string reply = this.GetRawData(code);

            long value = long.Parse(reply);

            Collection<byte> values = new Collection<byte>();

            while (value != 0)
            {
                values.Add((byte)(value & 0x00F));
                value >>= 4;
            }

            byte [] valueArray = new byte[values.Count];
            int i = 0;
            foreach (byte val in values)
            {
                valueArray[i] = val;
                i++;
            }

            return valueArray;
        }



        #endregion

        #region IBasicProtocol Members


        public bool[] GetDataBoolArray(int code)
        {
            return new bool[0];
        }

        #endregion

        #region IBasicProtocol Members


        public short[] GetDataTwoByteArray(int code)
        {
            return new short[0];
        }

        #endregion
    }
}