namespace RedPoint.ReefStatus.Gui.Converters
{
    using System.Windows.Data;
    using Common.ProfiLux;
    using System;

    [ValueConversion(typeof(OperationMode), typeof(bool))]
    class OperationModeToBool : IValueConverter
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
            return ((OperationMode)value).ToString() == (string)parameter;
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
                    case "CanTransparent":
                        return OperationMode.CanTransparent;
                    case "Diagnostics":
                        return OperationMode.Diagnostics;
                    case "LightTest":
                        return OperationMode.LightTest;
                    case "Maintenance":
                        return OperationMode.Maintenance;
                    case "ManualIllumination":
                        return OperationMode.ManualIllumination;
                    case "ManualSockets":
                        return OperationMode.ManualSockets;
                    case "Normal":
                        return OperationMode.Normal;
                }
            }

            return OperationMode.Normal;
        }
        #endregion
    }
}
