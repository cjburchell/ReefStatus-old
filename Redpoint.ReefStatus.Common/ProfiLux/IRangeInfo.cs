namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System.Drawing;

    public interface IRangeInfo
    {
        string Id { get; }

        object ConvertedValue { get; }

        double CenterValue { get; }

        double MaxRange { get; }

        double MinRange { get; }

        bool ShowMax { get; }

        bool ShowMin { get; }

        bool ShowCenter { get; }

        double ConvertValue(double value);

        Color HighRangeColour { get; }

        Color LowRangeColour { get; }

        Color NominalColour { get; }

        System.Windows.Media.Color HighRangeMediaColour { get; }

        System.Windows.Media.Color LowRangeMediaColour { get; }

        System.Windows.Media.Color NominalMediaColour { get; }

    }
}
