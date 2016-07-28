namespace RedPoint.ReefStatus.Common.WebServer
{
    using System.Collections.ObjectModel;
    using System.Drawing;

    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// Web data
    /// </summary>
    public class WebGraphData
    {
        public string Units { get; set; }

        public Color Colour { get; set; }

        public string DisplayName { get; set; }

        public string Id { get; set; }

        public Collection<DataPoint> Data { get; set; }
    }
}
