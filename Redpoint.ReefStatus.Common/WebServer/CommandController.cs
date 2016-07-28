// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandController.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.WebServer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Xml.Serialization;

    using HttpServer.MVC.Controllers;

    using Newtonsoft.Json;

    using RedPoint.ReefStatus.Common.Core;
    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;

    using Brushes = System.Windows.Media.Brushes;
    using Pen = System.Windows.Media.Pen;
    using Point = System.Windows.Point;
    using Size = System.Drawing.Size;

    /// <summary>
    /// Command Controller
    /// </summary>
    public class CommandController : RequestController
    {
        #region Properties

        /// <summary>
        /// Gets the default Controller.
        /// </summary>
        /// <value>The default Controller.</value>
        private static Controller DefaultController
        {
            get
            {
                if (ReefStatusSettings.Instance.Controllers.Count == 0)
                {
                    return null;
                }

                return ReefStatusSettings.Instance.Controllers[0];
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can write.
        /// </summary>
        /// <value><c>true</c> if this instance is locked; otherwise, <c>false</c>.</value>
        private bool IsLocked
        {
            get
            {
                if (ReefStatusSettings.Instance.Web.Protection)
                {
                    return WebSession.GetCurrent(this.SessionId).IsLocked;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the paramaters.
        /// </summary>
        /// <value>The paramaters.</value>
        private Dictionary<string, string> Paramaters
        {
            get
            {
                return CreateParams(this.Id);
            }
        }

        /// <summary>
        /// Gets the session ID.
        /// </summary>
        /// <value>The session ID.</value>
        private string SessionId
        {
            get
            {
                return this.Request.RemoteEndPoint.Address.ToString();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the User values.
        /// </summary>
        /// <returns>
        /// xml user values
        /// </returns>
        public string AllValues()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                return this.FormatResult(Controller.Items);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Creates a banner
        /// </summary>
        /// <returns>
        /// No value
        /// </returns>
        [RawHandler]
        public string Banner()
        {
            try
            {
                Dictionary<string, string> param = this.Paramaters;
                int height = 80;
                int width = 468;

                try
                {
                    if (param.ContainsKey("hight"))
                    {
                        height = int.Parse(param["hight"]);
                    }

                    if (param.ContainsKey("width"))
                    {
                        width = int.Parse(param["width"]);
                    }
                }
                catch (FormatException)
                {
                }

                Controller Controller = DefaultController;
                if (param.ContainsKey("Controller"))
                {
                    int ControllerId = int.Parse(param["Controller"]);
                    Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                }

                var resetEvent = new ManualResetEvent(false);

                RenderTargetBitmap image;
                Application.Current.Dispatcher.Invoke(
                    new Action(
                        () =>
                            {
                                const double dpiX = 96.0;
                                const double dpiY = 96.0;

                                image = new RenderTargetBitmap(
                                    (int)(width * dpiX / 96.0), 
                                    (int)(height * dpiY / 96.0), 
                                    dpiX, 
                                    dpiY, 
                                    PixelFormats.Pbgra32);

                                var dv = new DrawingVisual();
                                using (DrawingContext ctx = dv.RenderOpen())
                                {
                                    var pen = new Pen(Brushes.Black, 1);
                                    ctx.DrawRoundedRectangle(Brushes.White, pen, new Rect(0, 0, width, height), 5, 5);

                                    var font = new Typeface("Comic Sans MS");
                                    FormattedText formattedText;

                                    try
                                    {
                                        formattedText = new FormattedText(
                                            Controller.Name, 
                                            CultureInfo.CurrentCulture, 
                                            FlowDirection.LeftToRight, 
                                            font, 
                                            15, 
                                            Brushes.Black);
                                        ctx.DrawText(formattedText, new Point(5, 1));
                                    }
                                    catch (FileNotFoundException)
                                    {
                                        font = new Typeface("Verdana");

                                        formattedText = new FormattedText(
                                            Controller.Name, 
                                            CultureInfo.CurrentCulture, 
                                            FlowDirection.LeftToRight, 
                                            font, 
                                            15, 
                                            Brushes.Black);
                                        ctx.DrawText(formattedText, new Point(5, 1));
                                    }

                                    double ylocation = formattedText.Height;
                                    double xlocation = 5;
                                    double maxWidth = 0;
                                    foreach (var probe in Controller.Items.OfType<Probe>())
                                    {
                                        string text = probe.DisplayName + " " + probe.ValueWithUnits;
                                        var probeText = new FormattedText(
                                            text, 
                                            CultureInfo.CurrentCulture, 
                                            FlowDirection.LeftToRight, 
                                            font, 
                                            10, 
                                            probe.IsAlarmOn == CurrentState.On ? Brushes.Red : Brushes.Black);
                                        maxWidth = Math.Max(maxWidth, probeText.Width);
                                        ctx.DrawText(probeText, new Point(xlocation, ylocation));
                                        ylocation += probeText.Height;

                                        if ((ylocation + probeText.Height) >= height)
                                        {
                                            ylocation = formattedText.Height;
                                            xlocation += maxWidth + 10;
                                            maxWidth = 0;
                                        }
                                    }

                                    var reefStatusUrlText = new FormattedText(
                                        "reefstatus.codeplex.com", 
                                        CultureInfo.CurrentCulture, 
                                        FlowDirection.LeftToRight, 
                                        font, 
                                        8, 
                                        Brushes.Blue);
                                    ctx.DrawText(
                                        reefStatusUrlText, 
                                        new Point((width - reefStatusUrlText.Width) - 4, height - reefStatusUrlText.Height));

                                    var reefStatusText = new FormattedText(
                                        "Generated by ReefStatus", 
                                        CultureInfo.CurrentCulture, 
                                        FlowDirection.LeftToRight, 
                                        font, 
                                        8, 
                                        Brushes.Blue);

                                    ctx.DrawText(
                                        reefStatusText, 
                                        new Point((width - reefStatusText.Width) - 4, height - reefStatusText.Height - reefStatusUrlText.Height));

                                    var bi =
                                        new BitmapImage(
                                            new Uri("web\\icons\\starfish-icon.png", UriKind.RelativeOrAbsolute));

                                    ctx.DrawImage(bi, new Rect(width - 34, 2, 32, 32));
                                }

                                image.Render(dv);

                                this.Response.ContentType = "image/png";
                                var encoder = new PngBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(image));
                                encoder.Save(this.Response.Body);

                                resetEvent.Set();
                            }));

                resetEvent.WaitOne();

                this.Response.Send();
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Clears the level alarm.
        /// </summary>
        /// <returns>
        /// OK if succeded
        /// </returns>
        public string ClearLevelAlarm()
        {
            try
            {
                if (!this.IsLocked && this.Paramaters.ContainsKey("type"))
                {
                    Dictionary<string, string> param = this.Paramaters;

                    string type = param["type"];

                    Controller Controller = DefaultController;
                    if (param.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(param["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    BaseInfo infoItem = Controller.Items.FirstOrDefault(item => item.Id == type);
                    if (infoItem is LevelSensor)
                    {
                        Controller.Commands.SendClearLevelAlarm((LevelSensor)infoItem);
                    }

                    return this.FormatResult(true);
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Make a clone of this controller
        /// </summary>
        /// <returns>
        /// a new controller with the same base information as this one.
        /// </returns>
        public override object Clone()
        {
            return new CommandController();
        }

        /// <summary>
        /// Customs the graph.
        /// </summary>
        /// <returns>
        /// Custom graph
        /// </returns>
        [RawHandler]
        public string CustomGraph()
        {
            try
            {
                Dictionary<string, string> param = this.Paramaters;
                if (param.ContainsKey("type"))
                {
                    string[] types = param["type"].Split(new[] { ',' });

                    int hight = 480;
                    int width = 640;

                    try
                    {
                        if (param.ContainsKey("hight"))
                        {
                            hight = int.Parse(param["hight"]);
                        }

                        if (param.ContainsKey("width"))
                        {
                            width = int.Parse(param["width"]);
                        }
                    }
                    catch (FormatException)
                    {
                    }

                    DateTime startTime = DateTime.Parse(param["start"], CultureInfo.InvariantCulture);
                    DateTime endTime = DateTime.Parse(param["end"], CultureInfo.InvariantCulture);

                    Controller Controller = DefaultController;
                    if (param.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(param["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    if (Controller != null)
                    {
                        Dictionary<string, Collection<DataPoint>> datapoints = types.ToDictionary(
                            type => type, type => GetDatapoints(startTime, endTime, type, Controller));
                    }
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Datas the points.
        /// </summary>
        /// <returns>
        /// xml represntaion of the data points
        /// </returns>
        public string DataPoints()
        {
            try
            {
                Dictionary<string, string> param = this.Paramaters;
                if (param.ContainsKey("type"))
                {
                    string type = param["type"];

                    Controller Controller = DefaultController;
                    if (param.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(param["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    Collection<DataPoint> datapoints;
                    if (param.ContainsKey("range"))
                    {
                        datapoints = GetDatapoints(param["range"], type, Controller);
                    }
                    else
                    {
                        int count = 0;
                        if (param.ContainsKey("count"))
                        {
                            try
                            {
                                count = int.Parse(param["count"], CultureInfo.CurrentCulture);
                            }
                            catch (FormatException ex)
                            {
                                throw new ReefStatusException(6001, "Data Point Count Error", ex);
                            }
                        }

                        using (IDataAccess dataAccess = ReefStatusSettings.Instance.Logging.Connection.Create())
                        {
                            BaseInfo graphSetting = Controller.Items.FirstOrDefault(item => item.Id == type);
                            datapoints = count == 0
                                             ? dataAccess.GetDataPoints(graphSetting.GraphId, true, Controller.Id)
                                             : dataAccess.GetDataPoints(graphSetting.GraphId, count, true, Controller.Id);
                            if (graphSetting is Probe)
                            {
                                foreach (DataPoint dataPoint in datapoints)
                                {
                                    dataPoint.Value = ((Probe)graphSetting).ConvertValue(dataPoint.Value);
                                }
                            }
                        }
                    }

                    bool isJson = false;
                    if (this.Paramaters.ContainsKey("output"))
                    {
                        isJson = this.Paramaters["output"] == "json";
                    }

                    if (isJson)
                    {
                        string callback = string.Empty;
                        if (this.Paramaters.ContainsKey("callback"))
                        {
                            callback = this.Paramaters["callback"];
                        }

                        string data = string.Empty;

                        if (this.Paramaters.ContainsKey("graphdata") && this.Paramaters["graphdata"] == "true")
                        {
                            BaseInfo graphSetting = Controller.Items.FirstOrDefault(item => item.Id == type);
                            if (graphSetting != null)
                            {
                                var webGraphData = new WebGraphData
                                    {
                                        Units = graphSetting.Units, 
                                        Colour = graphSetting.Colour, 
                                        DisplayName = graphSetting.DisplayName, 
                                        Id = graphSetting.Id, 
                                        Data = datapoints
                                    };

                                data = JsonConvert.SerializeObject(webGraphData, Formatting.None);
                            }
                        }
                        else
                        {
                            data = JsonConvert.SerializeObject(datapoints, Formatting.None);
                        }

                        this.Response.ContentType = "text/javascript";

                        if (string.IsNullOrEmpty(callback))
                        {
                            return data;
                        }

                        return string.Format("{0}({1});", callback, data);
                    }

                    TextWriter writer = new StringWriter(CultureInfo.CurrentCulture);
                    var serializer = new XmlSerializer(typeof(Collection<DataPoint>));
                    serializer.Serialize(writer, datapoints);
                    writer.Flush();
                    this.Response.ContentType = MediaTypeNames.Text.Xml;
                    return writer.ToString();
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Gets a list of Digitals inputs.
        /// </summary>
        /// <returns>
        /// XML String
        /// </returns>
        public string DigitalInputs()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                return this.FormatResult(Controller.Items.OfType<DigitalInput>());
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Displays this instance.
        /// </summary>
        /// <returns>
        /// the display information string
        /// </returns>
        public string Display()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                try
                {
                    bool isJson = false;
                    bool isXml = false;
                    if (this.Paramaters.ContainsKey("output"))
                    {
                        isJson = this.Paramaters["output"] == "json";
                        isXml = this.Paramaters["output"] == "xml";
                    }

                    string callback = string.Empty;
                    if (this.Paramaters.ContainsKey("callback"))
                    {
                        callback = this.Paramaters["callback"];
                    }

                    if (isJson)
                    {
                        string[] display = Controller.Commands.GetDisplay();
                        this.Response.ContentType = "text/javascript";

                        string data = JsonConvert.SerializeObject(display, Formatting.None);

                        if (string.IsNullOrEmpty(callback))
                        {
                            return data;
                        }

                        return string.Format("{0}({1});", callback, data);
                    }

                    if (isXml)
                    {
                        string[] display = Controller.Commands.GetDisplay();

                        TextWriter writer = new StringWriter(CultureInfo.CurrentCulture);
                        var serializer = new XmlSerializer(typeof(string[]));
                        serializer.Serialize(writer, display);
                        writer.Flush();
                        this.Response.ContentType = MediaTypeNames.Text.Xml;
                        return writer.ToString();
                    }

                    string displayText = Controller.Commands.GetDisplayString();
                    this.Response.ContentType = MediaTypeNames.Text.Html;
                    displayText = displayText.Replace("<", "&lt;");
                    displayText = displayText.Replace(">", "&gt;");
                    return displayText.Replace("\n", "<br />");
                }
                catch (ReefStatusException ex)
                {
                    Logger.Instance.LogError(ex);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets a list of the dousing in XML
        /// </summary>
        /// <returns>
        /// XML String
        /// </returns>
        public string Dosing()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                return this.FormatResult(Controller.Items.OfType<DosingPump>());
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Feeds the pasue.
        /// </summary>
        /// <returns>
        /// Ok if succeded
        /// </returns>
        public string FeedPasue()
        {
            try
            {
                if (!this.IsLocked && DefaultController != null)
                {
                    Controller Controller = DefaultController;
                    if (this.Paramaters.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(this.Paramaters["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    bool enable = true;
                    if (this.Paramaters.ContainsKey("enable"))
                    {
                        enable = bool.Parse(this.Paramaters["enable"]);
                    }

                    Controller.Commands.SendFeed(enable);
                    return this.FormatResult(true);
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Graphed this instance.
        /// </summary>
        /// <returns>
        /// the graph image
        /// </returns>
        [RawHandler]
        public string Graph()
        {
            try
            {
                Dictionary<string, string> param = this.Paramaters;
                if (param.ContainsKey("type"))
                {
                    string type = param["type"];

                    int hight = 480;
                    int width = 640;

                    try
                    {
                        if (param.ContainsKey("hight"))
                        {
                            hight = int.Parse(param["hight"]);
                        }

                        if (param.ContainsKey("width"))
                        {
                            width = int.Parse(param["width"]);
                        }
                    }
                    catch (FormatException)
                    {
                    }

                    string range = "all";
                    if (param.ContainsKey("range"))
                    {
                        range = param["range"];
                    }

                    Controller Controller = DefaultController;
                    if (param.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(param["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    BaseInfo graphSetting = Controller.Items.FirstOrDefault(item => item.Id == type);
                    Collection<DataPoint> datapoints = GetDatapoints(range, type, Controller);
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Infoes this instance.
        /// </summary>
        /// <returns>
        /// xml represntaion of the info
        /// </returns>
        public string Info()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                try
                {
                    var info = new ControllerInfo
                        {
                            Name = Controller.Name, 
                            Alarm = Language.GetFrendyName(Controller.Info.Alarm), 
                            LastUpdateString = Controller.Info.LastUpdate.ToString(), 
                            LastUpdate = Controller.Info.LastUpdate, 
                            Model = Controller.Info.Model, 
                            OperationMode = Language.GetFrendyName(Controller.Info.OperationMode), 
                            ModeId = (int)Controller.Info.OperationMode, 
                            SerialNumber = Controller.Info.SerialNumber.ToString(), 
                            SoftwareDate = Controller.Info.SoftwareDate.ToShortDateString(), 
                            SoftwareVersion = Controller.Info.SoftwareVersion.ToString("0.00"), 
                            MoonPhase = Controller.Info.MoonPhase + "%", 
                            Latitude = Controller.Info.Latitude + "°", 
                            Longitude = Controller.Info.Longitude + "°", 
                            ReminderCount = Controller.Info.Reminders.Count,
                            ProbeCount = Controller.Items.OfType<Probe>().Count(),
                            LevelSensorCount = Controller.Items.OfType<LevelSensor>().Count(),
                            DigitalInputCount = Controller.Items.OfType<DigitalInput>().Count(),
                            SPortCount = Controller.Items.OfType<SPort>().Count(),
                            LPortCount = Controller.Items.OfType<LPort>().Count(), 
                            LightCount = Controller.Items.OfType<Light>().Count(), 
                            DosingPumpCount = Controller.Items.OfType<DosingPump>().Count(), 
                            UserValueCount = Controller.Items.OfType<UserInfo>().Count(), 
                            ProbeAlarm = Controller.Items.OfType<Probe>().Any(probe => probe.IsAlarmOn == CurrentState.On), 
                            LevelAlarm =
                                Controller.Items.OfType<LevelSensor>().Any(item => item.IsAlarmOn == CurrentState.On), 
                            ReminderOverdue = Controller.Info.Reminders.Any(reminder => reminder.IsOverdue)
                        };

                    bool isJson = false;
                    if (this.Paramaters.ContainsKey("output"))
                    {
                        isJson = this.Paramaters["output"] == "json";
                    }

                    if (isJson)
                    {
                        string callback = string.Empty;
                        if (this.Paramaters.ContainsKey("callback"))
                        {
                            callback = this.Paramaters["callback"];
                        }

                        this.Response.ContentType = "text/javascript";
                        string data = JsonConvert.SerializeObject(info, Formatting.None);

                        if (string.IsNullOrEmpty(callback))
                        {
                            return data;
                        }

                        return string.Format("{0}({1});", callback, data);
                    }

                    TextWriter writer = new StringWriter(CultureInfo.CurrentCulture);
                    var serializer = new XmlSerializer(typeof(ControllerInfo));
                    serializer.Serialize(writer, info);
                    writer.Flush();
                    this.Response.ContentType = MediaTypeNames.Text.Xml;
                    return writer.ToString();
                }
                catch (ReefStatusException ex)
                {
                    Logger.Instance.LogError(ex);
                }
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Keys this instance.
        /// </summary>
        /// <returns>
        /// empty string
        /// </returns>
        public string Key()
        {
            try
            {
                if (!this.IsLocked && this.Paramaters.ContainsKey("command") && DefaultController != null)
                {
                    Controller Controller = DefaultController;
                    if (this.Paramaters.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(this.Paramaters["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    string command = this.Paramaters["command"];

                    if (command == "up")
                    {
                        Controller.Commands.KeyCommand(FaceplateKey.Up);
                    }
                    else if (command == "down")
                    {
                        Controller.Commands.KeyCommand(FaceplateKey.Down);
                    }
                    else if (command == "right")
                    {
                        Controller.Commands.KeyCommand(FaceplateKey.Right);
                    }
                    else if (command == "left")
                    {
                        Controller.Commands.KeyCommand(FaceplateKey.Left);
                    }
                    else if (command == "esc")
                    {
                        Controller.Commands.KeyCommand(FaceplateKey.Esc);
                    }
                    else if (command == "enter")
                    {
                        Controller.Commands.KeyCommand(FaceplateKey.Enter);
                    }
                    else
                    {
                        return this.FormatResult(false);
                    }
                }

                return this.Display();
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Socketses this instance.
        /// </summary>
        /// <returns>
        /// xml represntaion of the L Ports
        /// </returns>
        public string LPort()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                return this.FormatResult(Controller.Items.OfType<LPort>());
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Sensors this instance.
        /// </summary>
        /// <returns>
        /// xml representation of the info
        /// </returns>
        public string LevelSensors()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                return this.FormatResult(Controller.Items.OfType<LevelSensor>());
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Gets a list of the lights in XML
        /// </summary>
        /// <returns>
        /// XML string
        /// </returns>
        public string Lights()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                return this.FormatResult(Controller.Items.OfType<Light>());
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Locks this instance.
        /// </summary>
        /// <returns>
        /// true if it is locked or false if it is not locked
        /// </returns>
        public string Lock()
        {
            if (ReefStatusSettings.Instance.Web.Protection)
            {
                WebSession.GetCurrent(this.SessionId).IsLocked = true;
            }

            return this.FormatResult(!this.IsLocked);
        }

        /// <summary>
        /// Locks the enabled.
        /// </summary>
        /// <returns>
        /// String "True" if the lock is enabled
        /// </returns>
        public string LockEnabled()
        {
            return this.FormatResult(ReefStatusSettings.Instance.Web.Protection);
        }

        /// <summary>
        /// Maintenances this instance.
        /// </summary>
        /// <returns>
        /// Ok if succeded
        /// </returns>
        public string Maintenance()
        {
            try
            {
                if (!this.IsLocked && DefaultController != null)
                {
                    Controller Controller = DefaultController;
                    if (this.Paramaters.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(this.Paramaters["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    bool enable = true;
                    if (this.Paramaters.ContainsKey("enable"))
                    {
                        enable = bool.Parse(this.Paramaters["enable"]);
                    }

                    Controller.Commands.SendMaintenance(enable, Controller.Info.Maintenance[0]);
                    return this.FormatResult(true);
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Manuals the lights.
        /// </summary>
        /// <returns>
        /// Ok if succreded
        /// </returns>
        public string ManualLights()
        {
            try
            {
                if (!this.IsLocked && DefaultController != null)
                {
                    Controller Controller = DefaultController;
                    if (this.Paramaters.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(this.Paramaters["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    bool enable = true;
                    if (this.Paramaters.ContainsKey("enable"))
                    {
                        enable = bool.Parse(this.Paramaters["enable"]);
                    }

                    Controller.Commands.SendOperationMode(
                        enable ? OperationMode.ManualIllumination : OperationMode.Normal);
                    return this.FormatResult(true);
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// enables/disables Manual sockets.
        /// </summary>
        /// <returns>
        /// ok if succeded
        /// </returns>
        public string ManualSockets()
        {
            try
            {
                if (!this.IsLocked && DefaultController != null)
                {
                    Controller Controller = DefaultController;
                    if (this.Paramaters.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(this.Paramaters["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    bool enable = true;
                    if (this.Paramaters.ContainsKey("enable"))
                    {
                        enable = bool.Parse(this.Paramaters["enable"]);
                    }

                    Controller.Commands.SendOperationMode(enable ? OperationMode.ManualSockets : OperationMode.Normal);
                    return this.FormatResult(true);
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Sensorses this instance.
        /// </summary>
        /// <returns>
        /// xml represntaion of the probes
        /// </returns>
        public string Probes()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                return this.FormatResult(Controller.Items.OfType<Probe>());
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        /// <returns>
        /// if the commnd is ok
        /// </returns>
        public string Refresh()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                Controller.Commands.UpdateAll();
            }

            return this.FormatResult(true);
        }

        /// <summary>
        /// Gets the Reminders in XML
        /// </summary>
        /// <returns>
        /// XML String
        /// </returns>
        public string Reminders()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                bool isJson = false;
                if (this.Paramaters.ContainsKey("output"))
                {
                    isJson = this.Paramaters["output"] == "json";
                }

                if (isJson)
                {
                    string callback = string.Empty;
                    if (this.Paramaters.ContainsKey("callback"))
                    {
                        callback = this.Paramaters["callback"];
                    }

                    return this.GetJsonReminders(Controller.Info.Reminders, callback);
                }

                return this.GetXmlReminders(Controller.Info.Reminders);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Resets the reminder.
        /// </summary>
        /// <returns>
        /// Ok if the reminder was reset
        /// </returns>
        public string ResetReminder()
        {
            try
            {
                if (!this.IsLocked && this.Paramaters.ContainsKey("index"))
                {
                    Dictionary<string, string> param = this.Paramaters;

                    string index = param["index"];

                    Controller Controller = DefaultController;
                    if (param.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(param["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    Common.ProfiLux.Reminder reminder =
                        Controller.Info.Reminders.FirstOrDefault(item => item.Index.ToString() == index);
                    if (reminder != null)
                    {
                        Controller.Commands.SendResetReminder(reminder);
                    }

                    return this.FormatResult(true);
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Socketses this instance.
        /// </summary>
        /// <returns>
        /// xml represntaion of the S Ports
        /// </returns>
        public string SPort()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                return this.FormatResult(Controller.Items.OfType<SPort>());
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Sets the socket.
        /// </summary>
        /// <returns>
        /// OK if succeded
        /// </returns>
        public string SetLight()
        {
            try
            {
                if (!this.IsLocked && this.Paramaters.ContainsKey("type"))
                {
                    Dictionary<string, string> param = this.Paramaters;

                    string type = param["type"];

                    Controller Controller = DefaultController;
                    if (param.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(param["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    BaseInfo infoItem = Controller.Items.FirstOrDefault(item => item.Id == type);
                    if (infoItem is Light)
                    {
                        int value = 0;
                        if (this.Paramaters.ContainsKey("value"))
                        {
                            if (!int.TryParse(this.Paramaters["value"], out value))
                            {
                                value = 0;
                            }
                        }

                        Controller.Commands.SendLightState((Light)infoItem, value);
                    }

                    return this.FormatResult(true);
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Sets the socket.
        /// </summary>
        /// <returns>
        /// OK if succeded
        /// </returns>
        public string SetSocket()
        {
            try
            {
                if (!this.IsLocked && this.Paramaters.ContainsKey("type"))
                {
                    Dictionary<string, string> param = this.Paramaters;

                    string type = param["type"];

                    Controller Controller = DefaultController;
                    if (param.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(param["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    BaseInfo infoItem = Controller.Items.FirstOrDefault(item => item.Id == type);
                    if (infoItem is SPort)
                    {
                        bool enable;
                        if (this.Paramaters.ContainsKey("enable"))
                        {
                            enable = bool.Parse(this.Paramaters["enable"]);
                        }
                        else
                        {
                            enable = ((CurrentState)infoItem.Value) == CurrentState.Off;
                        }

                        Controller.Commands.SendSocketState((SPort)infoItem, enable);
                    }

                    return this.FormatResult(true);
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Starts the water change.
        /// </summary>
        /// <returns>
        /// OK if succeded
        /// </returns>
        public string StartWaterChange()
        {
            try
            {
                if (!this.IsLocked && this.Paramaters.ContainsKey("type"))
                {
                    Dictionary<string, string> param = this.Paramaters;

                    string type = param["type"];

                    Controller Controller = DefaultController;
                    if (param.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(param["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    BaseInfo infoItem = Controller.Items.FirstOrDefault(item => item.Id == type);
                    if (infoItem is LevelSensor)
                    {
                        Controller.Commands.SendStartWaterChange((LevelSensor)infoItem);
                    }

                    return this.FormatResult(true);
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Gets the stats
        /// </summary>
        /// <returns>
        /// the XML text for the stats
        /// </returns>
        public string Stats()
        {
            try
            {
                Dictionary<string, string> param = this.Paramaters;
                if (param.ContainsKey("type"))
                {
                    string type = param["type"];

                    string range = "all";
                    if (param.ContainsKey("range"))
                    {
                        range = param["range"];
                    }

                    Controller Controller = DefaultController;
                    if (param.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(param["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    Stats stats;

                    Collection<DataPoint> data;
                    using (var dataAccess = ReefStatusSettings.Instance.Logging.Connection.Create())
                    {
                        data = new Collection<DataPoint>(dataAccess.GetDataPoints(type, true, Controller.Id));
                    }

                    if (range == "year")
                    {
                        stats = MemoryDataAccess.GetStats(DateTime.Now, DateTime.Now.AddYears(-1), data, true);
                    }
                    else if (range == "month")
                    {
                        stats = MemoryDataAccess.GetStats(DateTime.Now, DateTime.Now.AddMonths(-1), data, true);
                    }
                    else if (range == "week")
                    {
                        stats = MemoryDataAccess.GetStats(DateTime.Now, DateTime.Now.AddDays(-7), data, true);
                    }
                    else if (range == "day")
                    {
                        stats = MemoryDataAccess.GetStats(DateTime.Now, DateTime.Now.AddDays(-1), data, true);
                    }
                    else
                    {
                        stats = MemoryDataAccess.GetStats(DateTime.Now, DateTime.Now.AddYears(-100), data, true);
                    }

                    BaseInfo graphSetting = Controller.Items.FirstOrDefault(item => item.Id == type);
                   stats.ApplyConverter(graphSetting);

                    bool isJson = false;
                    if (this.Paramaters.ContainsKey("output"))
                    {
                        isJson = this.Paramaters["output"] == "json";
                    }

                    if (isJson)
                    {
                        string callback = string.Empty;
                        if (this.Paramaters.ContainsKey("callback"))
                        {
                            callback = this.Paramaters["callback"];
                        }

                        this.Response.ContentType = "text/javascript";
                        string jsonData = JsonConvert.SerializeObject(stats, Formatting.None);

                        if (string.IsNullOrEmpty(callback))
                        {
                            return jsonData;
                        }

                        return string.Format("{0}({1});", callback, data);
                    }

                    TextWriter writer = new StringWriter(CultureInfo.CurrentCulture);
                    var serializer = new XmlSerializer(typeof(Stats));
                    serializer.Serialize(writer, stats);
                    writer.Flush();
                    this.Response.ContentType = MediaTypeNames.Text.Xml;
                    return writer.ToString();
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Thunderstorms this instance.
        /// </summary>
        /// <returns>
        /// Ok if succeded
        /// </returns>
        public string StatusEmail()
        {
            try
            {
                if (!this.IsLocked)
                {
                    string message = string.Empty;
                    if (this.Paramaters.ContainsKey("message"))
                    {
                        message = this.Paramaters["message"];
                    }

                    DataService.SendStatusEmail(message);

                    return this.FormatResult(true);
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Thunderstorms this instance.
        /// </summary>
        /// <returns>
        /// Ok if succeded
        /// </returns>
        public string Thunderstorm()
        {
            try
            {
                if (!this.IsLocked && DefaultController != null)
                {
                    Controller Controller = DefaultController;
                    if (this.Paramaters.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(this.Paramaters["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    int duration = 5;
                    if (this.Paramaters.ContainsKey("duration"))
                    {
                        duration = int.Parse(this.Paramaters["duration"]);
                    }

                    Controller.Commands.ThunderStorm(duration);
                    return this.FormatResult(true);
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Unlocks this instance.
        /// </summary>
        /// <returns>
        /// true if it is locked or false if it is not locked
        /// </returns>
        public string Unlock()
        {
            if (this.Paramaters.ContainsKey("password"))
            {
                this.Unlock(this.Paramaters["password"]);
            }

            return this.FormatResult(!this.IsLocked);
        }

        /// <summary>
        /// Updates the dousing value.
        /// </summary>
        /// <returns>
        /// true or false
        /// </returns>
        public string UpdateDousingValue()
        {
            try
            {
                Dictionary<string, string> param = this.Paramaters;
                if (!this.IsLocked && param.ContainsKey("type") && param.ContainsKey("perday") &&
                    param.ContainsKey("rate"))
                {
                    string type = param["type"];

                    Controller Controller = DefaultController;
                    if (param.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(param["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    int perDay;
                    int rate;
                    if (int.TryParse(param["perday"], out perDay) && int.TryParse(param["rate"], out rate))
                    {
                        BaseInfo infoItem = Controller.Items.FirstOrDefault(item => item.Id == type);
                        if (infoItem is DosingPump)
                        {
                            Controller.Commands.SendUpdateDosingRate((DosingPump)infoItem, rate, perDay);
                            return this.FormatResult(true);
                        }
                    }
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Updates the user value.
        /// </summary>
        /// <returns>
        /// True or false
        /// </returns>
        public string UpdateUserValue()
        {
            try
            {
                Dictionary<string, string> param = this.Paramaters;
                if (!this.IsLocked && this.Paramaters.ContainsKey("type") && param.ContainsKey("value"))
                {
                    string type = param["type"];

                    Controller Controller = DefaultController;
                    if (param.ContainsKey("Controller"))
                    {
                        int ControllerId = int.Parse(param["Controller"]);
                        Controller = ReefStatusSettings.Instance.GetController(ControllerId);
                    }

                    double value;
                    if (double.TryParse(param["value"], out value))
                    {
                        BaseInfo infoItem = Controller.Items.FirstOrDefault(item => item.Id == type);
                        var userInfo = infoItem as UserInfo;
                        if (userInfo != null)
                        {
                            userInfo.Time = DateTime.Now;
                            userInfo.Value = value;

                            using (IDataAccess access = ReefStatusSettings.Instance.Logging.Connection.Create())
                            {
                                access.InsertItem(
                                    (double)userInfo.Value, userInfo.Time, userInfo.Id, false, userInfo.Controller.Id, null);
                            }

                            return this.FormatResult(true);
                        }
                    }
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Gets the User values.
        /// </summary>
        /// <returns>
        /// xml user values
        /// </returns>
        public string UserValues()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                return this.FormatResult(Controller.Items.OfType<UserInfo>());
            }

            return this.FormatResult(false);
        }

        /// <summary>
        /// Displays this instance.
        /// </summary>
        /// <returns>
        /// the display information string
        /// </returns>
        public string ViewString()
        {
            Controller Controller = DefaultController;
            if (this.Paramaters.ContainsKey("Controller"))
            {
                int ControllerId = int.Parse(this.Paramaters["Controller"]);
                Controller = ReefStatusSettings.Instance.GetController(ControllerId);
            }

            if (Controller != null)
            {
                try
                {
                    bool isJson = false;
                    if (this.Paramaters.ContainsKey("output"))
                    {
                        isJson = this.Paramaters["output"] == "json";
                    }

                    string callback = string.Empty;
                    if (this.Paramaters.ContainsKey("callback"))
                    {
                        callback = this.Paramaters["callback"];
                    }

                    string[] display = Controller.Commands.GetViewText();

                    if (isJson)
                    {
                        this.Response.ContentType = "text/javascript";

                        string data = JsonConvert.SerializeObject(display, Formatting.None);

                        if (string.IsNullOrEmpty(callback))
                        {
                            return data;
                        }

                        return string.Format("{0}({1});", callback, data);
                    }

                    TextWriter writer = new StringWriter(CultureInfo.CurrentCulture);
                    var serializer = new XmlSerializer(typeof(string[]));
                    serializer.Serialize(writer, display);
                    writer.Flush();
                    this.Response.ContentType = MediaTypeNames.Text.Xml;
                    return writer.ToString();
                }
                catch (ReefStatusException ex)
                {
                    Logger.Instance.LogError(ex);
                }
            }

            return string.Empty;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the params.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The paramaters.
        /// </returns>
        private static Dictionary<string, string> CreateParams(string id)
        {
            var paramaters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(id))
            {
                string[] items = id.Split('&');
                foreach (string item in items)
                {
                    string[] parm = item.Split('=');
                    if (parm.Length == 2)
                    {
                        paramaters.Add(parm[0], parm[1]);
                    }
                }
            }

            return paramaters;
        }

        /// <summary>
        /// Gets the datapoints.
        /// </summary>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <param name="endTime">
        /// The end time.
        /// </param>
        /// <param name="type">
        /// The type of the datapoint.
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// A list of datapoints
        /// </returns>
        private static Collection<DataPoint> GetDatapoints(
            DateTime startTime, DateTime endTime, string type, Controller Controller)
        {
            using (IDataAccess dataAccess = ReefStatusSettings.Instance.Logging.Connection.Create())
            {
                var datapoints = dataAccess.GetDataPoints(startTime, endTime, type, Controller.Id);

                BaseInfo graphSetting = Controller.Items.FirstOrDefault(item => item.Id == type);
                if (graphSetting is Probe)
                {
                    foreach (var dataPoint in datapoints)
                    {
                        dataPoint.Value = ((Probe)graphSetting).ConvertValue(dataPoint.Value);
                    }
                }

                return datapoints;
            }
        }

        /// <summary>
        /// Gets the datapoints.
        /// </summary>
        /// <param name="range">
        /// The range.
        /// </param>
        /// <param name="type">
        /// The type of the data point.
        /// </param>
        /// <param name="Controller">
        /// The Controller id.
        /// </param>
        /// <returns>
        /// the datapoints
        /// </returns>
        private static Collection<DataPoint> GetDatapoints(string range, string type, Controller Controller)
        {
            BaseInfo graphSetting = Controller.Items.FirstOrDefault(item => item.Id == type);

            Collection<DataPoint> datapoints;
            using (var dataAccess = ReefStatusSettings.Instance.Logging.Connection.Create())
            {
                if (range == "year")
                {
                    datapoints = dataAccess.GetDataPoints(
                        graphSetting.GraphId, DateTime.Now.AddYears(-1), false, Controller.Id);
                }
                else if (range == "month")
                {
                    datapoints = dataAccess.GetDataPoints(
                        graphSetting.GraphId, DateTime.Now.AddMonths(-1), false, Controller.Id);
                }
                else if (range == "week")
                {
                    datapoints = dataAccess.GetDataPoints(
                        graphSetting.GraphId, DateTime.Now.AddDays(-7), false, Controller.Id);
                }
                else if (range == "day")
                {
                    datapoints = dataAccess.GetDataPoints(
                        graphSetting.GraphId, DateTime.Now.AddDays(-1), false, Controller.Id);
                }
                else
                {
                    datapoints = dataAccess.GetDataPoints(graphSetting.GraphId, false, Controller.Id);
                }
            }

            if (graphSetting is Probe)
            {
                foreach (DataPoint dataPoint in datapoints)
                {
                    dataPoint.Value = ((Probe)graphSetting).ConvertValue(dataPoint.Value);
                }
            }

            return datapoints;
        }

        /// <summary>
        /// Formats the result.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <returns>
        /// the formated result
        /// </returns>
        private string FormatResult(IEnumerable items)
        {
            bool isJson = false;
            if (this.Paramaters.ContainsKey("output"))
            {
                isJson = this.Paramaters["output"] == "json";
            }

            if (isJson)
            {
                string callback = string.Empty;
                if (this.Paramaters.ContainsKey("callback"))
                {
                    callback = this.Paramaters["callback"];
                }

                return this.GetJson(items, callback);
            }

            return this.GetXml(items);
        }

        /// <summary>
        /// Formats the result.
        /// </summary>
        /// <param name="result">
        /// if set to <c>true</c> [result].
        /// </param>
        /// <returns>
        /// the result in string format
        /// </returns>
        private string FormatResult(bool result)
        {
            bool isJson = false;
            if (this.Paramaters.ContainsKey("output"))
            {
                isJson = this.Paramaters["output"] == "json";
            }

            if (isJson)
            {
                string callback = string.Empty;
                if (this.Paramaters.ContainsKey("callback"))
                {
                    callback = this.Paramaters["callback"];
                }

                this.Response.ContentType = "text/javascript";

                string data = JsonConvert.SerializeObject(result, Formatting.None);

                if (string.IsNullOrEmpty(callback))
                {
                    return data;
                }

                return string.Format("{0}({1});", callback, data);
            }

            this.Response.ContentType = MediaTypeNames.Text.Html;
            return result.ToString();
        }

        /// <summary>
        /// Gets the json.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// the json Result
        /// </returns>
        private string GetJson(IEnumerable items, string callback)
        {
            try
            {
                var webdata = new Collection<WebData>();
                foreach (BaseInfo value in items)
                {
                    webdata.Add(new WebData(value));
                }

                this.Response.ContentType = "text/javascript";
                string data = JsonConvert.SerializeObject(webdata, Formatting.None);

                if (string.IsNullOrEmpty(callback))
                {
                    return data;
                }

                return string.Format("{0}({1});", callback, data);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the json reminders.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// the Json result
        /// </returns>
        private string GetJsonReminders(IEnumerable items, string callback)
        {
            try
            {
                var reminders = new Collection<Reminder>();
                foreach (Common.ProfiLux.Reminder value in items)
                {
                    reminders.Add(
                        new Reminder
                            {
                                Index = value.Index, 
                                Text = value.Text, 
                                IsOverdue = value.IsOverdue, 
                                Date = value.Next.ToShortDateString()
                            });
                }

                this.Response.ContentType = "text/javascript";
                string data = JsonConvert.SerializeObject(reminders, Formatting.None);

                if (string.IsNullOrEmpty(callback))
                {
                    return data;
                }

                return string.Format("{0}({1});", callback, data);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the XML.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <returns>
        /// the xml
        /// </returns>
        private string GetXml(IEnumerable items)
        {
            try
            {
                var data = new Collection<WebData>();
                foreach (BaseInfo value in items)
                {
                    data.Add(new WebData(value));
                }

                TextWriter writer = new StringWriter(CultureInfo.CurrentCulture);
                var serializer = new XmlSerializer(typeof(Collection<WebData>));
                serializer.Serialize(writer, data);
                writer.Flush();
                this.Response.ContentType = MediaTypeNames.Text.Xml;
                return writer.ToString();
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the XML.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <returns>
        /// the xml
        /// </returns>
        private string GetXmlReminders(IEnumerable items)
        {
            try
            {
                var data = new Collection<Reminder>();
                foreach (Common.ProfiLux.Reminder value in items)
                {
                    data.Add(
                        new Reminder
                            {
                                Index = value.Index, 
                                Text = value.Text, 
                                IsOverdue = value.IsOverdue, 
                                Date = value.Next.ToShortDateString()
                            });
                }

                TextWriter writer = new StringWriter(CultureInfo.CurrentCulture);
                var serializer = new XmlSerializer(typeof(Collection<Reminder>));
                serializer.Serialize(writer, data);
                writer.Flush();
                this.Response.ContentType = MediaTypeNames.Text.Xml;
                return writer.ToString();
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Unlocks the specified password.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        private void Unlock(string password)
        {
            if (ReefStatusSettings.Instance.Web.Protection)
            {
                WebSession.GetCurrent(this.SessionId).IsLocked = ReefStatusSettings.Instance.Web.Password != password;
            }
        }

        #endregion
    }
}