namespace RedPoint.ReefStatus.Common.UI
{
    using System.Threading;
    using RedPoint.ReefStatus.Common.Database;
    using Settings;

    public class ExportToFile
    {
        /// <summary>
        /// Starts the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="Controller">The Controller.</param>
        public static void Start(string fileName, IProgressCallback callback, int Controller)
        {
            new Thread(() =>
                {
                    try
                    {
                        using (IDataAccess dataAccess = ReefStatusSettings.Instance.Logging.Connection.Create())
                        {
                            dataAccess.Export(fileName, callback, Controller);
                        }
                    }
                    catch (ReefStatusException ex)
                    {
                        Logger.Instance.LogError(ex);
                    }
                    finally
                    {
                        callback.End();
                    }
                }).Start();
        }
    }
}
