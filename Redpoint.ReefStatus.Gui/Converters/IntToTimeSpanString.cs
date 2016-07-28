using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedPoint.ReefStatus.Gui.Converters
{
    using System.Globalization;
    using System.Windows.Data;

    using RedPoint.ReefStatus.Common;

    [ValueConversion(typeof(int), typeof(string))]
    public class IntToTimeSpanString : IValueConverter
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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int timespan = (int)value;

            timespan = timespan / 60;
            var hours = timespan % 24;
            var days = timespan / 24;

            if (days == 0)
            {
                return string.Format("{0:D2} {1}", hours, Language.GetResource("strHours"));
            }

            if (days == 1)
            {
                return string.Format("{0} {2}, {1:D2} {3}", days, hours, Language.GetResource("strDay"), Language.GetResource("strHours"));
            }

            return string.Format("{0} {2}, {1:D2} {3}", days, hours, Language.GetResource("strDays"), Language.GetResource("strHours"));
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
