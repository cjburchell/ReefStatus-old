namespace RedPoint.ReefStatus.Common.UI
{
    using System.Threading;
    using RedPoint.ReefStatus.Common.Database;
    using Settings;

    public class ImportFromFile
    {
        /// <summary>
        /// Starts the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="progress">The progress.</param>
        public static void Start(string fileName, IProgressCallback progress, int Controller)
        {
            new Thread(() =>
            {
                try
                {
                    using (IDataAccess dataAccess = ReefStatusSettings.Instance.Logging.Connection.Create())
                    {
                        dataAccess.Import(fileName, progress, Controller);
                    }
                }
                catch (ReefStatusException ex)
                {
                    Logger.Instance.LogError(ex);
                }
                finally
                {
                    progress.End();
                }
            }).Start();
        }
    }
}
