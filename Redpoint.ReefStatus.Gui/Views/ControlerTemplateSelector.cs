namespace RedPoint.ReefStatus.Gui.Views
{
    using System.Windows;
    using System.Windows.Controls;

    using RedPoint.ReefStatus.Common.ProfiLux;

    public class ControllerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ControllerTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Controller)
            {
                return this.ControllerTemplate;
            }

            return null;
        }
    }
}
