// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportViewModel.cs" company="RedpointGames">
//   2010
// </copyright>
// <summary>
//   The report view
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Common.Settings;

    using Microsoft.Research.DynamicDataDisplay.DataSources;

    using RedPoint.ReefStatus.Common;
    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    /// <summary>
    /// The report view
    /// </summary>
    public class ReportViewModel : ViewModelBase, IInfoItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportViewModel"/> class.
        /// </summary>
        /// <param name="item">The item to get stats for.</param>
        public ReportViewModel(BaseInfo item)
        {
            this.Dispatcher = Dispatcher.CurrentDispatcher;

            this.Data = new ObservableCollection<Stats>();
            this.DayStats = new ObservableCollection<Stats>();
            this.UpdateReportCommand = new DelegateCommand(this.UpdateReport);

            this.DailyPoints = new ObservableDataSource<DataPoint>();
            this.DailyMax = new ObservableDataSource<DataPoint>();
            this.DailyMin = new ObservableDataSource<DataPoint>();

            this.WeeklyPoints = new ObservableDataSource<DataPoint>();
            this.WeeklyMax = new ObservableDataSource<DataPoint>();
            this.WeeklyMin = new ObservableDataSource<DataPoint>();

            this.MonthlyPoints = new ObservableDataSource<DataPoint>();
            this.MonthlyMax = new ObservableDataSource<DataPoint>();
            this.MonthlyMin = new ObservableDataSource<DataPoint>();


            this.Item = item;
            this.UpdateReport();
        }

        private Dispatcher Dispatcher { get; set; }

        public ObservableDataSource<DataPoint> DailyPoints { get; set; }
        public ObservableDataSource<DataPoint> DailyMax { get; set; }
        public ObservableDataSource<DataPoint> DailyMin { get; set; }
        public ObservableDataSource<DataPoint> WeeklyPoints { get; set; }
        public ObservableDataSource<DataPoint> WeeklyMax { get; set; }
        public ObservableDataSource<DataPoint> WeeklyMin { get; set; }
        public ObservableDataSource<DataPoint> MonthlyPoints { get; set; }
        public ObservableDataSource<DataPoint> MonthlyMax { get; set; }
        public ObservableDataSource<DataPoint> MonthlyMin { get; set; }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id of the data.</value>
        public BaseInfo Item { get; private set; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data to display.</value>
        public ObservableCollection<Stats> Data { get; private set; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data to display.</value>
        public ObservableCollection<Stats> DayStats { get; private set; }

        /// <summary>
        /// Gets the update report command.
        /// </summary>
        /// <value>The update report command.</value>
        public ICommand UpdateReportCommand { get; private set; }

        /// <summary>
        /// Updates the report.
        /// </summary>
        private void UpdateReport()
        {
            try
            {
                this.Data.Clear();
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
                using (IDataAccess access = ReefStatusSettings.Instance.Logging.Connection.Create())
                {
                    var day = access.GetStats(this.Item.GraphId, DateTime.Now, DateTime.Now.AddDays(-1), this.Item.Controler.Id);
                    day.ApplyConverter(this.Item);
                    day.Range = Language.GetResource("strDay");
                    this.Data.Add(day);

                    var week = access.GetStats(this.Item.GraphId, DateTime.Now, DateTime.Now.AddDays(-7), this.Item.Controler.Id);
                    week.ApplyConverter(this.Item);
                    week.Range = Language.GetResource("strWeek");
                    this.Data.Add(week);

                    var month = access.GetStats(this.Item.GraphId, DateTime.Now, DateTime.Now.AddMonths(-1), this.Item.Controler.Id);
                    month.ApplyConverter(this.Item);
                    month.Range = Language.GetResource("strMonth");
                    this.Data.Add(month);

                    var year = access.GetStats(this.Item.GraphId, DateTime.Now, DateTime.Now.AddYears(-1), this.Item.Controler.Id);
                    year.ApplyConverter(this.Item);
                    year.Range = Language.GetResource("strYear");
                    this.Data.Add(year);

                    var all = access.GetStats(this.Item.GraphId, DateTime.Now, DateTime.Now.AddYears(-100), this.Item.Controler.Id);
                    all.ApplyConverter(this.Item);
                    all.Range = Language.GetResource("strAll");
                    this.Data.Add(all);

                    var tmpDate = access.GetMinDate(this.Item.GraphId, this.Item.Controler.Id);     
                    this.MinDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day);

                    var date = this.MinDate;
                    while (date < DateTime.Now)
                    {
                        var dayStat = access.GetStats(this.Item.GraphId, date.AddDays(1), date, this.Item.Controler.Id, false);
                        dayStat.ApplyConverter(this.Item);
                        dayStat.Date = date;
                        this.DailyPoints.AppendAsync(this.Dispatcher, new DataPoint(this.Item.GraphId, date, dayStat.Average, 0));
                        this.DailyMin.AppendAsync(this.Dispatcher, new DataPoint(this.Item.GraphId, date, dayStat.Min, 0));
                        this.DailyMax.AppendAsync(this.Dispatcher, new DataPoint(this.Item.GraphId, date, dayStat.Max, 0));
                        this.DayStats.Add(dayStat);
                        date = date.AddDays(1);
                    }

                    date = this.MinDate;
                    while (date < DateTime.Now)
                    {
                        var monthStat = access.GetStats(this.Item.GraphId, date.AddMonths(1), date, this.Item.Controler.Id, false);
                        monthStat.ApplyConverter(this.Item);
                        this.MonthlyPoints.AppendAsync(this.Dispatcher, new DataPoint(this.Item.GraphId, date, monthStat.Average, 0));
                        this.MonthlyMin.AppendAsync(this.Dispatcher, new DataPoint(this.Item.GraphId, date, monthStat.Min, 0));
                        this.MonthlyMax.AppendAsync(this.Dispatcher, new DataPoint(this.Item.GraphId, date, monthStat.Max, 0));
                        date = date.AddMonths(1);
                    }

                    date = this.MinDate;               
                    while (date < DateTime.Now)
                    {
                        var weekStat = access.GetStats(this.Item.GraphId, date.AddDays(7), date, this.Item.Controler.Id, false);
                        weekStat.ApplyConverter(this.Item);
                        this.WeeklyPoints.AppendAsync(this.Dispatcher, new DataPoint(this.Item.GraphId, date, weekStat.Average, 0));
                        this.WeeklyMin.AppendAsync(this.Dispatcher, new DataPoint(this.Item.GraphId, date, weekStat.Min, 0));
                        this.WeeklyMax.AppendAsync(this.Dispatcher, new DataPoint(this.Item.GraphId, date, weekStat.Max, 0));
                        date = date.AddDays(7);
                    }
                }
            }
            catch (DataAccessException ex)
            {
                Logger.Instance.LogError(ex);
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
                    this.OnPropertyChanged("MinDate");
                }
            }
        }
    }
}
