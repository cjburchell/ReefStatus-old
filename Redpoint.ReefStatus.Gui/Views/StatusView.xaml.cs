namespace RedPoint.ReefStatus.Gui.Views
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// Interaction logic for StatusControl.xaml
    /// </summary>
    public partial class StatusView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusView"/> class.
        /// </summary>
        public StatusView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the UpdateUserItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void UpdateUserItem_Click(object sender, RoutedEventArgs e)
        {
            if (((Controler)DataContext).UpdateCommand.CanExecute(null))
            {
                ((Controler)DataContext).UpdateCommand.Execute(null);
                new UpdateUserValueView { DataContext = this.DataContext }.ShowDialog();
            }
        }

        private void UpdateDosing_Click(object sender, RoutedEventArgs e)
        {
            if (((Controler)DataContext).UpdateDosingCommand.CanExecute(null))
            {
                ((Controler)DataContext).UpdateDosingCommand.Execute(null);
                new UpdateDosingView { DataContext = this.DataContext }.ShowDialog();
            }
        }

        private void ThunderStorm_Click(object sender, RoutedEventArgs e)
        {
            new ThunderView((Controler)DataContext).ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the WebSiteButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void WebSiteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                Process.Start(new ProcessStartInfo((string)button.Tag));
            }
        }
    }
}