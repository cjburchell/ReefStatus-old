
namespace RedPoint.ReefStatus.Common.UI.Controls.Helpers
{
    using System.Windows;

    /// <summary>
    /// Defines the DynamicReferenceHelper type.
    /// </summary>
    public class DynamicReferenceHelper : DependencyObject
    {
        /// <summary>
        /// HasMaximize Property
        /// </summary>
        public static readonly DependencyProperty ReferenceProperty = DependencyProperty.Register(
            "Reference", typeof(object), typeof(DynamicReferenceHelper));

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>The reference.</value>
        public object Reference
        {
            get { return GetValue(ReferenceProperty); }
            set { SetValue(ReferenceProperty, value); }
        }
    }
}