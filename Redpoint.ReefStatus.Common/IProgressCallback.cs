// <copyright file="IProgressCallback.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common
{
    /// <summary>
    /// This defines an interface which can be implemented by UI elements
    /// which indicate the progress of a long operation.
    /// (See ProgressWindow for a typical implementation)
    /// </summary>
    public interface IProgressCallback
    {
        /// <summary>
        /// Gets a value indicating whether this instance is aborting.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is aborting; otherwise, <c>false</c>.
        /// </value>
        bool IsAborting { get; }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        object Tag { get; }

        /// <summary>
        /// Call this method from the worker thread to initialize
        /// the progress callback.
        /// </summary>
        /// <param name="minimum">The minimum value in the progress range (e.g. 0)</param>
        /// <param name="maximum">The maximum value in the progress range (e.g. 100)</param>
        void Begin(int minimum, int maximum, string title);

        /// <summary>
        /// Call this method from the worker thread to initialize
        /// the progress callback, without setting the range
        /// </summary>
        void Begin(string title);

        /// <summary>
        /// Call this method from the worker thread to reset the range in the progress callback
        /// </summary>
        /// <param name="minimum">The minimum value in the progress range (e.g. 0)</param>
        /// <param name="maximum">The maximum value in the progress range (e.g. 100)</param>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        void SetRange(int minimum, int maximum);

        /// <summary>
        /// Call this method from the worker thread to update the progress text.
        /// </summary>
        /// <param name="text">The progress text to display</param>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        void SetText(string text);

        /// <summary>
        /// Call this method from the worker thread to increase the progress counter by a specified value.
        /// </summary>
        /// <param name="value">The amount by which to increment the progress indicator</param>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        void StepTo(int value);

        /// <summary>
        /// Call this method from the worker thread to step the progress meter to a particular value.
        /// </summary>
        /// <param name="value">The value to which to step the meter</param>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        void Increment(int value);

        /// <summary>
        /// Call this method from the worker thread to finalize the progress meter
        /// </summary>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        void End();

        object Lock { get; }
    }
}
