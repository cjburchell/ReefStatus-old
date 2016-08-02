using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedPoint.ReefStatus.Gui.Views
{
    using System.Windows;
    using System.Windows.Controls;

    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.ProfiLux.Data;

    public class SensorTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ProbeTemplate { get; set; }
        public DataTemplate LevelTemplate { get; set; }
        public DataTemplate DigitalInputTemplate { get; set; }
        public DataTemplate SPortTemplate { get; set; }
        public DataTemplate LPortTemplate { get; set; }
        public DataTemplate LightTemplate { get; set; }
        public DataTemplate DosingPumpTemplate { get; set; }
        public DataTemplate CurrentPumpTemplate { get; set; }
        public DataTemplate UserInfoTemplate { get; set; }
        public DataTemplate ReminderTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate ProgramableLogicTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {

            if (item is Probe)
            {
                return this.ProbeTemplate ?? this.DefaultTemplate;
            }

            if (item is LevelSensor)
            {
                return this.LevelTemplate ?? this.DefaultTemplate;
            }

            if (item is DigitalInput)
            {
                return this.DigitalInputTemplate ?? this.DefaultTemplate;
            }

            if (item is SPort)
            {
                return this.SPortTemplate ?? this.DefaultTemplate;
            }

            if (item is LPort)
            {
                return this.LPortTemplate ?? this.DefaultTemplate;
            }

            if (item is Light)
            {
                return this.LightTemplate ?? this.DefaultTemplate;
            }

            if (item is DosingPump)
            {
                return this.DosingPumpTemplate ?? this.DefaultTemplate;
            }

            if (item is CurrentPump)
            {
                return this.CurrentPumpTemplate ?? this.DefaultTemplate;
            }

            if (item is UserInfo)
            {
                return this.UserInfoTemplate ?? this.DefaultTemplate;
            }

            if (item is Reminder)
            {
                return this.ReminderTemplate ?? this.DefaultTemplate;
            }

            if (item is ProgramableLogic)
            {
                return this.ProgramableLogicTemplate ?? this.ProgramableLogicTemplate;
            }

            return this.DefaultTemplate;
        }
      
    }
}
