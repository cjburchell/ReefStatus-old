// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataPointsViewModel.cs"  company="Redpoint">
//      2010
// </copyright>
// <summary>
//   The data points view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;

    using RedPoint.ReefStatus.Common;
    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    /// <summary>
    /// The data points view model.
    /// </summary>
    public class DataPointsViewModel : ViewModelBase, IInfoItem
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
            try
            {
                this.Item = item;
                using (IDataAccess dataAccess = ReefStatusSettings.Instance.Logging.Connection.Create())
                {
                    this.Data =
                        new ObservableCollection<DataPoint>(
                            dataAccess.GetDataPoints(this.Item.GraphId, true, item.Controler.Id));
                }
            }
            catch (DataAccessException ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data to display.</value>
        public ObservableCollection<DataPoint> Data { get; private set; }

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
            try
            {
                using (IDataAccess dataAccess = ReefStatusSettings.Instance.Logging.Connection.Create())
                {
                    if (dataAccess != null)
                    {
                        foreach (DataPoint item in obj.Cast<DataPoint>().ToList())
                        {
                            dataAccess.RemoveDataPoint(item.Index);
                            this.Data.Remove(item);
                        }
                    }
                }
            }
            catch (ReefStatusException ex)
            {
                Trace.WriteLine(ex);
            }
        }

        #endregion
    }
}