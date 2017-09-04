// <copyright file="WebInterface.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.WebServer
{
    using System;
    using System.Net.Sockets;

    using HttpServer;
    using HttpServer.HttpModules;
    using HttpServer.MVC;
    using HttpServer.Rules;
    using HttpServer.Sessions;

    using log4net;

    using RedPoint.ReefStatus.Common.Commands;
    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Service;
    using RedPoint.ReefStatus.Common.Settings;

    /// <summary>
    /// Web server interface
    /// </summary>
    public class WebInterface : HttpModule
    {
        private readonly ILog log = LogManager.GetLogger("ReefStatus");

        /// <summary>
        /// Http Server
        /// </summary>
        private HttpServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebInterface"/> class.
        /// </summary>
        public WebInterface(IReefStatusSettings settings, IController controller, CommandThread commandThread, IDataAccess dataAccess, IAlertService alertService)
        {
            this.server = new HttpServer();

            var mod = new ControllerModule();

            this.server.Add(new RedirectRule("/", "/default.html"));

            this.server.Add(this);

            mod.Add(new CommandController(controller, commandThread, alertService));
            mod.Add(new ControllerController(controller));
            mod.Add(new DataController(dataAccess));
            mod.Add(new SettingsController(settings, dataAccess));
            this.server.Add(mod);

            var fileModule = new FileModule("/", Environment.CurrentDirectory + "\\web\\");
            fileModule.AddDefaultMimeTypes();
            fileModule.MimeTypes.Add("xml", "xml");
            fileModule.MimeTypes.Add("svg", "image/svg+xml");
            fileModule.MimeTypes.Add("xsl", "xsl");
            this.server.Add(fileModule);

            try
            {
                this.server.Start(System.Net.IPAddress.Any, 8081);
            }
            catch (SocketException ex)
            {
                throw new ReefStatusException(40001, "Unable to start web server", ex);
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.server.Stop();
            this.server = null;
        }

        /// <summary>
        /// Method that process the url
        /// </summary>
        /// <param name="request">Information sent by the browser about the request</param>
        /// <param name="response">Information that is being sent back to the client.</param>
        /// <param name="session">Session used to</param>
        /// <returns>true if this module handled the request.</returns>
        public override bool Process(IHttpRequest request, IHttpResponse response, IHttpSession session)
        {
            log.DebugFormat("{0} {1}", request.Method, request.Uri);
            return false;
        }
    }
}