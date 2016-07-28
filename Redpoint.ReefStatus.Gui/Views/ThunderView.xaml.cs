using RedPoint.ReefStatus.Gui.ViewModels;

namespace RedPoint.ReefStatus.Gui.Views
{
    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// Interaction logic for ThunderView.xaml
    /// </summary>
    public partial class ThunderView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThunderView"/> class.
        /// </summary>
        /// <param name="controller">The Controller.</param>
        public ThunderView(Controller controller)
        {
            DataContext = new ThunderViewModel(controller);
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the Ok control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Ok_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}
