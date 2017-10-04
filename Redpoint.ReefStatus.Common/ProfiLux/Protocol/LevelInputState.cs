
namespace RedPoint.ReefStatus.Common.ProfiLux.Protocol
{
    public class LevelInputState
    {
        public CurrentState Delayed { get; set; }
        public CurrentState Previous { get; set; }
        public CurrentState Undelayed { get; set; }
    }
}
