namespace RedPoint.ReefStatus.Gui.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    ///   Resource Key To StringConverter
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class ResourceKeyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resourceKey = value as string;
            if (resourceKey == null)
            {
                return null;
            }

            var resource = Application.Current.TryFindResource(resourceKey);
            if (resource is string)
            {
                return resource as string;
            }

            return resourceKey;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
