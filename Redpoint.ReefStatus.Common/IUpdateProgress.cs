namespace RedPoint.ReefStatus.Common
{
    /// <summary>
    /// The progress Update Interface
    /// </summary>
    public interface IUpdateProgress
    {
        /// <summary>
        /// Gets or sets a value indicating whether [display progress].
        /// </summary>
        /// <value><c>true</c> if [display progress]; otherwise, <c>false</c>.</value>
        bool DisplayProgress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [stop processing].
        /// </summary>
        /// <value><c>true</c> if [stop processing]; otherwise, <c>false</c>.</value>
        bool StopProcessing { get; set; }

        /// <summary>
        /// Sets the progress steps.
        /// </summary>
        /// <param name="steps">The steps.</param>
        void SetProgressSteps(double steps);

        /// <summary>
        /// Gets or sets the progress text.
        /// </summary>
        /// <value>The progress text.</value>
        string ProgressText { get; set; }

        /// <summary>
        /// Increments the progress.
        /// </summary>
        void IncrementProgress();
    }
}
