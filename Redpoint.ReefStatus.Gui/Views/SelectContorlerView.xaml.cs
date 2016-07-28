namespace RedPoint.ReefStatus.Gui.Views
{
    /// <summary>
    /// Interaction logic for SelectContorlerView.xaml
    /// </summary>
    public partial class SelectContorlerView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectContorlerView"/> class.
        /// </summary>
        public SelectContorlerView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the Ok control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Ok_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}