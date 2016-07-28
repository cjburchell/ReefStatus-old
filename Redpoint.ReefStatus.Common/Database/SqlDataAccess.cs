// <copyright file="SqlDataAccess.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The database access class
    /// </summary>
    internal class SqlDataAccess : LinqDataAccess
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataAccess"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlDataAccess(string connectionString)
        {
            try
            {
                Database = new ReefStatusDatabaseDataContext(connectionString);
                if (!Database.DatabaseExists())
                {
                    // Create new database
                    Database.CreateDatabase();
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new DataAccessException(100,"Unable to open Database : " + connectionString, ex);
            }
        }

        public ReefStatusDatabaseDataContext Database { get; private set; }

        protected override IQueryable<DataLog> DataLogs { get { return this.Database.DataLogs; } }
        protected override IQueryable<DataType> DataTypes { get { return this.Database.DataTypes; } }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void OnDispose()
        {
            GC.SuppressFinalize(this);
            if (Database != null)
            {
                Database.Dispose();
            }
        }

        protected override void Insert(DataType dataType)
        {
            Database.DataTypes.InsertOnSubmit(dataType);
            Database.SubmitChanges();
        }

        protected override void DeleteAll(IEnumerable<DataLog> logs)
        {
            Database.DataLogs.DeleteAllOnSubmit(logs);
            Database.SubmitChanges();
        }

        protected override void SubmitChanges()
        {
            Database.SubmitChanges();
        }

        protected override void Insert(DataLog log)
        {
            Database.DataLogs.InsertOnSubmit(log);
            Database.SubmitChanges();
        }

        protected override void InserAll(System.Collections.ObjectModel.Collection<RedPoint.ReefStatus.Common.Database.DataLog> log)
        {
            Database.DataLogs.InsertAllOnSubmit(log);
            Database.SubmitChanges();
        }
    }
}
