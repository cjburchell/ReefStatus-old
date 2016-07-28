namespace RedPoint.ReefStatus.Common.UI
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using RedPoint.ReefStatus.Common.Core;
    using RedPoint.ReefStatus.Common.Database;
    using Settings;

    public class ConvertDatabase
    {
        private readonly DatabaseConnection oldDatabaseLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertDatabase"/> class.
        /// </summary>
        /// <param name="oldDatabaseLocation">The old database location.</param>
        public ConvertDatabase(DatabaseConnection oldDatabaseLocation)
        {
            this.oldDatabaseLocation = oldDatabaseLocation;
        }

        /// <summary>
        /// Starts the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void Start(IProgressCallback callback)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(this.ConvertDatabaseThread, callback);
        }

        /// <summary>
        /// Setups the database thread.
        /// </summary>
        /// <param name="status">The status.</param>
        private void ConvertDatabaseThread(object status)
        {
            IProgressCallback callback = status as IProgressCallback;
            if (callback != null)
            {
                lock (callback.Lock)
                {
                    try
                    {
                        callback.Begin(0, 2, Language.GetResource("strImportFromDatabase"));

                        using (IDataAccess oldData = this.oldDatabaseLocation.Create())
                        {
                            using (IDataAccess dataAccess = ReefStatusSettings.Instance.Logging.Connection.Create())
                            {
                                Dictionary<int, string> types = oldData.GetTypes();
                                callback.SetRange(0, types.Count);
                                int count = 0;
                                foreach (int typeIndex in types.Keys)
                                {
                                    if (callback.IsAborting)
                                    {
                                        return;
                                    }

                                    callback.SetText(Language.GetResource("strConvertingTable") + types[typeIndex]);
                                    int newTypeIndex = dataAccess.GetTypeIndex(types[typeIndex]);
                                    Collection<DataLog> dataPoints = oldData.GetDataPoints(typeIndex, newTypeIndex, callback);

                                    if (callback.IsAborting)
                                    {
                                        return;
                                    }

                                    dataAccess.InsertItems(dataPoints);
                                    callback.StepTo(count++);
                                }
                            }
                        }
                    }
                    catch (ReefStatusException ex)
                    {
                        Logger.Instance.LogError(ex);
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                        // We want to exit gracefully here (if we're lucky)
                    }
                    catch (System.Threading.ThreadInterruptedException)
                    {
                        // And here, if we can
                    }
                    finally
                    {
                        callback.End();
                    }
                }
            }
        }
    }
}
