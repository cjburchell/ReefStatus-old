// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphViewModel.cs" company="Repoint Apps">
//   2011
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Windows.Threading;

    using Microsoft.Practices.Prism.Mvvm;
    using Microsoft.Research.DynamicDataDisplay.DataSources;

    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    /// <summary>
    /// The graph view model.
    /// </summary>
    public class GraphViewModel : BindableBase
    {
        #region Constants and Fields

        /// <summary>
        /// The loading data.
        /// </summary>
        private bool loadingData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphViewModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public GraphViewModel(BaseInfo settings)
        {
            this.Dispatcher = Dispatcher.CurrentDispatcher;
            this.Settings = settings;
        }

        #endregion

        #region Properties

        protected ObservableDataSource<DataPoint> dataSource;

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public ObservableDataSource<DataPoint> DataSource
        {
            get
            {
                if (this.dataSource == null)
                {
                    this.Refresh();
                }

                return this.dataSource;
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item to display the data.</value>
        public BaseInfo Item
        {
            get
            {
                return this.Settings;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [loading data].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [loading data]; otherwise, <c>false</c>.
        /// </value>
        public bool LoadingData
        {
            get
            {
                return this.loadingData;
            }

            set
            {
                if (this.loadingData != value)
                {
                    this.loadingData = value;
                    this.OnPropertyChanged(() => this.LoadingData);
                }
            }
        }

        /// <summary>
        /// Gets Points.
        /// </summary>
        public ObservableCollection<DataPoint> Points
        {
            get
            {
                return this.DataSource.Collection;
            }
        }

        /// <summary>
        /// Gets Settings.
        /// </summary>
        public BaseInfo Settings { get; private set; }

        /// <summary>
        /// Gets or Sets the currenet displatcher for the view
        /// </summary>
        protected Dispatcher Dispatcher { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            this.LoadingData = true;
            new Thread(
                () =>
                    {
                        this.RefreshThread();
                        this.LoadingData = false;
                    }).Start();
        }

        /// <summary>
        /// Updates the point.
        /// </summary>
        /// <param name="timeStamp">
        /// The time stamp.
        /// </param>
        public virtual void UpdatePoint(DateTime timeStamp)
        {
            if (this.dataSource != null)
            {
                this.DataSource.AppendAsync(
                    this.Dispatcher, new DataPoint(this.Item.Id, timeStamp, this.Item.DoubleValue, 0));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// the collection of points
        /// </returns>
        protected Collection<DataPoint> GetDataPoints(string id)
        {
            var points = new Collection<DataPoint>();
            try
            {
                using (IDataAccess data = ReefStatusSettings.Instance.Logging.Connection.Create())
                {
                    switch (this.Item.Range)
                    {
                        case GraphRange.All:
                            points = data.GetDataPoints(id, false, this.Item.Controller.Id);
                            break;
                        case GraphRange.Year:
                            points = data.GetDataPoints(id, DateTime.Now.AddYears(-1), false, this.Item.Controller.Id);
                            break;
                        case GraphRange.Month:
                            points = data.GetDataPoints(id, DateTime.Now.AddMonths(-1), false, this.Item.Controller.Id);
                            break;
                        case GraphRange.Week:
                            points = data.GetDataPoints(id, DateTime.Now.AddDays(-7), false, this.Item.Controller.Id);
                            break;
                        case GraphRange.Day:
                            points = data.GetDataPoints(id, DateTime.Now.AddDays(-1), false, this.Item.Controller.Id);
                            break;
                    }
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return points;
        }

        /// <summary>
        /// Refreshes using the thread.
        /// </summary>
        protected virtual void RefreshThread()
        {
            Collection<DataPoint> points = this.GetDataPoints(this.Item.Id);
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
        }

        #endregion
    }
}