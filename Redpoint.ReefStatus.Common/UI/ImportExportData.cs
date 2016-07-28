namespace RedPoint.ReefStatus.Common.UI
{
    /// <summary>
    /// Inport and Exportable Data
    /// </summary>
    internal class ImportExportData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportExportData"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="callback">The callback.</param>
        public ImportExportData(string filename, IProgressCallback callback)
        {
            FileName = filename;
            Callback = callback;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { private set; get; }

        /// <summary>
        /// Gets the callback.
        /// </summary>
        /// <value>The callback.</value>
        public IProgressCallback Callback { private set; get; }
    }
}
