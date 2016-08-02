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
    public class DatabaseConnectionFactory
    {
        /// <summary>
        /// Creates the specified connection.
        /// </summary>
        /// <returns>The database Connection for the settings</returns>
        public static IDataAccess Create()
        {
            try
            {
                return new CouchDataAccess();
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
    }
}
