namespace ReefStatusServer
{
    using log4net;

    using RedPoint.ReefStatus.Common;

    public class Progress : IUpdateProgress
    {
        private string progressText;

        private readonly ILog log = LogManager.GetLogger("ReefStatus");

        /// <summary>
        /// Gets or sets a value indicating whether [display progress].
        /// </summary>
        /// <value><c>true</c> if [display progress]; otherwise, <c>false</c>.</value>
        public bool DisplayProgress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [stop processing].
        /// </summary>
        /// <value><c>true</c> if [stop processing]; otherwise, <c>false</c>.</value>
        public bool StopProcessing { get; set; }

        /// <summary>
        /// Sets the progress steps.
        /// </summary>
        /// <param name="steps">The steps.</param>
        public void SetProgressSteps(double steps)
        {
        }

        /// <summary>
        /// Gets or sets the progress text.
        /// </summary>
        /// <value>The progress text.</value>
        public string ProgressText
        {
            get
            {
                return this.progressText;
            }

            set
            {
                this.progressText = value;

                if (string.IsNullOrEmpty(this.progressText))
                {
                    return;
                }

                this.log.Info(this.progressText);
            }
        }

        /// <summary>
        /// Increments the progress.
        /// </summary>
        public void IncrementProgress()
        {
        }
    }
}
