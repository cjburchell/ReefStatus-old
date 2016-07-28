// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncProfiluxProtocol.cs" company="RedpointGames">
//   2009
// </copyright>
// <summary>
//   Defines the AsyncProfiluxProtocol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using RedPoint.ReefStatus.Common.Communication;

    public class AsyncProfiluxProtocol
    {
        public int Address { get; set; }
        public int Pin { get; set; }

        private int version;

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public IConnection Connection { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get { return Connection != null; }
        }

        public void Connect()
        {
            Enq(0);
        }

        private void Enq(int code)
        {
            ProfiLux.SendCommand(code, Connection, Address);
        }

        private void Sel(int code, int data)
        {
            ProfiLux.SendCommand(code, data, Connection, Address);
        }
    }
}
