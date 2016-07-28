namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common.UI.ViewModel;

    public class CalendarViewModel : BindableBase
    {

        public string Title
        {
            get
            {
                return "Calendar";
                //return (string)Application.Current.Resources["strRemote"]; 
            }
        }
    }
}
