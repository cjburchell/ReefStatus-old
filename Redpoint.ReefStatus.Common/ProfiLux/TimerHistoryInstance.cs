namespace RedPoint.ReefStatus.Common.ProfiLux
{
    public class TimerHistoryInstance
    {
        public TimerHistoryInstance(string outletName, string recording)
        {
            this.OutletName = outletName;
            this.Recording = recording;
        }

        public string OutletName { get; set; }
        public string Recording { get; set; }

    }
}
