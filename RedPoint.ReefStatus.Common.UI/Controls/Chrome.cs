
namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Crome hover and selected state
    /// </summary>
    public class Chrome : UserControl
    {
        /// <summary>
        /// IsPressed Property
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(Chrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Corner Radius Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Chrome), new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Is Checked Property
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(Chrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

         /// <summary>
        /// IsHighlighted Property
        /// </summary>
        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(Chrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// PressedBorderBrush Property
        /// </summary>
        public static readonly DependencyProperty PressedBorderProperty =
            DependencyProperty.Register("PressedBorder", typeof(Brush), typeof(Chrome), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ChromeBackgroundProperty =
           DependencyProperty.Register("ChromeBackground", typeof(Brush), typeof(Chrome), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ChromeBorderProperty =
            DependencyProperty.Register("ChromeBorder", typeof(Brush), typeof(Chrome), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));


        public static readonly DependencyProperty PressedChromeProperty =
          DependencyProperty.Register("PressedChrome", typeof(Brush), typeof(Chrome), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));


        public static readonly DependencyProperty HoverChromeProperty =
          DependencyProperty.Register("HoverChrome", typeof(Brush), typeof(Chrome), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty PressedChromeBorderProperty =
         DependencyProperty.Register("PressedChromeBorder", typeof(Brush), typeof(Chrome), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));


        public static readonly DependencyProperty HoverChromeBorderProperty =
          DependencyProperty.Register("HoverChromeBorder", typeof(Brush), typeof(Chrome), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// PressedBorderBrush Property
        /// </summary>
        public static readonly DependencyProperty HoverBorderProperty = DependencyProperty.Register(
            "HoverBorder", 
            typeof(Brush),
            typeof(Chrome),
            new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// PressedBorderBrush Property
        /// </summary>
        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(Chrome), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// PressedBorderBrush Property
        /// </summary>
        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(Chrome), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));


        public Brush PressedChrome
        {
            get { return (Brush)GetValue(PressedChromeProperty); }
            set { SetValue(PressedChromeProperty, value); }
        }

        public Brush HoverChrome
        {
            get { return (Brush)GetValue(HoverChromeProperty); }
            set { SetValue(HoverChromeProperty, value); }
        }

        public Brush PressedChromeBorder
        {
            get { return (Brush)GetValue(PressedChromeBorderProperty); }
            set { SetValue(PressedChromeBorderProperty, value); }
        }

        public Brush HoverChromeBorder
        {
            get { return (Brush)GetValue(HoverChromeBorderProperty); }
            set { SetValue(HoverChromeBorderProperty, value); }
        }

        public Brush ChromeBorder
        {
            get { return (Brush)GetValue(ChromeBorderProperty); }
            set { SetValue(ChromeBorderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the pressed border brush.
        /// </summary>
        /// <value>The pressed border brush.</value>
        public Brush PressedBorder
        {
            get { return (Brush)GetValue(PressedBorderProperty); }
            set { SetValue(PressedBorderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the hover border brush.
        /// </summary>
        /// <value>The hover border brush.</value>
        public Brush HoverBorder
        {
            get { return (Brush)GetValue(HoverBorderProperty); }
            set { SetValue(HoverBorderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the pressed background brush.
        /// </summary>
        /// <value>The pressed background brush.</value>
        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the pressed background brush.
        /// </summary>
        /// <value>The pressed background brush.</value>
        public Brush ChromeBackground
        {
            get { return (Brush)GetValue(ChromeBackgroundProperty); }
            set { SetValue(ChromeBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the hover background.
        /// </summary>
        /// <value>The hover background.</value>
        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is pressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is pressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>The corner radius.</value>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
    }
}
