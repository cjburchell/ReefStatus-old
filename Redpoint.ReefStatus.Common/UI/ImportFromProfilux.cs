namespace RedPoint.ReefStatus.Common.UI
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;

    using RedPoint.ReefStatus.Common.Commands;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;

    public class ImportFromProfilux
    {
        /// <summary>
        /// Starts the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="commands">The commands.</param>
        public static void Start(IProgressCallback callback, Controller controller, Dispatcher dispatcher, CommandThread commands, IReefStatusSettings settings)
        {
            new Thread(() =>
                    {
                        if (callback != null)
                        {
                            try
                            {
                                callback.Begin(Language.GetResource("strImport"));

                                Collection<ItemDataRow> data = commands.GetDataPoints(callback);

                                if (callback.IsAborting)
                                {
                                    return;
                                }

                                using (var access = settings.Logging.Connection.Create())
                                {
                                    access.AddLog(data, callback, controller.Id);
                                }

                                foreach (var item in controller.Items.OfType<Probe>().Where(item => item.SaveToDatabase))
                                {
                                    var item1 = item;
                                    dispatcher.BeginInvoke(
                                        new Action(
                                            () =>
                                                {
                                                    if (item1.HasGraph)
                                                    {
                                                        item1.Graph.Refresh();
                                                    }

                                                    if (item1.HasDataPoints)
                                                    {
                                                        item1.DataPoints.Refresh();
                                                    }

                                                    if (item1.HasReport)
                                                    {
                                                        item1.Report.UpdateReport();
                                                    }
                                                }));
                                }
                            }
                            catch (ReefStatusException ex)
                            {
                                Logger.Instance.LogError(ex);
                            }
                            finally
                            {
                                callback.End();
                            }
                        }
                    }).Start();
        }
    }
}
