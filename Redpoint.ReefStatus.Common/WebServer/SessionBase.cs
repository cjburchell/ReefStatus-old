// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SessionBase.cs" company="Redpoint Apps">
//   2010
// </copyright>
// <summary>
//   Session Base object
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.WebServer
{
    using System;

    /// <summary>
    /// Session Base object
    /// </summary>
    public class SessionBase
    {
        /// <summary>
        /// Gets or sets the accessed at.
        /// </summary>
        /// <value>The accessed at.</value>
        public DateTime AccessedAt { get; set; }

        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        /// <value>The session id.</value>
        public string SessionId { get; set; }
    }
}
