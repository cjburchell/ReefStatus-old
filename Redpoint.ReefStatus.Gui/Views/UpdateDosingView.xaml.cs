namespace RedPoint.ReefStatus.Gui.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for UpdateDosingView.xaml
    /// </summary>
    public partial class UpdateDosingView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDosingView"/> class.
        /// </summary>
        public UpdateDosingView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
