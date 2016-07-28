// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebSession.cs" company="Redpoint Apps">
//   2010
// </copyright>
// <summary>
//   Web session
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.WebServer
{
    using System;

    /// <summary>
    /// Web session
    /// </summary>
    public class WebSession : SessionBase
    {
        /// <summary>
        /// session provider
        /// </summary>
        private static readonly SessionProvider<SessionBase> SessionProvider = new SessionProvider<SessionBase>();

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        /// <value><c>true</c> if this instance is locked; otherwise, <c>false</c>.</value>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns>the current session</returns>
        public static WebSession GetCurrent(string sessionId)
        {
            return
                (WebSession)
                (SessionProvider.GetCurrent(sessionId) ??
                 SessionProvider.Add(
                     new WebSession { SessionId = sessionId, IsLocked = true, AccessedAt = DateTime.Now }));
        }
    }
}
