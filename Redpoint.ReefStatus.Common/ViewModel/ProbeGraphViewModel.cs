// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProbeGraphViewModel.cs" company="Repoint Apps">
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
    /// The probe graph view model.
    /// </summary>
    public class ProbeGraphViewModel : GraphViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProbeGraphViewModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public ProbeGraphViewModel(BaseInfo settings)
            : base(settings)
        {
            this.HighRangeDataSource = new ObservableDataSource<DataPoint>();
            this.LowRangeDataSource = new ObservableDataSource<DataPoint>();
            this.NominalDataSource = new ObservableDataSource<DataPoint>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets HighRangeDataSource.
        /// </summary>
        public ObservableDataSource<DataPoint> HighRangeDataSource { get; private set; }

        /// <summary>
        /// Gets LowRangeDataSource.
        /// </summary>
        public ObservableDataSource<DataPoint> LowRangeDataSource { get; private set; }

        /// <summary>
        /// Gets NominalDataSource.
        /// </summary>
        public ObservableDataSource<DataPoint> NominalDataSource { get; private set; }

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
            var probe = this.Item as IRangeInfo;
            if (probe != null)
            {
                if (this.dataSource != null)
                {
                    this.DataSource.AppendAsync(
                        this.Dispatcher, new DataPoint(probe.Id, timeStamp, (double)probe.ConvertedValue, 0));
                }

                if (probe.ShowCenter)
                {
                    this.NominalDataSource.AppendAsync(
                        this.Dispatcher, new DataPoint(probe.Id, timeStamp, probe.CenterValue, 0));
                }

                if (probe.ShowMin)
                {
                    this.LowRangeDataSource.AppendAsync(
                        this.Dispatcher, new DataPoint(probe.Id, timeStamp, probe.MinRange, 0));
                }

                if (probe.ShowMax)
                {
                    this.HighRangeDataSource.AppendAsync(
                        this.Dispatcher, new DataPoint(probe.Id, timeStamp, probe.MaxRange, 0));
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refreshes using the thread.
        /// </summary>
        protected override void RefreshThread()
        {
            Collection<DataPoint> points = this.GetDataPoints(this.Item.Id);

            var probe = this.Item as IRangeInfo;

            if (probe != null)
            {
                foreach (DataPoint point in points)
                {
                    point.Value = probe.ConvertValue(point.Value);
                }

                this.Dispatcher.BeginInvoke(
                    new Action(
                        () =>
                        {
                            if (this.dataSource == null)
                            {
                                this.dataSource = new ObservableDataSource<DataPoint>();
                                this.OnPropertyChanged(() => this.DataSource);
                                this.OnPropertyChanged(() => this.Points);
                            }

                            this.dataSource.Collection.Clear();
                            this.dataSource.AppendMany(points);

                        }));

                if (points.Count != 0)
                {
                    if (probe.ShowCenter)
                    {
                        var nominalPoints = new Collection<DataPoint>
                            {
                                new DataPoint(string.Empty, points[0].Time, probe.CenterValue, 0),
                                new DataPoint(string.Empty, points[points.Count - 1].Time, probe.CenterValue, 0)
                            };

                        this.Dispatcher.BeginInvoke(
                            new Action(
                                () =>
                                    {
                                        this.NominalDataSource.Collection.Clear();
                                        this.NominalDataSource.AppendMany(nominalPoints);
                                    }));
                    }

                    if (probe.ShowMin)
                    {
                        var lowRange = new Collection<DataPoint>
                            {
                                new DataPoint(string.Empty, points[0].Time, probe.MinRange, 0),
                                new DataPoint(string.Empty, points[points.Count - 1].Time, probe.MinRange, 0)
                            };

                        this.Dispatcher.BeginInvoke(
                            new Action(
                                () =>
                                    {
                                        this.LowRangeDataSource.Collection.Clear();
                                        this.LowRangeDataSource.AppendMany(lowRange);
                                    }));
                    }

                    if (probe.ShowMax)
                    {
                        var highRange = new Collection<DataPoint>
                            {
                                new DataPoint(string.Empty, points[0].Time, probe.MaxRange, 0),
                                new DataPoint(string.Empty, points[points.Count - 1].Time, probe.MaxRange, 0)
                            };

                        this.Dispatcher.BeginInvoke(
                            new Action(
                                () =>
                                    {
                                        this.HighRangeDataSource.Collection.Clear();
                                        this.HighRangeDataSource.AppendMany(highRange);
                                    }));
                    }
                }
            }
        }

        #endregion
    }
}
