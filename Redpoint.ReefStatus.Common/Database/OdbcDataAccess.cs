namespace RedPoint.ReefStatus.Common.Database
{
    using System.Data;
    using System.Data.Common;
    using System.Data.Odbc;

    public class OdbcDataAccess : AdoDataAccess, IDataAccess
    {
        public OdbcDataAccess(string dataSource)
        {
            try
            {
                this.Connection = new OdbcConnection(dataSource);
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
        public override void Backup(string databaseConnection, string archiveLocation)
        {
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>Datatabase command</returns>
        protected override DbCommand CreateCommand(string sql)
        {
            return new OdbcCommand(sql, (OdbcConnection)Connection);
        }


        protected override DbParameter CreateParameter(object value, DbType type)
        {
            return new OdbcParameter { Value = value, DbType = type };
        }

        public override string InsertCommand
        {
            get
            {
                return "INSERT INTO LOG (LOG.TIME, LOG.VALUE, TYPE, CONTROLLER) VALUES (?, ?, ?, ?)";
            }
        }
    }
}
