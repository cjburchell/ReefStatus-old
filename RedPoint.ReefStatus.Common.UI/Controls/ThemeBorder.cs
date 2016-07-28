
namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Crome hover and selected state
    /// </summary>
    public class ThemeBorder : UserControl
    {
        /// <summary>
        /// IsPressed Property
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(ThemeBorder), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Corner Radius Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ThemeBorder), new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Is Checked Property
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ThemeBorder), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// IsHighlighted Property
        /// </summary>
        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(ThemeBorder), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// PressedBorderBrush Property
        /// </summary>
        public static readonly DependencyProperty PressedBorderProperty =
            DependencyProperty.Register("PressedBorder", typeof(Brush), typeof(ThemeBorder), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BackgroundThemeLevelProperty =
           DependencyProperty.Register("BackgroundThemeLevel", typeof(Brush), typeof(ThemeBorder), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BorderThemeLevelProperty =
            DependencyProperty.Register("BorderThemeLevel", typeof(Brush), typeof(ThemeBorder), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty PressedThemeLevelProperty =
          DependencyProperty.Register("PressedThemeLevel", typeof(Brush), typeof(ThemeBorder), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty HoverThemeLevelProperty =
          DependencyProperty.Register("HoverThemeLevel", typeof(Brush), typeof(ThemeBorder), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty PressedBorderThemeLevelProperty =
         DependencyProperty.Register("PressedBorderThemeLevel", typeof(Brush), typeof(ThemeBorder), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty HoverBorderThemeLevelProperty =
          DependencyProperty.Register("HoverBorderThemeLevel", typeof(Brush), typeof(ThemeBorder), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// PressedBorderBrush Property
        /// </summary>
        public static readonly DependencyProperty HoverBorderProperty = DependencyProperty.Register(
            "HoverBorder",
            typeof(Brush),
            typeof(ThemeBorder),
            new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// PressedBorderBrush Property
        /// </summary>
        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(ThemeBorder), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// PressedBorderBrush Property
        /// </summary>
        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(ThemeBorder), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush PressedThemeLevel
        {
            get { return (Brush)GetValue(PressedThemeLevelProperty); }
            set { SetValue(PressedThemeLevelProperty, value); }
        }

        public Brush HoverThemeLevel
        {
            get { return (Brush)GetValue(HoverThemeLevelProperty); }
            set { SetValue(HoverThemeLevelProperty, value); }
        }

        public Brush PressedBorderThemeLevel
        {
            get { return (Brush)GetValue(PressedBorderThemeLevelProperty); }
            set { SetValue(PressedBorderThemeLevelProperty, value); }
        }

        public Brush HoverBorderThemeLevel
        {
            get { return (Brush)GetValue(HoverBorderThemeLevelProperty); }
            set { SetValue(HoverBorderThemeLevelProperty, value); }
        }

        public Brush BorderThemeLevel
        {
            get { return (Brush)GetValue(BorderThemeLevelProperty); }
            set { SetValue(BorderThemeLevelProperty, value); }
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
        public Brush BackgroundThemeLevel
        {
            get { return (Brush)GetValue(BackgroundThemeLevelProperty); }
            set { SetValue(BackgroundThemeLevelProperty, value); }
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
