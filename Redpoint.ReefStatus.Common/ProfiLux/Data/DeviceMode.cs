namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System.ComponentModel;

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
}