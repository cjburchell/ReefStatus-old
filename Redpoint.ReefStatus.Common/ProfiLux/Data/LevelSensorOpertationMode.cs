namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using System.ComponentModel;

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
}