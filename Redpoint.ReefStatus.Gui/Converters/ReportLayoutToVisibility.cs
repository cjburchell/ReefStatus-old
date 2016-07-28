namespace RedPoint.ReefStatus.Gui.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using RedPoint.ReefStatus.Common.Settings;

    /// <summary>
    /// Converts ReportLayout To Visibility
    /// </summary>
    [ValueConversion(typeof(ReportLayout), typeof(Visibility))]
    public class ReportLayoutToVisibility : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BooleanToVisibilityConverter boolToVis = new BooleanToVisibilityConverter();
            ReportLayoutToBool layoutToBool = new ReportLayoutToBool();
            return boolToVis.Convert(layoutToBool.Convert(value, targetType, parameter, culture), targetType, parameter, culture);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BooleanToVisibilityConverter boolToVis = new BooleanToVisibilityConverter();
            ReportLayoutToBool layoutToBool = new ReportLayoutToBool();
            return layoutToBool.ConvertBack(boolToVis.ConvertBack(value, targetType, parameter, culture), targetType, parameter, culture);
        }
    }
}
