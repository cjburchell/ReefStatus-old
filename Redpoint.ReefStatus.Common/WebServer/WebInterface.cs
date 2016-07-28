// <copyright file="WebInterface.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.WebServer
{
    using System;
    using System.Diagnostics;
    using System.Net.Sockets;

    using HttpServer;
    using HttpServer.HttpModules;
    using HttpServer.MVC;
    using HttpServer.Rules;
    using HttpServer.Sessions;

    using RedPoint.ReefStatus.Common.Settings;

    /// <summary>
    /// Web server interface
    /// </summary>
    public class WebInterface : HttpModule
    {
        /// <summary>
        /// Http Server
        /// </summary>
        private HttpServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebInterface"/> class.
        /// </summary>
        public WebInterface()
        {
            this.server = new HttpServer();

            var mod = new ControllerModule();

            this.server.Add(new RedirectRule("/", "/default.html"));

            this.server.Add(this);

            // browsing to http://localhost:8081/user/edit will invoke the method "public string Edit()" in  UserController.
            mod.Add(new CommandController());
            this.server.Add(mod);

            var fileModule = new FileModule("/", Environment.CurrentDirectory + "\\web\\");
            fileModule.AddDefaultMimeTypes();
            fileModule.MimeTypes.Add("xml", "xml");
            fileModule.MimeTypes.Add("xsl", "xsl");
            this.server.Add(fileModule);

            try
            {
                this.server.Start(System.Net.IPAddress.Any, ReefStatusSettings.Instance.Web.Port);
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
            Trace.WriteLine(request.Uri.ToString());
            return false;
        }
    }
}