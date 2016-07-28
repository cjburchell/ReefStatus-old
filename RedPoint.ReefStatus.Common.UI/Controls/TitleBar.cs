namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Titlebar Control
    /// </summary>
    [TemplatePart(Name = "PART_Minimize", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Maximize", Type = typeof(Button))]
    public class Titlebar : UserControl
    {
        /// <summary>
        /// Text Property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(object), typeof(Titlebar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Icon Property
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(Titlebar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// HasMaximize Property
        /// </summary>
        public static readonly DependencyProperty HasMaximizeProperty =
            DependencyProperty.Register("HasMaximize", typeof(bool), typeof(Titlebar), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// HasMinimize Property
        /// </summary>
        public static readonly DependencyProperty HasMinimizeProperty =
            DependencyProperty.Register("HasMinimize", typeof(bool), typeof(Titlebar), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// HasClose Property
        /// </summary>
        public static readonly DependencyProperty HasCloseProperty =
            DependencyProperty.Register("HasClose", typeof(bool), typeof(Titlebar), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// CloseCommand Property
        /// </summary>
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(Titlebar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// IsMovable Property
        /// </summary>
        public static readonly DependencyProperty IsMovableProperty =
            DependencyProperty.Register("IsMovable", typeof(bool), typeof(Titlebar), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text field.</value>
        public object Text
        {
            get { return this.GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is movable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is movable; otherwise, <c>false</c>.
        /// </value>
        public bool IsMovable
        {
            get { return (bool)GetValue(IsMovableProperty); }
            set { SetValue(IsMovableProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text field.</value>
        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon to set.</value>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has maximize.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has maximize; otherwise, <c>false</c>.
        /// </value>
        public bool HasMaximize
        {
            get { return (bool)GetValue(HasMaximizeProperty); }
            set { SetValue(HasMaximizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has minimize.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has minimize; otherwise, <c>false</c>.
        /// </value>
        public bool HasMinimize
        {
            get { return (bool)GetValue(HasMinimizeProperty); }
            set { SetValue(HasMinimizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has close.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has close; otherwise, <c>false</c>.
        /// </value>
        public bool HasClose
        {
            get { return (bool)GetValue(HasCloseProperty); }
            set { SetValue(HasCloseProperty, value); }
        }

        /// <summary>
        /// Gets the parent window.
        /// </summary>
        /// <value>The parent window.</value>
        private Window ParentWindow
        {
            get { return Window.GetWindow(this); }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.MouseDoubleClick += this.TitlebarControl_MouseDoubleClick;
            this.MouseDown += this.Border_MouseDown;

            Button closeButton = GetTemplateChild("PART_Close") as Button;
            if (closeButton != null)
            {
                closeButton.Click += this.Close_Click;
            }

            Button minimizeButton = GetTemplateChild("PART_Minimize") as Button;
            if (minimizeButton != null)
            {
                minimizeButton.Click += this.Minimize_Click;
            }

            Button maximizeButton = GetTemplateChild("PART_Maximize") as Button;
            if (maximizeButton != null)
            {
                maximizeButton.Click += this.Maximize_Click;
            }
        }

        /// <summary>
        /// Handles the Click event of the Minimize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.ParentWindow.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Handles the Click event of the Close control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (this.CloseCommand == null)
            {
                this.ParentWindow.Close();
            }
        }

        /// <summary>
        /// Handles the Click event of the Maximize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            this.Maiximize();
        }

        /// <summary>
        /// Maximizes this instance.
        /// </summary>
        private void Maiximize()
        {
            if (this.HasMaximize)
            {
                this.ParentWindow.WindowState = this.ParentWindow.WindowState == WindowState.Maximized
                                                    ? WindowState.Normal
                                                    : WindowState.Maximized;
            }
        }

        /// <summary>
        /// Handles the MouseDown event of the Border control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (this.IsMovable)
                {
                    this.ParentWindow.DragMove();
                }
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the TitlebarControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TitlebarControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Maiximize();
        }
    }
}
