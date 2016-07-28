// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SessionProvider.cs" company="Redpoint Apps">
//   2010
// </copyright>
// <summary>
//   Session provider
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.WebServer
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Session provider
    /// </summary>
    /// <typeparam name="T">Session type</typeparam>
    public class SessionProvider<T> where T : SessionBase, new()
    {
        /// <summary>
        /// Session list
        /// </summary>
        private readonly Collection<T> sessionList = new Collection<T>();

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns>the current session or NULL if there is no session active</returns>
        internal T GetCurrent(string sessionId)
        {
            Trace.WriteLine("Access Session " + sessionId);
            T session = this.sessionList.FirstOrDefault(item => item.SessionId == sessionId);

            if (session != null)
            {
                DateTime timeout = session.AccessedAt + new TimeSpan(0, 0, 3, 0);
                if (timeout > DateTime.Now)
                {
                    session.AccessedAt = DateTime.Now;
                }
                else
                {
                    this.sessionList.Remove(session);
                    session = null;
                }
            }

            return session;
        }

        /// <summary>
        /// Adds the specified session.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>the newly created session</returns>
        internal T Add(T session)
        {
            this.sessionList.Add(session);
            Trace.WriteLine("Create Session " + session.SessionId);
            return session;
        }
    }
}
