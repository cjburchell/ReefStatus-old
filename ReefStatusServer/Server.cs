namespace Redpoint.ReefStatus.Server
{
    using log4net;

    using RedPoint.ReefStatus.Common.Commands;
    using RedPoint.ReefStatus.Common.ProfiLux;
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

        private IReefStatusSettings settings;

        private CommandThread commands;

        private Controller controler;

        public void Initialize()
        {
            this.log.Debug("Starting Server");

            // create and start web server
            this.settings = ReefStatusSettings.LoadSettings();

            this.controler = ReefStatusSettings.LoadControler();

            this.commands = new CommandThread(this.controler, this.settings.Connection) { Progress = new Progress() };

            this.dataService = new DataService(this.settings.Logging, this.commands, this.controler);

            this.webServer = new WebInterface(this.settings, this.controler, this.commands);

            this.dataService.Start();
        }

        public void Shutdown()
        {
            this.log.Debug("Stopping Server");

            this.dataService.Stop();
            this.webServer = null;
            this.dataService = null;
        }
    }
}