namespace RedPoint.ReefStatus.Common.ViewModel
{
    using System;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Input;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common.Commands;
    using RedPoint.ReefStatus.Common.Core;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    public class RemoteViewModel : BindableBase
    {
        private readonly System.Windows.Forms.Timer timerDisplayText = new System.Windows.Forms.Timer();

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteViewModel"/> class.
        /// </summary>
        public RemoteViewModel(CommandThread commands, Controller controller, DataService dataService)
        {
            this.Commands = commands;
            this.Controller = controller;
            this.UpCommand = new DelegateCommand(this.Up);
            this.DownCommand = new DelegateCommand(this.Down);
            this.LeftCommand = new DelegateCommand(this.Left);
            this.RightCommand = new DelegateCommand(this.Right);
            this.EnterCommand = new DelegateCommand(this.Enter);
            this.EscCommand = new DelegateCommand(this.Esc);
            this.dataService = dataService;

            dataService.OnUpdateDisplayText += this.UpdateDisplayText;
            this.timerDisplayText.Interval = 1000;
            this.timerDisplayText.Tick += this.TimeDrisplayText_Tick;
        }

        public ICommand UpCommand { get; private set; }
        public ICommand DownCommand { get; private set; }
        public ICommand LeftCommand { get; private set; }
        public ICommand RightCommand { get; private set; }
        public ICommand EnterCommand { get; private set; }
        public ICommand EscCommand { get; private set; }

        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <value>
        /// The commands.
        /// </value>
        public CommandThread Commands { get; private set; }

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <value>
        /// The controller.
        /// </value>
        public Controller Controller { get; private set; }

        /// <summary>
        /// Ups this instance.
        /// </summary>
        private void Up()
        {
            this.Commands.SendKeyCommand(FaceplateKey.Up);
            this.Commands.UpdateDisplayText(false);
        }

        /// <summary>
        /// Lefts this instance.
        /// </summary>
        private void Left()
        {
            this.Commands.SendKeyCommand(FaceplateKey.Left);
            this.Commands.UpdateDisplayText(false);
        }

        /// <summary>
        /// Enters this instance.
        /// </summary>
        private void Enter()
        {
            this.Commands.SendKeyCommand(FaceplateKey.Enter);
            this.Commands.UpdateDisplayText(false);
        }

        /// <summary>
        /// Rights this instance.
        /// </summary>
        private void Right()
        {
            this.Commands.SendKeyCommand(FaceplateKey.Right);
            this.Commands.UpdateDisplayText(false);
        }

        /// <summary>
        /// Escs this instance.
        /// </summary>
        private void Esc()
        {
            this.Commands.SendKeyCommand(FaceplateKey.Esc);
            this.Commands.UpdateDisplayText(false);
        }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        private void Down()
        {
            this.Commands.SendKeyCommand(FaceplateKey.Down);
            this.Commands.UpdateDisplayText(false);
        }

        /// <summary>
        /// Updates the display text.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="view">The view.</param>
        /// <param name="statusDisplay">The status display.</param>
        /// <param name="controller">The controller.</param>
        public void UpdateDisplayText(string status, string view, Image statusDisplay, Controller controller)
        {
            if (controller == this.Controller)
            {
                if (this.Controller.Info.IsP3)
                {
                    this.DisplayImage = statusDisplay;
                }
                else
                {
                    this.DisplayText = status;
                }

                this.ViewText = view;
            }
        }

        private string dispalyText;

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>The display text.</value>
        public string DisplayText
        {
            get
            {
                return dispalyText;
            }

            set
            {
                if (dispalyText != value)
                {
                    dispalyText = value;
                    this.OnPropertyChanged(() => this.DisplayText);
                }
            }
        }

        private string viewText;

        /// <summary>
        /// Gets or sets the view text.
        /// </summary>
        /// <value>The view text.</value>
        public string ViewText
        {
            get
            {
                return this.viewText;
            }

            set
            {
                if (this.viewText != value)
                {
                    this.viewText = value;
                    this.OnPropertyChanged(() => this.ViewText);
                }
            }
        }

        /// <summary>
        /// Handles the Tick event of the timeDrisplayText control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TimeDrisplayText_Tick(object sender, EventArgs e)
        {
            if (this.Commands != null && this.IsActive)
            {
                this.Commands.UpdateDisplayText(false);
            }
        }


        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return (string)Application.Current.Resources["strRemote"]; }
        }

        public void Stop()
        {
            this.timerDisplayText.Stop();
            this.timerDisplayText.Dispose();
        }

        private Image displayImage;

        private bool isActive;

        private DataService dataService;

        /// <summary>
        /// Gets or sets the display image.
        /// </summary>
        /// <value>The display image.</value>
        public Image DisplayImage
        {
            get
            {
                return this.displayImage;
            }
            set
            {
                if (this.displayImage != value)
                {
                    this.displayImage = value;
                    this.OnPropertyChanged(() => this.DisplayImage);
                }
            }
        }

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                if (this.isActive != value)
                {
                    this.isActive = value;
                    this.OnPropertyChanged(() => this.IsActive);

                    if (value)
                    {
                        this.timerDisplayText.Start();
                    }
                    else
                    {
                        this.timerDisplayText.Stop();
                    }
                }
            }
        }
    }
}