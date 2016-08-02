// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProfilux.cs" company="Redpoint Apps">
//   2009
// </copyright>
// <summary>
//   The Profilux interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using RedPoint.ReefStatus.Common.ProfiLux.Data;

    /// <summary>
    /// The Profilux interface
    /// </summary>
    public interface IProfilux
    {
        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        double Version { get; }

        /// <summary>
        /// Gets the alarm.
        /// </summary>
        /// <value>The alarm.</value>
        CurrentState Alarm { get; }

        /// <summary>
        /// Gets the product id.
        /// </summary>
        /// <value>The product id.</value>
        ProductId ProductId { get; }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <value>The serial number.</value>
        int SerialNumber { get; }

        /// <summary>
        /// Gets the display text.
        /// </summary>
        /// <value>The display text.</value>
        string DisplayText { get; }

        /// <summary>
        /// Gets the view text.
        /// </summary>
        /// <value>The view text.</value>
        string[] View { get; }

        /// <summary>
        /// Gets the display text.
        /// </summary>
        /// <value>The display text.</value>
        string[] Display { get; }

        /// <summary>
        /// Gets the view text.
        /// </summary>
        /// <value>The view text.</value>
        string ViewText { get; }

        /// <summary>
        /// Gets the software date.
        /// </summary>
        /// <value>The software date.</value>
        DateTime SoftwareDate { get; }

        /// <summary>
        /// Gets the device address.
        /// </summary>
        /// <value>The device address.</value>
        int DeviceAddress { get; }

        /// <summary>
        /// Gets the module count.
        /// </summary>
        /// <value>The module count.</value>
        int ModuleCount { get; }

        /// <summary>
        /// Gets the reminder count.
        /// </summary>
        /// <value>The reminder count.</value>
        int ReminderCount { get; }

        /// <summary>
        /// Gets the sensor count.
        /// </summary>
        /// <value>The sensor count.</value>
        int SensorCount { get; }

        /// <summary>
        /// Gets the level senosr count.
        /// </summary>
        /// <value>The level senosr count.</value>
        int LevelSenosrCount { get; }

        /// <summary>
        /// Gets the S port count.
        /// </summary>
        /// <value>The S port count.</value>
        int SPortCount { get; }

        /// <summary>
        /// Gets the L port count.
        /// </summary>
        /// <value>The L port count.</value>
        int LPortCount { get; }

        /// <summary>
        /// Gets the digital input count.
        /// </summary>
        /// <value>The digital input count.</value>
        int DigitalInputCount { get; }

        /// <summary>
        /// Gets the programmable logic count.
        /// </summary>
        /// <value>
        /// The programmable logic count.
        /// </value>
        int ProgrammableLogicCount { get; }

        /// <summary>
        /// Gets the light count.
        /// </summary>
        /// <value>The light count.</value>
        int LightCount { get; }

        /// <summary>
        /// Gets the timer count.
        /// </summary>
        /// <value>The timer count.</value>
        int TimerCount { get; }

        /// <summary>
        /// Gets the current pump count.
        /// </summary>
        /// <value>The current pump count.</value>
        int CurrentPumpCount { get; }

        /// <summary>
        /// Gets or sets the op mode.
        /// </summary>
        /// <value>The op mode.</value>
        OperationMode OpMode { get; set; }

        /// <summary>
        /// Digitals the state of the input.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The current state of the input</returns>
        CurrentState GetDigitalInputState(int index);

        /// <summary>
        /// Gets the S port current.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns></returns>
        double GetSPortCurrent(int portIndex);

        /// <summary>
        /// Gets the S port function.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns>the function</returns>
        PortMode GetSPortFunction(int portIndex);

        /// <summary>
        /// Gets the S port value.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns>the value of the port</returns>
        int GetSPortValue(int portIndex);

        /// <summary>
        /// Gets the module version.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>Module Version</returns>
        int GetModuleVersion(int module);

        /// <summary>
        /// Gets the state of the module.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>Module State</returns>
        int GetModuleState(int module);

        /// <summary>
        /// Gets the module id.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>Module Id</returns>
        int GetModuleId(int module);

        /// <summary>
        /// Gets the reminder text.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        /// <returns>Reminder Text</returns>
        string GetReminderText(int reminder);

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Keys the command.
        /// </summary>
        /// <param name="facePlateKey">The face plate key.</param>
        void KeyCommand(FaceplateKey facePlateKey);

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="probes">The probes.</param>
        /// <returns>Data Points</returns>
        Collection<ItemDataRow> GetDataPoints(IProgressCallback callback, IEnumerable<Probe> probes);

        /// <summary>
        /// Thunderstorms the specified duration.
        /// </summary>
        /// <param name="duration">The duration.</param>
        void Thunderstorm(int duration);

        /// <summary>
        /// Performs a Water Change
        /// </summary>
        /// <param name="index">The index.</param>
        void WaterChange(int index);

        /// <summary>
        /// Sets the Feed Pause
        /// </summary>
        /// <param name="activate">if set to <c>true</c> [activate].</param>
        void FeedPause(bool activate);

        /// <summary>
        /// Sets the Maintenance Mode
        /// </summary>
        /// <param name="activate">if set to <c>true</c> [activate].</param>
        /// <param name="index">The index.</param>
        void Maintenace(bool activate, int index);

        /// <summary>
        /// Gets the days to next reminder.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        /// <returns>the number of days to the next</returns>
        DateTime? GetNextReminder(int reminder);

        /// <summary>
        /// Gets the type of the sensor.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>Sensor Type</returns>
        SensorType GetSensorType(int sensorIndex);

        /// <summary>
        /// Gets the program logic input1.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns></returns>
        PortMode GetProgramLogicInput(int input, int portIndex);

        /// <summary>
        /// Gets the program logic function.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns></returns>
        LogicFunction GetProgramLogicFunction(int portIndex);

        /// <summary>
        /// Gets the sensor format.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>Sensor Format</returns>
        int GetSensorFormat(int sensorIndex);

        /// <summary>
        /// Gets the sensor nominal value.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>Nominal Value</returns>
        int GetSensorNominalValue(int sensorIndex);

        /// <summary>
        /// Gets the sensor alarm deviation.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>Alarm Deviation</returns>
        int GetSensorAlarmDeviation(int sensorIndex);

        /// <summary>
        /// Gets the sensor alarm enable.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>If the Alarm is Enable</returns>
        bool GetSensorAlarmEnable(int sensorIndex);

        /// <summary>
        /// Gets the sensor value.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>Get Sensor Value</returns>
        int GetSensorValue(int sensorIndex);

        /// <summary>
        /// Gets the state of the level sensor.
        /// </summary>
        /// <param name="levelSensorIndex">Index of the level sensor.</param>
        /// <returns>Sensor state</returns>
        LevelState GetLevelSensorState(int levelSensorIndex);

        /// <summary>
        /// Gets the L port value.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns>LPort Value</returns>
        int GetLPortValue(int portIndex);

        /// <summary>
        /// Gets the L port function.
        /// </summary>
        /// <param name="portIndex">Index of the port.</param>
        /// <returns>LPort Funtion</returns>
        PortMode GetLPortFunction(int portIndex);

        /// <summary>
        /// Gets the digital input function.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>the funtion of the digital input</returns>
        DigitalInputFunction GetDigitalInputFunction(int sensorIndex);

        /// <summary>
        /// Gets the level sensor mode.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>The Opertational Mode</returns>
        LevelSensorOpertationMode GetLevelSensorMode(int sensorIndex);

        /// <summary>
        /// Gets the sensor alarm.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>true if the sensor is in alarm state</returns>
        CurrentState GetSensorAlarm(int sensorIndex);

        /// <summary>
        /// Gets the reminder period.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        /// <returns>the period</returns>
        int GetReminderPeriod(int reminder);

        /// <summary>
        /// Gets the reminder repeats.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        /// <returns>if the reminder repeats</returns>
        bool GetReminderRepeats(int reminder);

        /// <summary>
        /// Resets the reminder.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        /// <param name="period">The period.</param>
        void ResetReminder(int reminder, int period);

        /// <summary>
        /// Clears the reminder.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        void ClearReminder(int reminder);

        /// <summary>
        /// Determines whether [is light active] [the specified channel].
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>
        ///     <c>true</c> if [is light active] [the specified channel]; otherwise, <c>false</c>.
        /// </returns>
        bool IsLightActive(int channel);

        /// <summary>
        /// Gets the light value.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>the current value of the light channel</returns>
        double GetLightValue(int channel);

        /// <summary>
        /// Gets the dosing rate.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>the rate</returns>
        int GetDosingRate(int channel);

        /// <summary>
        /// Gets the timer mode.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>Timer Settings</returns>
        TimerSettings GetTimerSettings(int channel);

        /// <summary>
        /// Updates the timer settings.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="settings">The settings.</param>
        void UpdateTimerSettings(int channel, TimerSettings settings);

        /// <summary>
        /// Updates the dosing rate.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="rate">The rate.</param>
        void UpdateDosingRate(int channel, int rate);

        /// <summary>
        /// Clears the level alarm.
        /// </summary>
        /// <param name="index">The index.</param>
        void ClearLevelAlarm(int index);

        /// <summary>
        /// Gets the sensor mode.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>the mode of the sensor</returns>
        SensorMode GetSensorMode(int sensorIndex);

        /// <summary>
        /// Sets the state of the socket.
        /// </summary>
        /// <param name="portNumber">The port number.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        void SetSocketState(int portNumber, bool value);

        /// <summary>
        /// Sets the state of the light.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="value">The value.</param>
        void SetLightValue(int channel, double value);

        /// <summary>
        /// Determines whether [is light dimmable] [the specified channel].
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>
        ///     <c>true</c> if [is light dimmable] [the specified channel]; otherwise, <c>false</c>.
        /// </returns>
        bool IsLightDimmable(int channel);

        /// <summary>
        /// Gets the state of the current pump.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>the state of the pump</returns>
        int GetCurrentPumpValue(int index);

        /// <summary>
        /// Gets the probe operation hours.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The nuber of hours the probe is in opeeration</returns>
        int GetProbeOperationHours(int index);

        /// <summary>
        /// Sets the probe operation hours.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        void SetProbeOperationHours(int index, int value);

        /// <summary>
        /// Gets the light operation hours.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>>The nuber of hours the light is in opeeration</returns>
        int GetLightOperationHours(int channel);

        /// <summary>
        /// Sets the light operation hours.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="value">The value.</param>
        void SetLightOperationHours(int channel, int value);

        /// <summary>
        /// Gets the sensor id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="type">The type.</param>
        /// <returns>the sensor Id</returns>
        string GetSensorId(int index, SensorType type);

        /// <summary>
        /// Gets the level id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Id</returns>
        string GetLevelId(int index);

        /// <summary>
        /// Gets the digtial input id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Id</returns>
        string GetDigtialInputId(int index);

        /// <summary>
        /// Gets the S port id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Id</returns>
        string GetSPortId(int index);

        /// <summary>
        /// Gets the L port id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Id</returns>
        string GetLPortId(int index);

        /// <summary>
        /// Gets the light id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Id</returns>
        string GetLightId(int index);

        /// <summary>
        /// Gets the dousing pump id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Id</returns>
        string GetDousingPumpId(int index);

        /// <summary>
        /// Gets the current pump id.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>the Id</returns>
        string GetCurrentPumpId(int index);

        /// <summary>
        /// Gets the sensor active.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>If the sensor is active</returns>
        bool GetSensorActive(int index);

        /// <summary>
        /// Gets the web server port.
        /// </summary>
        /// <value>The web server port.</value>
        int WebServerPort { get; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>The latitude.</value>
        double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>The longitude.</value>
        double Longitude { get; set; }

        /// <summary>
        /// Gets the moon phase.
        /// </summary>
        /// <value>The moon phase.</value>
        double MoonPhase { get; }

        /// <summary>
        /// Gets the display image.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>The display image.</returns>
        System.Drawing.Image GetDisplayImage(int size);

        bool IsCurrentPumpAssinged(int i);

        /// <summary>
        /// Gets the name of the light.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        string GetLightName(int i);

        /// <summary>
        /// Gets the name of the S port.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        string GetSPortName(int i);

        /// <summary>
        /// Gets the name of the probe.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        string GetProbeName(int i);

        void SetLightName(int i, string text);

        void SetSPortName(int i, string text);

        void SetProbeName(int i, string text);

        bool GetMaintenanceIsActive(int p);

        int GetMaintenanceDuration(int index);

        int GetMaintenanceTimeLeft(int index);

        void SetLightTestValue(int value);

        int LightTestTime { get; }
    }
}
