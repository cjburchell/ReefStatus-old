namespace RedPoint.ReefStatus.Gui.Converters
{
    using System.Windows.Data;

    using RedPoint.ReefStatus.Common.ProfiLux;

    public class IsBaseInfoConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is BaseInfo;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
