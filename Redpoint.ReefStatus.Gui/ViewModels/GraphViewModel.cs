// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphViewModel.cs" company="Redpoint">
//      2009
// </copyright>
// <summary>
//   Defines the GraphViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Threading;
    using Common.UI.ViewModel;
    using Microsoft.Research.DynamicDataDisplay.DataSources;

    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;

    /// <summary>
    /// The graph view model.
    /// </summary>
    public class GraphViewModel : ViewModelBase, IInfoItem
    {
        /// <summary>
        /// the currenet displatcher for the view
        /// </summary>
        protected Dispatcher Dispatcher { get; private set; }

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
            this.DataSource = new ObservableDataSource<DataPoint>();
        }

        /// <summary>
        /// Gets Settings.
        /// </summary>
        public BaseInfo Settings { get; private set; }

        /// <summary>
        /// Gets Points.
        /// </summary>
        public ObservableCollection<DataPoint> Points
        {
            get { return this.DataSource.Collection; }
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public ObservableDataSource<DataPoint> DataSource { get; private set; }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item to display the data.</value>
        public BaseInfo Item
        {
            get { return this.Settings; }
        }

        protected Collection<DataPoint> GetDataPoints(string id)
        {
            Collection<DataPoint> points = new Collection<DataPoint>();
            using (
                                                IDataAccess data =
                                                    ReefStatusSettings.Instance.Logging.Connection.Create())
            {
                switch (this.Item.Range)
                {
                    case GraphRange.All:
                        points = data.GetDataPoints(id, false, this.Item.Controler.Id);
                        break;
                    case GraphRange.Year:
                        points = data.GetDataPoints(
                            id, DateTime.Now.AddYears(-1), false, this.Item.Controler.Id);
                        break;
                    case GraphRange.Month:
                        points = data.GetDataPoints(
                            id, DateTime.Now.AddMonths(-1), false, this.Item.Controler.Id);
                        break;
                    case GraphRange.Week:
                        points = data.GetDataPoints(
                            id, DateTime.Now.AddDays(-7), false, this.Item.Controler.Id);
                        break;
                    case GraphRange.Day:
                        points = data.GetDataPoints(
                            id, DateTime.Now.AddDays(-1), false, this.Item.Controler.Id);
                        break;
                }
            }

            return points;
        }

        /// <summary>
        /// The add points.
        /// </summary>
        /// <param name="points">
        /// The points.
        /// </param>
        public virtual void Refresh()
        {

            Collection<DataPoint> points = this.GetDataPoints(this.Item.Id);
            this.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                        {
                            this.DataSource.Collection.Clear();
                            this.DataSource.AppendMany(points);
                        }));
        }


        public virtual void UpdatePoint(DateTime timeStamp)
        {
            this.DataSource.AppendAsync(this.Dispatcher, new DataPoint(Item.Id, timeStamp, this.Item.DoubleValue, 0));
        }
    }

    public class SPortGraphViewModel : GraphViewModel
    {
        public SPortGraphViewModel(BaseInfo settings)
            : base(settings)
        {
            this.CurrentDataSource = new ObservableDataSource<DataPoint>();
        }

        public override void UpdatePoint(DateTime timeStamp)
        {
            base.UpdatePoint(timeStamp);
            this.CurrentDataSource.AppendAsync(this.Dispatcher, new DataPoint(Item.Id, timeStamp, ((SPort)Item).Current, 0));
        }

        public override void Refresh()
        {
            base.Refresh();

            var sport = (SPort)this.Item;

            Collection<DataPoint> points = this.GetDataPoints(sport.CurrentId);

            this.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                    {
                        this.CurrentDataSource.Collection.Clear();
                        this.CurrentDataSource.AppendMany(points);
                    }));
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public ObservableDataSource<DataPoint> CurrentDataSource { get; private set; }
    }

    public class ProbeGraphViewModel : GraphViewModel
    {
        public ProbeGraphViewModel(BaseInfo settings)
            : base(settings)
        {
            this.HighRangeDataSource = new ObservableDataSource<DataPoint>();
            this.LowRangeDataSource = new ObservableDataSource<DataPoint>();
            this.NominalDataSource = new ObservableDataSource<DataPoint>();
        }

        public override void Refresh()
        {
            Collection<DataPoint> points = this.GetDataPoints(this.Item.Id);

            var probe = (Probe)this.Item;
            foreach (var point in points)
            {
                point.Value = probe.ConvertValue(point.Value);
            }

            this.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                        {
                            this.DataSource.Collection.Clear();
                            this.DataSource.AppendMany(points);
                        }));

            if (points.Count != 0)
            {
                Collection<DataPoint> nominalPoints = new Collection<DataPoint>();
                nominalPoints.Add(new DataPoint("", points[0].Time, probe.ConvertValue(probe.NominalValue), 0));
                nominalPoints.Add(new DataPoint("", points[points.Count - 1].Time, probe.ConvertValue(probe.NominalValue), 0));

                this.Dispatcher.BeginInvoke(
                    new Action(
                        () =>
                            {
                                this.NominalDataSource.Collection.Clear();
                                this.NominalDataSource.AppendMany(nominalPoints);
                            }));

                if (probe.AlarmEnable)
                {
                    Collection<DataPoint> lowRange = new Collection<DataPoint>();
                    lowRange.Add(
                        new DataPoint(
                            "", points[0].Time, probe.ConvertValue(probe.NominalValue - probe.AlarmDeviation), 0));
                    lowRange.Add(
                        new DataPoint(
                            "",
                            points[points.Count - 1].Time,
                            probe.ConvertValue(probe.NominalValue - probe.AlarmDeviation),
                            0));

                    this.Dispatcher.BeginInvoke(
                        new Action(
                            () =>
                                {
                                    this.LowRangeDataSource.Collection.Clear();
                                    this.LowRangeDataSource.AppendMany(lowRange);
                                }));

                    Collection<DataPoint> highRange = new Collection<DataPoint>();
                    highRange.Add(
                        new DataPoint(
                            "", points[0].Time, probe.ConvertValue(probe.NominalValue + probe.AlarmDeviation), 0));
                    highRange.Add(
                        new DataPoint(
                            "",
                            points[points.Count - 1].Time,
                            probe.ConvertValue(probe.NominalValue + probe.AlarmDeviation),
                            0));

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

        public override void UpdatePoint(DateTime timeStamp)
        {
            var probe = (Probe)this.Item;
            this.DataSource.AppendAsync(this.Dispatcher, new DataPoint(probe.Id, timeStamp, (double)probe.ConvertedValue, 0));
            this.NominalDataSource.AppendAsync(
                this.Dispatcher, new DataPoint(probe.Id, timeStamp, probe.ConvertValue(probe.NominalValue), 0));

            if (probe.AlarmEnable)
            {
                this.LowRangeDataSource.AppendAsync(
                    this.Dispatcher,
                    new DataPoint(probe.Id, timeStamp, probe.ConvertValue(probe.NominalValue - probe.AlarmDeviation), 0));
                this.HighRangeDataSource.AppendAsync(
                    this.Dispatcher,
                    new DataPoint(probe.Id, timeStamp, probe.ConvertValue(probe.NominalValue + probe.AlarmDeviation), 0));
            }
        }

        public ObservableDataSource<DataPoint> HighRangeDataSource { get; private set; }
        public ObservableDataSource<DataPoint> LowRangeDataSource { get; private set; }
        public ObservableDataSource<DataPoint> NominalDataSource { get; private set; }
    }
}