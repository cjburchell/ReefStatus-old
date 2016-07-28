using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedPoint.ReefStatus.Gui.Converters
{
    using System.Windows.Data;
    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// Convrts an int to a bool based on the paramater given
    /// </summary>
    [ValueConversion(typeof(GraphRange), typeof(bool))]
    public class RangeToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

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
            return ((GraphRange)value).ToString() == (string)parameter;
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
            if ((bool)value)
            {
                switch ((string)parameter)
                {
                    case "Day":
                        return GraphRange.Day;
                    case "All":
                        return GraphRange.All;
                    case "Month":
                        return GraphRange.Month;
                    case "Week":
                        return GraphRange.Week;
                    case "Year":
                        return GraphRange.Year;
                }
            }

            return null;
        }

        #endregion
    }
}
