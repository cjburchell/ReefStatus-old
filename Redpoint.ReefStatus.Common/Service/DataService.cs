// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Service
{
    using System;
    using System.Threading;

    using RedPoint.ReefStatus.Common.Commands;
    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;

    /// <summary>
    ///     Data Service
    /// </summary>
    public class DataService
    {
        private readonly LoggingSettings settings;

        private readonly IProtocolCommands commands;

        private readonly IController controller;

        private readonly IDataAccess dataBase;

        private readonly IAlertService alertService;

        /// <summary>
        ///     The timer log.
        /// </summary>
        private Timer timerLog;

        private Timer hourLog;

        private Timer dayLog;

        public DataService(LoggingSettings settings, IProtocolCommands commands, IController controller, IDataAccess dataBase, IAlertService alertService)
        {
            this.settings = settings;
            this.commands = commands;
            this.controller = controller;
            this.dataBase = dataBase;
            this.alertService = alertService;
        }

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        public void Start()
        {
            var autoEvent = new AutoResetEvent(false);
            this.timerLog = new Timer(this.TimerLogTick, autoEvent, 0, this.settings.LogInterval * 60000);

            this.hourLog = new Timer(this.TimerHourLogTick, autoEvent, 0, 3600000);

            this.dayLog = new Timer(this.TimerDayLogTick, autoEvent, 0, 86400000);
        }

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.timerLog.Dispose();
            this.timerLog = null;

            this.hourLog.Dispose();
            this.hourLog = null;

            this.dayLog.Dispose();
            this.dayLog = null;
        }

        /// <summary>
        ///     Updates the settings.
        /// </summary>
        public void UpdateSettings()
        {
            this.timerLog?.Change(0, this.settings.LogInterval * 60000);
        }

        /// <summary>
        ///     Handles the Tick event of the timerLog control.
        /// </summary>
        /// <param name="sender">
        ///     The source of the event.
        /// </param>
        private void TimerLogTick(object sender)
        {
            try
            {
                this.commands.LogParameters();
                this.dataBase.AddLog(this.controller, DateTime.Now, null);
                this.alertService.CheckAlarm();
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        private void TimerHourLogTick(object sender)
        {
            try
            {
                this.dataBase.AddHourLog(this.controller, DateTime.Now);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        private void TimerDayLogTick(object sender)
        {
            try
            {
                this.dataBase.AddDayLog(this.controller, DateTime.Now);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        
    }
}