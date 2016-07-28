// <copyright file="OleDataAccess.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// The database access class
    /// </summary>
    internal class OleDataAccess : AdoDataAccess, IDataAccess
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OleDataAccess"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        public OleDataAccess(string dataSource)
        {
            try
            {
                    OleDbConnectionStringBuilder connectionString = new OleDbConnectionStringBuilder
                                                                        {
                                                                            Provider = "Microsoft.Jet.OLEDB.4.0",
                                                                            DataSource = dataSource
                                                                        };

                    if (!File.Exists(dataSource))
                    {
                        try
                        {
                            File.Copy("ReefStatus.mdb", dataSource);
                        }
                        catch (System.IO.IOException ex)
                        {
                            throw new DataAccessException(200, "Unable to create new database file " + dataSource, ex);
                        }
                    }

                    this.Connection = new OleDbConnection(connectionString.ConnectionString);
                

                this.Connection.Open();
            }
            catch (DbException ex)
            {
                throw new DataAccessException(202, "Unable to open Database : " + dataSource, ex);
            }
        }

        /// <summary>
        /// Backups the specified database connection.
        /// </summary>
        /// <param name="databaseConnection">The database connection.</param>
        /// <param name="archiveLocation">The archive location.</param>
        public override void Backup(string databaseFile, string archiveLocation)
        {
            if (!string.IsNullOrEmpty(databaseFile))
            {
                if (this.Connection != null)
                {
                    this.Connection.Close();
                    this.Connection = null;
                }

                File.Copy(
                    databaseFile,
                    archiveLocation +
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "\\{0}_{1}_{2}.mdb",
                        DateTime.Now.Year,
                        DateTime.Now.Month,
                        DateTime.Now.Day));
            }
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>Datatabase command</returns>
        protected override DbCommand CreateCommand(string sql)
        {
            return new OleDbCommand(sql, (OleDbConnection)Connection);
        }


        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>Database Parameter</returns>
        protected override DbParameter CreateParameter(object value, DbType type)
        {
            return new OleDbParameter { Value = value, DbType = type };
        }

        public override string InsertCommand
        {
            get
            {
                return "INSERT INTO LOG ([TIME], [VALUE], TYPE, CONTROLLER) VALUES (?, ?, ?, ?)";
            }
        }
    }
}
