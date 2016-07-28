// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportViewModel.cs" company="RedpointGames">
//   2010
// </copyright>
// <summary>
//   The report view
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common.Settings;

    using Microsoft.Research.DynamicDataDisplay.DataSources;

    using RedPoint.ReefStatus.Common;
    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    /// <summary>
    /// The report view
    /// </summary>
    public class ReportViewModel : BindableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportViewModel"/> class.
        /// </summary>
        /// <param name="item">The item to get stats for.</param>
        public ReportViewModel(BaseInfo item)
        {
            this.Dispatcher = Dispatcher.CurrentDispatcher;

            this.UpdateReportCommand = new DelegateCommand(this.UpdateReport, () => !this.LoadingData);
            this.StopCommand = new DelegateCommand(this.Stop, () => this.LoadingData);

            this.DailyMax = new ObservableDataSource<DataPoint>();
            this.DailyMin = new ObservableDataSource<DataPoint>();

            this.WeeklyMax = new ObservableDataSource<DataPoint>();
            this.WeeklyMin = new ObservableDataSource<DataPoint>();

            this.MonthlyMax = new ObservableDataSource<DataPoint>();
            this.MonthlyMin = new ObservableDataSource<DataPoint>();


            this.Item = item;
        }

        public DelegateCommand StopCommand { get; private set; }

        private bool stop;

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.stop = true;
        }

        private Dispatcher Dispatcher { get; set; }

        private ObservableDataSource<DataPoint> dailyPoints;

        public ObservableDataSource<DataPoint> DailyPoints
        {
            get
            {
                if (this.dailyPoints == null)
                {
                    this.UpdateReport();
                }

                return this.dailyPoints;
            }
            
        }

        public ObservableDataSource<DataPoint> DailyMax { get; set; }
        public ObservableDataSource<DataPoint> DailyMin { get; set; }

        private ObservableDataSource<DataPoint> weeklyPoints;

        public ObservableDataSource<DataPoint> WeeklyPoints
        {
            get
            {
                if (this.weeklyPoints == null)
                {
                    this.UpdateReport();
                }

                return this.weeklyPoints;
            }
            
        }

        public ObservableDataSource<DataPoint> WeeklyMax { get; set; }
        public ObservableDataSource<DataPoint> WeeklyMin { get; set; }

        private ObservableDataSource<DataPoint> monthlyPoints;

        public ObservableDataSource<DataPoint> MonthlyPoints
        {
            get
            {
                if (this.monthlyPoints == null)
                {
                    this.UpdateReport();
                }

                return this.monthlyPoints;
            }
            
        }

        public ObservableDataSource<DataPoint> MonthlyMax { get; set; }
        public ObservableDataSource<DataPoint> MonthlyMin { get; set; }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id of the data.</value>
        public BaseInfo Item { get; private set; }

        private ObservableCollection<Stats> dayStats;

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data to display.</value>
        public ObservableCollection<Stats> DayStats
        {
            get
            {
                if (this.dayStats == null)
                {
                    this.UpdateReport();
                }

                return this.dayStats;
            }
        }

        /// <summary>
        /// Gets the update report command.
        /// </summary>
        /// <value>The update report command.</value>
        public ICommand UpdateReportCommand { get; private set; }

        /// <summary>
        /// Updates the report.
        /// </summary>
        public void UpdateReport()
        {
            if (this.dayStats == null)
            {
                this.dayStats = new ObservableCollection<Stats>();
                this.OnPropertyChanged(() => this.DayStats);
            }

            if (this.dailyPoints == null)
            {
                this.dailyPoints = new ObservableDataSource<DataPoint>();
                this.OnPropertyChanged(() => this.DailyPoints);
            }

            if (this.weeklyPoints == null)
            {
                this.weeklyPoints = new ObservableDataSource<DataPoint>();
                this.OnPropertyChanged(() => this.WeeklyPoints);
            }

            if (this.monthlyPoints == null)
            {
                this.monthlyPoints = new ObservableDataSource<DataPoint>();
                this.OnPropertyChanged(() => this.MonthlyPoints);
            }

            this.DayStats.Clear();
            this.DailyPoints.Collection.Clear();
            this.DailyMax.Collection.Clear();
            this.DailyMin.Collection.Clear();

            this.WeeklyPoints.Collection.Clear();
            this.WeeklyMax.Collection.Clear();
            this.WeeklyMin.Collection.Clear();

            this.MonthlyPoints.Collection.Clear();
            this.MonthlyMax.Collection.Clear();
            this.MonthlyMin.Collection.Clear();

            this.LoadingData = true;
            this.stop = false;

            new Thread(
                () =>
                    {
                        try
                        {
                            if (this.stop)
                            {
                                this.LoadingData = false;
                                return;
                            }

                            Collection<DataPoint> data;
                            using (var access = ReefStatusSettings.Instance.Logging.Connection.Create())
                            {
                                data =
                                    new Collection<DataPoint>(
                                        access.GetDataPoints(this.Item.GraphId, true, this.Item.Controller.Id));
                            }

                            var tmpDate = MemoryDataAccess.GetMinDate(data);
                            this.MinDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day);

                            var date = this.MinDate;

                            if (this.stop)
                            {
                                this.LoadingData = false;
                                return;
                            }

                            var points = new Collection<DataPoint>();
                            var minPoints = new Collection<DataPoint>();
                            var maxPoints = new Collection<DataPoint>();

                            while (date < DateTime.Now)
                            {
                                if (this.stop)
                                {
                                    this.LoadingData = false;
                                    return;
                                }

                                var dayStat = MemoryDataAccess.GetStats(date.AddDays(1), date, data);
                                dayStat.Date = date;

                                if (dayStat.Average != 0 && dayStat.Min != 0 && dayStat.Max != 0)
                                {
                                    dayStat.ApplyConverter(this.Item);
                                    points.Add(new DataPoint(this.Item.GraphId, dayStat.Date, dayStat.Average, 0));
                                    minPoints.Add(new DataPoint(this.Item.GraphId, dayStat.Date, dayStat.Min, 0));
                                    maxPoints.Add(new DataPoint(this.Item.GraphId, dayStat.Date, dayStat.Max, 0));

                                    if (points.Count == 30)
                                    {
                                        var points1 = points;
                                        var minPoints1 = minPoints;
                                        var maxPoints1 = maxPoints;
                                        this.Dispatcher.BeginInvoke(
                                            new Action(
                                                () =>
                                                    {
                                                        this.DailyPoints.AppendMany(points1);
                                                        this.DailyMin.AppendMany(minPoints1);
                                                        this.DailyMax.AppendMany(maxPoints1);
                                                    }));

                                        points = new Collection<DataPoint>();
                                        minPoints = new Collection<DataPoint>();
                                        maxPoints = new Collection<DataPoint>();
                                    }
                                }
                                else
                                {
                                    dayStat.ApplyConverter(this.Item);
                                }

                                this.Dispatcher.Invoke(new Action(() => this.DayStats.Add(dayStat)));
                                date = date.AddDays(1);
                            }

                            if (points.Count != 0)
                            {
                                var points1 = points;
                                var minPoints1 = minPoints;
                                var maxPoints1 = maxPoints;
                                this.Dispatcher.BeginInvoke(
                                    new Action(
                                        () =>
                                            {
                                                this.DailyPoints.AppendMany(points1);
                                                this.DailyMin.AppendMany(minPoints1);
                                                this.DailyMax.AppendMany(maxPoints1);
                                            }));

                                points = new Collection<DataPoint>();
                                minPoints = new Collection<DataPoint>();
                                maxPoints = new Collection<DataPoint>();
                            }

                            date = this.MinDate;
                            while (date < DateTime.Now)
                            {
                                if (this.stop)
                                {
                                    this.LoadingData = false;
                                    return;
                                }

                                var weekStat = MemoryDataAccess.GetStats(date.AddDays(7), date, data);
                                weekStat.Date = date;

                                if (weekStat.Average != 0 && weekStat.Min != 0 && weekStat.Max != 0)
                                {
                                    weekStat.ApplyConverter(this.Item);
                                    points.Add(new DataPoint(this.Item.GraphId, weekStat.Date, weekStat.Average, 0));
                                    minPoints.Add(new DataPoint(this.Item.GraphId, weekStat.Date, weekStat.Min, 0));
                                    maxPoints.Add(new DataPoint(this.Item.GraphId, weekStat.Date, weekStat.Max, 0));

                                    if (points.Count == 30)
                                    {
                                        var points1 = points;
                                        var minPoints1 = minPoints;
                                        var maxPoints1 = maxPoints;
                                        this.Dispatcher.BeginInvoke(
                                            new Action(
                                                () =>
                                                {
                                                    this.WeeklyPoints.AppendMany(points1);
                                                    this.WeeklyMin.AppendMany(minPoints1);
                                                    this.WeeklyMax.AppendMany(maxPoints1);
                                                }));

                                        points = new Collection<DataPoint>();
                                        minPoints = new Collection<DataPoint>();
                                        maxPoints = new Collection<DataPoint>();
                                    }
                                }

                                date = date.AddDays(7);
                            }

                            if (points.Count != 0)
                            {
                                var points1 = points;
                                var minPoints1 = minPoints;
                                var maxPoints1 = maxPoints;
                                this.Dispatcher.BeginInvoke(
                                    new Action(
                                        () =>
                                        {
                                            this.WeeklyPoints.AppendMany(points1);
                                            this.WeeklyMin.AppendMany(minPoints1);
                                            this.WeeklyMax.AppendMany(maxPoints1);
                                        }));

                                points = new Collection<DataPoint>();
                                minPoints = new Collection<DataPoint>();
                                maxPoints = new Collection<DataPoint>();
                            }

                            date = this.MinDate;
                            while (date < DateTime.Now)
                            {
                                if (this.stop)
                                {
                                    this.LoadingData = false;
                                    return;
                                }

                                var monthStat = MemoryDataAccess.GetStats(date.AddMonths(1), date, data);
                                monthStat.Date = date;

                                if (monthStat.Average != 0 && monthStat.Min != 0 && monthStat.Max != 0)
                                {
                                    monthStat.ApplyConverter(this.Item);

                                    points.Add(new DataPoint(this.Item.GraphId, monthStat.Date, monthStat.Average, 0));
                                    minPoints.Add(new DataPoint(this.Item.GraphId, monthStat.Date, monthStat.Min, 0));
                                    maxPoints.Add(new DataPoint(this.Item.GraphId, monthStat.Date, monthStat.Max, 0));
                                }

                                date = date.AddMonths(1);
                            }

                            if (points.Count != 0)
                            {
                                var points1 = points;
                                        var minPoints1 = minPoints;
                                        var maxPoints1 = maxPoints;
                                this.Dispatcher.BeginInvoke(
                                    new Action(
                                        () =>
                                            {
                                                this.MonthlyPoints.AppendMany(points1);
                                                this.MonthlyMin.AppendMany(minPoints1);
                                                this.MonthlyMax.AppendMany(maxPoints1);
                                            }));
                            }
                        }
                        catch (ReefStatusException ex)
                        {
                            Logger.Instance.LogError(ex);
                        }

                        this.LoadingData = false;
                    }).Start();
        }

        private bool loadingData;

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
                if (value != this.loadingData)
                {
                    this.loadingData = value;
                    this.OnPropertyChanged(() => this.LoadingData);
                }
            }
        }

        private DateTime minDate;

        public DateTime MinDate
        {
            get
            {
                return this.minDate;
            }

            set
            {
                if (value != this.minDate)
                {
                    this.minDate = value;
                    this.OnPropertyChanged(() => this.MinDate);
                }
            }
        }
    }
}
