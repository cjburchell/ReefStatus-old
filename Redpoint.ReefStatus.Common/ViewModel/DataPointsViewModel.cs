// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataPointsViewModel.cs"  company="Redpoint">
//      2010
// </copyright>
// <summary>
//   The data points view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ViewModel
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common;
    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    /// <summary>
    /// The data points view model.
    /// </summary>
    public class DataPointsViewModel : BindableBase
    {
        private ICommand deletePointsCommand;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointsViewModel"/> class.
        /// </summary>
        /// <param name="item">
        /// The item to display the data for.
        /// </param>
        public DataPointsViewModel(BaseInfo item)
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;

            this.Item = item;
        }

        private bool loadingData;

        private Dispatcher dispatcher;

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

        public void Refresh()
        {
            this.LoadingData = true;
            new Thread(
                () =>
                    {
                        try
                        {
                            using (IDataAccess dataAccess = ReefStatusSettings.Instance.Logging.Connection.Create())
                            {
                                var dataPoints =
                                    new ObservableCollection<DataPoint>(
                                        dataAccess.GetDataPoints(this.Item.GraphId, true, this.Item.Controller.Id));
                                this.data = dataPoints;
                                this.OnPropertyChanged(() => this.Data);
                            }
                        }
                        catch (ReefStatusException ex)
                        {
                            Logger.Instance.LogError(ex);
                        }

                        this.LoadingData = false;
                    }).Start();
        }

        #endregion

        #region Properties

        private ObservableCollection<DataPoint> data;

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data to display.</value>
        public ObservableCollection<DataPoint> Data
        {
            get
            {
                if (this.data == null)
                {
                    this.Refresh();
                }

                return this.data;
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item to display the data.</value>
        public BaseInfo Item { get; private set; }

        /// <summary>
        /// Gets the delete points command.
        /// </summary>
        /// <value>The delete points command.</value>
        public ICommand DeletePointsCommand
        {
            get
            {
                return this.deletePointsCommand ??
                       (this.deletePointsCommand =
                        new DelegateCommand<IList>(this.DeletePoints, items => items != null && items.Count != 0));
            }
        }

        /// <summary>
        /// Deletes the points.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void DeletePoints(IList obj)
        {
            new Thread(() =>
                {
                    try
                    {
                        using (var dataAccess = ReefStatusSettings.Instance.Logging.Connection.Create())
                        {
                            if (dataAccess != null)
                            {
                                foreach (var item in obj.Cast<DataPoint>().ToList())
                                {
                                    dataAccess.RemoveDataPoint(item.Index);
                                    var item1 = item;
                                    dispatcher.BeginInvoke(new Action(() => this.Data.Remove(item1)));
                                }
                            }
                        }

                        dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (this.Item.HasGraph)
                            {
                                this.Item.Graph.Refresh();
                            }

                            if (this.Item.HasDataPoints)
                            {
                                this.Item.DataPoints.Refresh();
                            }
                        }));
                    }
                    catch (ReefStatusException ex)
                    {
                        Logger.Instance.LogError(ex);
                    }
                }).Start();
        }

        #endregion
    }
}