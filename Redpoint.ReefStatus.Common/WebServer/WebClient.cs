namespace RedPoint.ReefStatus.Common.WebServer
{
    using System;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Xml.Serialization;

    public class WebClient
    {
        /// <summary>
        /// The number of retry attempts
        /// </summary>
        private const int MaxAttempts = 5;

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; }

        /// <summary>
        /// Gets the view string.
        /// </summary>
        /// <param name="Controller">The Controller.</param>
        /// <returns>the current view string</returns>
        public string[] GetViewString(int Controller)
        {
            string rawData = this.GetRawXmlData("viewstring", "Controller=" + Controller);
            
            if (string.IsNullOrEmpty(rawData))
            {
                throw new ReefStatusException(6104, "Unable to View String");
            }

            string[] data;
            using (TextReader reader = new StringReader(rawData))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(string[]));
                data = (string[])serializer.Deserialize(reader);
                reader.Close();
            }

            return data;
        }

        /// <summary>
        /// Gets the display string.
        /// </summary>
        /// <param name="Controller">The Controller.</param>
        /// <returns>the current display string</returns>
        public string[] GetDisplayString(int Controller)
        {
            string rawData = this.GetRawXmlData("display", "output=xml&Controller=" + Controller);
            if (string.IsNullOrEmpty(rawData))
            {
                throw new ReefStatusException(6104, "Unable to Display String");
            }

            string[] data;
            using (TextReader reader = new StringReader(rawData))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(string[]));
                data = (string[])serializer.Deserialize(reader);
                reader.Close();
            }

            return data;
        }

        /// <summary>
        /// Gets the item data.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="Controller">The Controller.</param>
        /// <returns>A list of data items</returns>
        public Collection<WebData> GetItemData(string type, int Controller)
        {
            string rawData = this.GetRawXmlData(type, "Controller=" + Controller);
            if (string.IsNullOrEmpty(rawData))
            {
                throw new ReefStatusException(6104, "Unable to Item Data");
            }

            Collection<WebData> data;
            using (TextReader reader = new StringReader(rawData))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Collection<WebData>), new[] { typeof(WebData) });
                data = (Collection<WebData>)serializer.Deserialize(reader);
                reader.Close();
            }

            return data;
        }

        /// <summary>
        /// Gets the Controller info.
        /// </summary>
        /// <param name="Controller">The Controller.</param>
        /// <returns>The Controller Info</returns>
        public ControllerInfo GetControllerInfo(int Controller)
        {
            string rawData = this.GetRawXmlData("info", "Controller=" + Controller);
            if (string.IsNullOrEmpty(rawData))
            {
                throw new ReefStatusException(6104, "Unable to get Info");
            }

            ControllerInfo data;
            using (TextReader reader = new StringReader(rawData))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ControllerInfo));
                data = (ControllerInfo)serializer.Deserialize(reader);
                reader.Close();
            }

            return data;
        }

        /// <summary>
        /// Graphes the specified Controller.
        /// </summary>
        /// <param name="Controller">The Controller.</param>
        /// <param name="id">The id.</param>
        /// <param name="range">The range.</param>
        /// <returns>An Image</returns>
        public Bitmap Graph(int Controller, string id, string range)
        {
            return this.GetImage("graph", string.Format("Controller={0}&type={1}&range={2}", Controller, id, range));                
        }

        /// <summary>
        /// Sends the status email.
        /// </summary>
        /// <param name="message">The message.</param>
        public void SendStatusEmail(string message)
        {
            this.GetRawXmlData("statusemail", "message=" + message);
        }

        /// <summary>
        /// Gets the raw data.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="paramaters">The paramaters.</param>
        /// <returns>The get raw data.</returns>
        private string GetRawXmlData(string command, string paramaters)
        {
            UriBuilder uri = new UriBuilder("http", this.Address, this.Port, "command/" + command + "/");

            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                try
                {
                    string rawData;
                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(uri.Uri + paramaters);
                    httpRequest.Credentials = CredentialCache.DefaultCredentials;

                    using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                    {
                        using (Stream data = httpResponse.GetResponseStream())
                        {
                            if (data == null)
                            {
                                continue;
                            }

                            using (StreamReader reader = new StreamReader(data))
                            {
                                rawData = reader.ReadToEnd();
                                reader.Close();
                            }

                            data.Close();
                        }
                    }

                    return rawData;
                }
                catch (WebException e)
                {
                    if (e.InnerException is SocketException)
                    {
                        if (((SocketException)e.InnerException).ErrorCode == 10061)
                        {
                            throw new ReefStatusException(6145, "Unable to Connect to Web Server", e);
                        }
                    }

                    if (!e.Message.Contains("503"))
                    {
                        throw new ReefStatusException(6104, "Unable to get data for " + command, e);
                    }

                    Thread.Sleep(100 * attempt);
                }
            }

            throw new ReefStatusException(6104, "Unable to get data for " + command);
        }

        /// <summary>
        /// Gets the raw data.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="paramaters">The paramaters.</param>
        /// <returns>The get raw data.</returns>
        private Bitmap GetImage(string command, string paramaters)
        {
            UriBuilder uri = new UriBuilder("http", this.Address, this.Port, "command/" + command + "/");

            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                try
                {
                    Bitmap image;
                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(uri.Uri + paramaters);
                    httpRequest.Credentials = CredentialCache.DefaultCredentials;

                    using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                    {
                        using (Stream data = httpResponse.GetResponseStream())
                        {
                            if (data == null)
                            {
                                continue;
                            }

                            image = new Bitmap(data);
                            data.Close();
                        }
                    }

                    return image;
                }
                catch (WebException e)
                {
                    if (!e.Message.Contains("503"))
                    {
                        throw new ReefStatusException(6104, "Unable to get data for " + command, e);
                    }

                    Thread.Sleep(100 * attempt);
                }
            }

            throw new ReefStatusException(6104, "Unable to get data for " + command);
        }

        /// <summary>
        /// Unlocks the specified password.
        /// </summary>
        /// <param name="password">The password.</param>
        public void Unlock(string password)
        {
            this.GetRawXmlData("unlock", "password=" + password);
        }
    }
}
