namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.ComponentModel;

    public enum OperationMode
    {
        [Description("strNormal")]
        Normal = 0,
        [Description("strDiagnostics")]
        Diagnostics = 1,
        [Description("strLightTest")]
        LightTest = 3,
        [Description("strMaintenance")]
        Maintenance = 4,
        [Description("strManualSockets")]
        ManualSockets = 5,
        [Description("strManualIllumination")]
        ManualIllumination = 6,
        [Description("strCanTransparent")]
        CanTransparent = 7,
    }

    /// <summary>
    /// The Id's of all the Profilux products
    /// </summary>
    public enum ProductId
    {
        /// <summary>
        ///  Unknown Product ID
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// ProfiLux II
        /// </summary>
        ProfiLuxII = 2,

        /// <summary>
        /// ProfiLux Plus II
        /// </summary>
        ProfiLuxPlusII = 3,

        /// <summary>
        /// ProfiLux Plus II Ex
        /// </summary>
        ProfiLuxPlusIIEx = 4,

        /// <summary>
        /// ProfiLux II Ex
        /// </summary>
        ProfiLuxIITerra = 5,

        /// <summary>
        /// ProfiLux II Ex
        /// </summary>
        ProfiLuxIIEx = 6,

        /// <summary>
        /// ProfiLux Light II
        /// </summary>
        ProfiLuxLightII = 7,

        /// <summary>
        /// ProfiLux II Outdoor
        /// </summary>
        ProfiLuxIIOutdoor = 8,

        /// <summary>
        /// ProfiLux III
        /// </summary>
        ProfiLuxIII = 11,

        /// <summary>
        /// ProfiLux III Ex
        /// </summary>
        ProfiLuxIIIEx = 12,


        Neptune = 100,
    }

    /// <summary>
    /// Type of sensor
    /// </summary>
    public enum SensorType
    {
        /// <summary>
        /// No Sensor
        /// </summary>
        None = 0,

        /// <summary>
        /// Temperature sensor
        /// </summary>
        Temperature = 1,

        /// <summary>
        /// PH sensor
        /// </summary>
        PH = 2,

        /// <summary>
        /// Redox sensor
        /// </summary>
        Redox = 3,

        /// <summary>
        /// Freshwater Conductivity sensor
        /// </summary>
        ConductivityF = 4,

        /// <summary>
        /// Saltwater Conductivity sensor
        /// </summary>
        Conductivity = 5,

        /// <summary>
        /// Free sensor
        /// </summary>
        Free = 6,

        /// <summary>
        /// Humidity sensor
        /// </summary>
        Humidity = 7,

        /// <summary>
        /// Air Temperature sensor
        /// </summary>
        AirTemperature = 8,

        /// <summary>
        /// Oxygen sensor
        /// </summary>
        Oxygen = 9,

        /// <summary>
        /// Voltage Sensor
        /// </summary>
        Voltage = 10,

        /// <summary>
        /// Level sensor
        /// </summary>
        Level = 30,

        DigitalInput = 31
    }

    public enum DigitalInputFunction
    {
        [Description("strNotUsed")]
        NotUsed,
        [Description("strLevelSensor")]
        LevelSensor,
        [Description("strWaterChange")]
        WaterChange,
        [Description("strMaintenance")]
        Maintenance,
        [Description("strFeedingPause")]
        FeedingPause,
        [Description("strThunderstorm")]
        Thunderstorm
    }

    public enum LevelSensorOpertationMode
    {
        [Description("strNotEnabled")]
        NotEnabled = 0,

        [Description("strAutoTopOff")]
        AutoTopOff = 1,

        [Description("strMinMaxControl")]
        MinMaxControl = 2,

        [Description("strWaterChange")]
        WaterChange = 3,

        [Description("strLeekageDetection")]
        LeekageDetection = 4,

        [Description("strWaterChangeAndAutoTopOff")]
        WaterChangeAndAutoTopOff = 5,

        [Description("strAutoTopOffWith2Sensors")]
        AutoTopOffWith2Sensors = 6,

        [Description("strReturnPump")]
        ReturnPump = 7
    }

    /// <summary>
    /// The mode of the sensor
    /// </summary>
    public enum SensorMode
    {
        /// <summary>
        /// Normal Sensor mode
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Avrage Virtual Probe
        /// </summary>
        Average = 1,

        /// <summary>
        /// Copy Virtual Probe
        /// </summary>
        Copy = 4
    }

    /// <summary>
    /// the type of device
    /// </summary>
    public enum DeviceMode
    {
        /// <summary>
        /// Light Device
        /// </summary>
        [Description("strLights")]
        Lights = 0,

        /// <summary>
        /// Timer Device
        /// </summary>
        [Description("strTimer")]
        Timer = 1,

        /// <summary>
        /// Decrease sensor device
        /// </summary>
        [Description("strDecrease")]
        Decrease = 2,

        /// <summary>
        /// Increase sensor device
        /// </summary>
        [Description("strIncrease")]
        Increase = 3,

        /// <summary>
        /// Substrate sensor device
        /// </summary>
        [Description("strSubstrate")]
        Substrate = 4,

        /// <summary>
        /// Alarm Probe device
        /// </summary>
        [Description("strProbeAlarm")]
        ProbeAlarm = 5,

        /// <summary>
        /// Water device
        /// </summary>
        [Description("strWater")]
        Water = 6,

        /// <summary>
        /// Powerhead device
        /// </summary>
        [Description("strCurrentPump")]
        CurrentPump = 7,

        Unused8 = 8,

        /// <summary>
        /// Programmable Logic
        /// </summary>
        [Description("strProgrammableLogic")]
        ProgrammableLogic = 9,

        /// <summary>
        /// Variable Illumination
        /// </summary>
        [Description("strVariableIllumination")]
        VariableIllumination = 10,

        [Description("strTempPTC")]
        TempPTC = 11,

        [Description("strDigtialInput")]
        DigtialInput = 12,


        [Description("strMaintenance")]
        Maintenance = 13,

        Unused14 = 14,
        Unused15 = 15,
        Unused16 = 16,
        Unused17 = 17,
        Unused18 = 18,
        Unused19 = 19,
        Unused20 = 20,
        Unused21 = 21,
        Unused22 = 22,
        Unused23 = 23,
        Unused24 = 24,

        [Description("strThunderStorm")]
        ThunderStorm = 25,

        /// <summary>
        /// Water Change
        /// </summary>
        [Description("strWaterChange")]
        WaterChange = 26,

        /// <summary>
        /// Filter Device
        /// </summary>
        [Description("strFilter")]
        Filter = 27,

        /// <summary>
        /// Alarm Device
        /// </summary>
        [Description("strAlarm")]
        Alarm = 28,

        /// <summary>
        /// Device is Always On
        /// </summary>
        [Description("strAlwaysOn")]
        AlwaysOn = 29,

        /// <summary>
        /// Device is Always Off
        /// </summary>
        [Description("strAlwaysOff")]
        AlwaysOff = 30,

        /// <summary>
        /// Thunder Device
        /// </summary>
        [Description("strThunder")]
        Thunder = 31
    }

    public enum LogicMode
    {
        And = 0,
        Or = 1,
        InvertAnd = 2,
        InvertOr = 3,
        Inverted = 4,
        Equal = 5,
        NoEqual = 6,
        Pulse = 7,
        DelayedOn = 8,
        DelayedOff = 9,
        FrequentPulses = 10,
        SRFlipFlop = 11,
        ExclusiveOr = 12
    }

}
