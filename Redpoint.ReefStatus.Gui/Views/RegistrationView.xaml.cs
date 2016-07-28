
namespace RedPoint.ReefStatus.Gui.Views
{
    using RedPoint.ReefStatus.Gui.ViewModels;

    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegistrationView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationView"/> class.
        /// </summary>
        public RegistrationView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the Close control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
