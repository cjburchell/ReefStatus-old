namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
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
}