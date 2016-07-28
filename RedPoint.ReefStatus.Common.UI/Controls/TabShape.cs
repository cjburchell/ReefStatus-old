namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// The tab shape.
    /// </summary>
    public class TabShape : UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The chrome background property.
        /// </summary>
        public static readonly DependencyProperty BackgroundThemeLevelProperty =
            DependencyProperty.Register(
                "BackgroundThemeLevel",
                typeof(Brush),
                typeof(TabShape),
                new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The chrome border property.
        /// </summary>
        public static readonly DependencyProperty BorderThemeLevelProperty = DependencyProperty.Register(
            "BorderThemeLevel",
            typeof(Brush),
            typeof(TabShape),
            new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The chrome bottom property.
        /// </summary>
        public static readonly DependencyProperty BottomThemeLevelProperty = DependencyProperty.Register(
            "BottomThemeLevel",
            typeof(Brush),
            typeof(TabShape),
            new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The chrome top property.
        /// </summary>
        public static readonly DependencyProperty TopThemeLevelProperty = DependencyProperty.Register(
            "TopThemeLevel",
            typeof(Brush),
            typeof(TabShape),
            new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the chrome background.
        /// </summary>
        /// <value>
        /// The chrome background.
        /// </value>
        public Brush BackgroundThemeLevel
        {
            get
            {
                return (Brush)this.GetValue(BackgroundThemeLevelProperty);
            }

            set
            {
                this.SetValue(BackgroundThemeLevelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the chrome border.
        /// </summary>
        /// <value>
        /// The chrome border.
        /// </value>
        public Brush BorderThemeLevel
        {
            get
            {
                return (Brush)this.GetValue(BorderThemeLevelProperty);
            }

            set
            {
                this.SetValue(BorderThemeLevelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the chrome bottom.
        /// </summary>
        /// <value>
        /// The chrome bottom.
        /// </value>
        public Brush BottomThemeLevel
        {
            get
            {
                return (Brush)this.GetValue(BottomThemeLevelProperty);
            }

            set
            {
                this.SetValue(BottomThemeLevelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the chrome top.
        /// </summary>
        /// <value>
        /// The chrome top.
        /// </value>
        public Brush TopThemeLevel
        {
            get
            {
                return (Brush)this.GetValue(TopThemeLevelProperty);
            }

            set
            {
                this.SetValue(TopThemeLevelProperty, value);
            }
        }

        #endregion
    }
}