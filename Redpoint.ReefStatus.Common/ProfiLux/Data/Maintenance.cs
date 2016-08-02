namespace RedPoint.ReefStatus.Common.ProfiLux
{
    public class Maintenance
    {
        private bool isActive;

        public Maintenance(int index)
        {
            this.Index = index;
        }

        public int Index { get; set; }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                if (this.isActive == value)
                {
                    return;
                }

                if (value)
                {
                    this.TimeLeft = this.Duration;
                }

                this.isActive = value;
            }
        }

        public int Duration { get; set; }

        public int TimeOn => this.Duration - this.TimeLeft;

        public int TimeLeft { get; set; }
    }
}