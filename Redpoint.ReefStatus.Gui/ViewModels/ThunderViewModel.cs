using RedPoint.ReefStatus.Common.UI.ViewModel;
using System.Windows.Input;

namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common.ProfiLux;

    public class ThunderViewModel : BindableBase
    {
        /// <summary>
        /// The thunder storm command
        /// </summary>
        private ICommand thunderStormCommand;

        /// <summary>
        /// The duration
        /// </summary>
        private int duration = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThunderViewModel"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public ThunderViewModel(Controller controller)
        {
            this.Controller = controller;
        }

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <value>
        /// The controller.
        /// </value>
        public Controller Controller { get; private set; }

        /// <summary>
        /// Gets the thunder storm command.
        /// </summary>
        /// <value>The thunder storm command.</value>
        public ICommand ThunderStormCommand
        {
            get
            {
                if (thunderStormCommand == null)
                {
                    thunderStormCommand = new DelegateCommand(ThunderStorm);
                }
                return thunderStormCommand;
            }
        }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration
        {
            set 
            { 
                duration = value;
                this.OnPropertyChanged(() => this.Duration);
            }
            get { return duration; }
        }

        /// <summary>
        /// Thunders the storm.
        /// </summary>
        private void ThunderStorm()
        {
            this.Controller.Commands.ThunderStorm(Duration);
        }
    }
}
