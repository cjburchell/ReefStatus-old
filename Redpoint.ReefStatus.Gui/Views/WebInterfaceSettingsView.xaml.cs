namespace RedPoint.ReefStatus.Gui.Views
{
    using System.Diagnostics;
    using System.Windows.Navigation;

    /// <summary>
    /// Interaction logic for WebInterfaceSettingsView.xaml
    /// </summary>
    public partial class WebInterfaceSettingsView
    {
        public WebInterfaceSettingsView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
