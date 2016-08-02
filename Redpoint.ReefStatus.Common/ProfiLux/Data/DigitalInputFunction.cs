namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System.ComponentModel;

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
}