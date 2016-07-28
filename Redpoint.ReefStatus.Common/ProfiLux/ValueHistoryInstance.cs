namespace RedPoint.ReefStatus.Common.ProfiLux
{
    public class ValueHistoryInstance
    {
        public ValueHistoryInstance(string probeName, double recording)
        {
            this.ProbeName = probeName;
            this.Recording = recording;
        }

        public string ProbeName { get; set; }
        public double Recording { get; set; }
    }
}
