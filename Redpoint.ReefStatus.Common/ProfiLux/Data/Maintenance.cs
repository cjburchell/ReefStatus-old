namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    public class Maintenance
    {
        public Maintenance(int index)
        {
            this.Index = index;
            this.DisplayName = "Maintenance" + (index+1);
        }

        public Maintenance()
        {
        }

        public string DisplayName { get; set; }

        public int Index { get; set; }

        public bool IsActive { get; set; }

        public int Duration { get; set; }

        public int TimeLeft { get; set; }
    }
}