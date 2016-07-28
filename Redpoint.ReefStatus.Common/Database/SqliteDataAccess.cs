// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqliteDataAccess.cs" company="Redpoint Apps">
//   2010
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// The sqlite data access.
    /// </summary>
    public class SqliteDataAccess : AdoDataAccess, IDataAccess
    {
        #region Constants and Fields

        /// <summary>
        /// Gets SqlAverageCommand.
        /// </summary>
        protected string SqlAverageCommand
        {
            get
            {
                return "SELECT AVG(VALUE) FROM LOG WHERE TYPE=? AND TIME<=? AND TIME>? AND CONTROLLER=?";
            }
        }

        /// <summary>
        /// Gets SqlCountCommand.
        /// </summary>
        protected override string SqlCountCommand
        {
            get
            {
                return "SELECT COUNT(*) FROM LOG";
            }
        }

        /// <summary>
        /// Gets SqlDeleteItemCommand.
        /// </summary>
        protected override string SqlDeleteItemCommand
        {
            get
            {
                return "DELETE FROM LOG WHERE LogIndex=?";
            }
        }

        /// <summary>
        /// Gets SqlDeleteItemCommand.
        /// </summary>
        protected override string SqlDeleteLastItemCommand
        {
            get
            {
                return "DELETE FROM LOG WHERE LogIndex = (SELECT MAX(LogIndex) FROM LOG WHERE TYPE=? AND CONTROLLER=?);";
            }
        }

        /// <summary>
        /// Gets SqlGetDataPointsCommand.
        /// </summary>
        protected override string SqlGetDataPointsCommand
        {
            get
            {
                return "SELECT TIME, VALUE FROM LOG WHERE TYPE=? AND CONTROLLER=?";
            }
        }

        /// <summary>
        /// Gets SqlGetDataPointsCommand2.
        /// </summary>
        protected override string SqlGetDataPointsCommand2
        {
            get
            {
                return
                    "SELECT TIME, VALUE, LogIndex FROM LOG WHERE TYPE=? AND TIME>? AND CONTROLLER=? ORDER BY TIME ";
            }
        }

        /// <summary>
        /// Gets SqlGetDataPointsCommand3.
        /// </summary>
        protected override string SqlGetDataPointsCommand3
        {
            get
            {
                return "SELECT TIME, VALUE FROM LOG WHERE TYPE=? AND CONTROLLER=? ORDER BY TIME DESC";
            }
        }

        /// <summary>
        /// Gets SqlGetDataPointsCommand4.
        /// </summary>
        protected override string SqlGetDataPointsCommand4
        {
            get
            {
                return
                    "SELECT TIME, VALUE, LogIndex FROM LOG WHERE TYPE=? AND TIME>? AND TIME<=? AND CONTROLLER=? ORDER BY TIME DESC";
            }
        }

        /// <summary>
        /// Gets SqlGetDataPointsCommand5.
        /// </summary>
        protected override string SqlGetDataPointsCommand5
        {
            get
            {
                return "SELECT VALUE, TYPE FROM LOG WHERE TIME = ? AND CONTROLLER=? ";
            }
        }

        /// <summary>
        /// Gets SqlGetTypesCommand.
        /// </summary>
        protected override string SqlGetTypesCommand
        {
            get
            {
                return "SELECT * FROM TYPES";
            }
        }

        /// <summary>
        /// Gets SqlInsertTypeCommand.
        /// </summary>
        protected override string SqlInsertTypeCommand
        {
            get
            {
                return "INSERT INTO TYPES (TYPE) VALUES (?)";
            }
        }

        /// <summary>
        /// Gets SqlRemoveDataSetCommand.
        /// </summary>
        protected override string SqlRemoveDataSetCommand
        {
            get
            {
                return "DELETE FROM LOG WHERE TYPE=? AND CONTROLLER=?";
            }
        }

        /// <summary>
        /// Gets SqlRemoveLogsCommand.
        /// </summary>
        protected override string SqlRemoveLogsCommand
        {
            get
            {
                return "DELETE FROM LOG WHERE TIME<?";
            }
        }

        /// <summary>
        /// Gets SqlTimesCommand.
        /// </summary>
        protected override string SqlTimesCommand
        {
            get
            {
                return "SELECT DISTINCT TIME FROM LOG ORDER BY TIME DESC";
            }
        }



        protected override string TypeIndex
        {
            get
            {
                return "TypeIndex";
            }
        }

        protected override string LogIndex
        {
            get
            {
                return "LogIndex";
            }
        }


        /// <summary>
        /// The create log table command.
        /// </summary>
        private const string CreateLogTableCommand =
            @"CREATE TABLE LOG(
  LogIndex INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  TIME DATETIME NOT NULL,
  VALUE DOUBLE NOT NULL,
  TYPE INTEGER NOT NULL,
  CONTROLLER INTEGER NOT NULL)";

        /// <summary>
        /// The create type table command.
        /// </summary>
        private const string CreateTypeTableCommand =
            @"CREATE TABLE TYPES (
  TypeIndex INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  TYPE VARCHAR(45) NOT NULL)";

        #endregion

        /// <summary>
        /// Gets SqlGetTypeIndexCommand.
        /// </summary>
        protected override string SqlGetTypeIndexCommand
        {
            get
            {
                return "SELECT TypeIndex FROM TYPES WHERE TYPE=?";
            }
        }

        /// <summary>
        /// Gets the SQL get data points command1.
        /// </summary>
        /// <value>The SQL get data points command1.</value>
        protected override string SqlGetDataPointsCommand1
        {
            get
            {
                return "SELECT TIME, VALUE, LogIndex FROM LOG WHERE TYPE=? AND CONTROLLER=? ORDER BY TIME ";
            }
        }

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteDataAccess"/> class.
        /// </summary>
        /// <param name="dataSource">
        /// The data source.
        /// </param>
        /// <exception cref="DataAccessException">
        /// </exception>
        /// <exception cref="DataAccessException">
        /// </exception>
        public SqliteDataAccess(string dataSource)
        {
            try
            {
                if (!File.Exists(dataSource))
                {
                    try
                    {
                        SQLiteConnection.CreateFile(dataSource);
                        CreateDatabase(dataSource);
                    }
                    catch (DbException ex)
                    {
                        throw new DataAccessException(200, "Unable to create new database file " + dataSource, ex);
                    }
                }

                this.Connection =
                    new SQLiteConnection(
                        (new SQLiteConnectionStringBuilder { DataSource = dataSource }).ConnectionString);
                this.Connection.Open();
            }
            catch (DbException ex)
            {
                throw new DataAccessException(202, "Unable to open Database : " + dataSource, ex);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets InsertCommand.
        /// </summary>
        public override string InsertCommand
        {
            get
            {
                return "INSERT INTO LOG (TIME, VALUE, TYPE, CONTROLLER) VALUES (?, ?, ?, ?)";
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDataAccess

        /// <summary>
        /// The backup.
        /// </summary>
        /// <param name="databaseFile">
        /// The database file.
        /// </param>
        /// <param name="archiveLocation">
        /// The archive location.
        /// </param>
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

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="sql">
        /// The SQL.
        /// </param>
        /// <returns>
        /// Datatabase command
        /// </returns>
        protected override DbCommand CreateCommand(string sql)
        {
            return new SQLiteCommand(sql, (SQLiteConnection)this.Connection);
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// Database Parameter
        /// </returns>
        protected override DbParameter CreateParameter(object value, DbType type)
        {
            return new SQLiteParameter { Value = value, DbType = type };
        }

        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <param name="dataSource">
        /// The data source.
        /// </param>
        private static void CreateDatabase(string dataSource)
        {
            using (
                var connection =
                    new SQLiteConnection(
                        (new SQLiteConnectionStringBuilder { DataSource = dataSource }).ConnectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand(CreateLogTableCommand, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(CreateTypeTableCommand, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        #endregion
    }
}