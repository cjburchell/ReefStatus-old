using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedPoint.ReefStatus.Gui.Converters
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using RedPoint.ReefStatus.Common.ProfiLux;

    [ValueConversion(typeof(BaseInfo), typeof(Visibility))]
    public class IsSPortToVisiblity : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BooleanToVisibilityConverter boolToVis = new BooleanToVisibilityConverter();
            return boolToVis.Convert(value is SPort, targetType, parameter, culture);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}

