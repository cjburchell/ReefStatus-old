namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System.ComponentModel;

    /// <summary>
    /// Water Mode
    /// </summary>
    public enum WaterMode
    {
        [Description("strReady")]
        Ready = 0,

        [Description("strDraining")]
        Draining = 1,

        [Description("strFilling")]
        Filling = 2
    }
}