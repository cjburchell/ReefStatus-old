namespace Redpoint.ReefStatus.Server
{
    using log4net;

    using RedPoint.ReefStatus.Common.Commands;
    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux.Data;
    using RedPoint.ReefStatus.Common.Service;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.WebServer;

    using ReefStatusServer;

    public class Server
    {
        private readonly ILog log = LogManager.GetLogger("ReefStatus");

        private DataService dataService;

        /// <summary>
        ///     The web server.
        /// </summary>
        private WebInterface webServer;

        public void Initialize()
        {
            this.log.Debug("Starting Server");

            var dataAccess = new CouchDataAccess();

            // create and start web server
            var settings = new ReefStatusSettings(dataAccess);

            var controler = new Controller();

            var alertService = new AlertService(settings.Mail, controler);

            var commands = new CommandThread(controler, settings.Connection) { Progress = new Progress() };

            this.dataService = new DataService(settings.Logging, commands, controler, dataAccess, alertService);

            this.webServer = new WebInterface(settings, controler, commands, dataAccess, alertService);

            this.dataService.Start();
        }

        public void Shutdown()
        {
            this.log.Debug("Stopping Server");

            this.dataService.Stop();
            this.webServer.Stop();
        }
    }
}