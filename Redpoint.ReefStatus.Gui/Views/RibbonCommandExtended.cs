namespace RedPoint.ReefStatus.Gui.Views
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Windows.Controls.Ribbon;

    public class RibbonCommandExtended : RibbonCommand
    {
        private ICommand command;
        public ICommand DelegatedCommand
        {
            get { return this.command; }
            set
            {
                this.command = value;
                if (this.command != null)
                {
                    this.CanExecute += us_CanExecute;
                    this.Executed += us_Executed;
                }
                else
                {
                    this.CanExecute -= us_CanExecute;
                    this.Executed -= us_Executed;
                }
            }
        }

        private void us_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.DelegatedCommand.Execute(e.Parameter);
        }

        private void us_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.DelegatedCommand.CanExecute(e.Parameter);
        }
    }
}
