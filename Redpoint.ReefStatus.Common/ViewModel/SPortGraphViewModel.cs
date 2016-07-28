// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SPortGraphViewModel.cs" company="Repoint Apps">
//   2011
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ViewModel
{
    using System;
    using System.Collections.ObjectModel;

    using Microsoft.Research.DynamicDataDisplay.DataSources;

    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// The s port graph view model.
    /// </summary>
    public class SPortGraphViewModel : GraphViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SPortGraphViewModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public SPortGraphViewModel(BaseInfo settings)
            : base(settings)
        {
            this.CurrentDataSource = new ObservableDataSource<DataPoint>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public ObservableDataSource<DataPoint> CurrentDataSource { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the point.
        /// </summary>
        /// <param name="timeStamp">
        /// The time stamp.
        /// </param>
        public override void UpdatePoint(DateTime timeStamp)
        {
            base.UpdatePoint(timeStamp);
            this.CurrentDataSource.AppendAsync(
                this.Dispatcher, new DataPoint(this.Item.Id, timeStamp, ((SPort)this.Item).Current, 0));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refreshes using the thread.
        /// </summary>
        protected override void RefreshThread()
        {
            base.RefreshThread();
            var sport = this.Item as SPort;
            if (sport != null)
            {
                Collection<DataPoint> points = this.GetDataPoints(sport.CurrentId);

                this.Dispatcher.BeginInvoke(
                    new Action(
                        () =>
                        {
                            this.CurrentDataSource.Collection.Clear();
                            this.CurrentDataSource.AppendMany(points);
                        }));
            }
        }

        #endregion
    }
}
