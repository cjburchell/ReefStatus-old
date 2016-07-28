namespace RedPoint.ReefStatus.Gui.Converters
{
    using System.Windows.Data;

    using RedPoint.ReefStatus.Common.ProfiLux;

    [ValueConversion(typeof(GraphRange), typeof(int))]
    public class RangeToInt : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)((GraphRange)value);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (GraphRange)((int)value);
        }

        #endregion
    }
}
