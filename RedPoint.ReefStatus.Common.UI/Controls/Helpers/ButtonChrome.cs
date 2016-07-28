

namespace RedPoint.ReefStatus.Common.UI.Controls.Helpers
{
    using System.Windows;

    /// <summary>
    /// Custom button
    /// </summary>
    public class ButtonChrome : DependencyObject
    {
        /// <summary>
        /// Corner Radius Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ButtonChrome), new FrameworkPropertyMetadata(new CornerRadius(2), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Sets the corner radius.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetCornerRadius(UIElement element, CornerRadius value)
        {
            element.SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// Gets the corner radius.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The corner radius</returns>
        public static CornerRadius GetCornerRadius(UIElement element)
        {
            return (CornerRadius)element.GetValue(CornerRadiusProperty);
        }
    }
}
