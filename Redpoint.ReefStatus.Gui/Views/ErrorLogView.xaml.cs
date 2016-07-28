using RedPoint.ReefStatus.Gui.ViewModels;

namespace RedPoint.ReefStatus.Gui.Views
{
    /// <summary>
    /// Interaction logic for ErrorLogView.xaml
    /// </summary>
    public partial class ErrorLogView
    {
        public ErrorLogView()
        {
            InitializeComponent();
            DataContext = ErrorLogViewModel.Instance;
        }
    }
}
