
namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Arrow Direction
    /// </summary>
    public enum ArrowDirection
    {
        /// <summary>
        /// Show up Arrow
        /// </summary>
        Up,

        /// <summary>
        /// Show down Arrow
        /// </summary>
        Down,

        /// <summary>
        /// Show left Arrow
        /// </summary>
        Left,

        /// <summary>
        /// Show Right Arrow
        /// </summary>
        Right,
    }

    /// <summary>
    /// Arrow Control
    /// </summary>
    public class Arrow : UserControl
    {
        /// <summary>
        /// Direction Property
        /// </summary>
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction",
            typeof(ArrowDirection),
            typeof(Arrow),
            new FrameworkPropertyMetadata(ArrowDirection.Up, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BackgroundThemeLevelProperty =
            DependencyProperty.Register("BackgroundThemeLevel", typeof(Brush), typeof(Arrow), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the background theme level.
        /// </summary>
        /// <value>
        /// The background theme level.
        /// </value>
        public Brush BackgroundThemeLevel
        {
            get { return (Brush)GetValue(BackgroundThemeLevelProperty); }
            set { SetValue(BackgroundThemeLevelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public ArrowDirection Direction
        {
            get { return (ArrowDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }
    }
}
