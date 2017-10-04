namespace RedPoint.ReefStatus.Common.ProfiLux
{
    /// <summary>
    /// Level State
    /// </summary>
    public struct LevelState
    {
        public WaterMode WaterMode { get; set; }
        public CurrentState Alarm { get; set; }
        public CurrentState Drain { get; set; }
        public CurrentState Fill { get; set; }
    }
}