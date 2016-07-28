// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TitleBar.xaml.cs" company="Redpoint Games">
//   2009
// </copyright>
// <summary>
//   Interaction logic for TitleBar.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Gui.Views.Controls
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for TitleBar.xaml
    /// </summary>
    public partial class TitleBar
    {
        /// <summary>
        /// Text Property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(object), typeof(TitleBar));

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleBar"/> class.
        /// </summary>
        public TitleBar()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text field.</value>
        public object Text
        {
            get { return GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
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
            this.ParentWindow.Close();
        }

        /// <summary>
        /// Handles the Click event of the Maximize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.ParentWindow.WindowState == WindowState.Maximized)
            {
                this.ParentWindow.WindowState = WindowState.Normal;
            }
            else
            {
                this.ParentWindow.WindowState = WindowState.Maximized;
            }
        }

        /// <summary>
        /// Handles the MouseDown event of the Border control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.ParentWindow.DragMove();
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the TitleBarControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TitleBarControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.ParentWindow.WindowState == WindowState.Maximized)
            {
                this.ParentWindow.WindowState = WindowState.Normal;
            }
            else
            {
                this.ParentWindow.WindowState = WindowState.Maximized;
            }
        }
    }
}