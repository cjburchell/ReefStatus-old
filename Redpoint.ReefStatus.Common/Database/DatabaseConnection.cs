// <copyright file="DatabaseConnection.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.IO;

    /// <summary>
    /// Database Connection
    /// </summary>
    public class DatabaseConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="type">The type.</param>
        public DatabaseConnection(string connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public string Connection { get; private set; }


        /// <summary>
        /// Creates the specified connection.
        /// </summary>
        /// <returns>The database Connection for the settings</returns>
        public IDataAccess Create()
        {
            try
            {
                return new MemoryDataAccess();
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new DataAccessException(201, "Unable to Connect to Database", ex);
            }
            catch (IOException ex)
            {
                throw new DataAccessException(203, "Unable to Connect to Database Unauthorized Access", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new DataAccessException(202, "Unable to Connect to Database Unauthorized Access", ex);
            }
        }

        internal void Backup(string archiveLocation)
        {
            /*using (IDataAccess dataAccess = this.Create())
            {
                if (dataAccess != null)
                {
                    dataAccess.Backup(Connection, archiveLocation);
                }
            }*/
        }
    }
}
