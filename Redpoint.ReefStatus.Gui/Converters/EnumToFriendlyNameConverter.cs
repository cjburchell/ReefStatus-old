// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumToFriendlyNameConverter.cs" company="Redpoint Apps">
//   2010
// </copyright>
// <summary>
//   Defines the EnumToFriendlyNameConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Gui.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using RedPoint.ReefStatus.Common;

    /// <summary>
    /// Converts a enum to a formated name from the resources
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class EnumToFriendlyNameConverter : IValueConverter
    {
        /// <summary>
        /// Convert value for binding from source object
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return Language.GetFrendyName(value);
            }

            return string.Empty;
        }

        /// <summary>
        /// ConvertBack value from binding back to source object
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("Cant convert back");
        }
    }
}
