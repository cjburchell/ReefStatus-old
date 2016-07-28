namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using System;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Input;
    using Common.Core;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    public class RemoteViewModel : ViewModelBase
    {
        private readonly System.Windows.Forms.Timer timerDisplayText = new System.Windows.Forms.Timer();

        public ICommand UpCommand { get; private set; }
        public ICommand DownCommand { get; private set; }
        public ICommand LeftCommand { get; private set; }
        public ICommand RightCommand { get; private set; }
        public ICommand EnterCommand { get; private set; }
        public ICommand EscCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteViewModel"/> class.
        /// </summary>
        public RemoteViewModel()
        {
            this.UpCommand = new DelegateCommand(this.Up, () => true);
            this.DownCommand = new DelegateCommand(this.Down, () => true);
            this.LeftCommand = new DelegateCommand(this.Left, () => true);
            this.RightCommand = new DelegateCommand(this.Right, () => true);
            this.EnterCommand = new DelegateCommand(this.Enter, () => true);
            this.EscCommand = new DelegateCommand(this.Esc, () => true);

            if (ReefStatusSettings.Instance.Controlers.Count != 0)
            {
                Controler = ReefStatusSettings.Instance.Controlers[0];
            }

            DataService.Instance.OnUpdateDisplayText += this.UpdateDisplayText;
            this.timerDisplayText.Interval = 1000;
            this.timerDisplayText.Tick += this.TimeDrisplayText_Tick;
            this.timerDisplayText.Start();
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public ReefStatusSettings Settings
        {
            get
            {
                return ReefStatusSettings.Instance;
            }
        }

        private Controler controler;

        /// <summary>
        /// Gets or sets the controler.
        /// </summary>
        /// <value>The controler.</value>
        public Controler Controler
        {
            get
            {
                return this.controler;
            }

            set
            {
                if (this.controler != value)
                {
                    this.controler = value;
                    this.OnPropertyChanged("Controler");
                }
            }
        }

        /// <summary>
        /// Ups this instance.
        /// </summary>
        private void Up()
        {
            this.Controler.Commands.SendKeyCommand(FaceplateKey.Up);
            this.Controler.Commands.UpdateDisplayText();
        }

        /// <summary>
        /// Lefts this instance.
        /// </summary>
        private void Left()
        {
            this.Controler.Commands.SendKeyCommand(FaceplateKey.Left);
            this.Controler.Commands.UpdateDisplayText();
        }

        /// <summary>
        /// Enters this instance.
        /// </summary>
        private void Enter()
        {
            this.Controler.Commands.SendKeyCommand(FaceplateKey.Enter);
            this.Controler.Commands.UpdateDisplayText();
        }

        /// <summary>
        /// Rights this instance.
        /// </summary>
        private void Right()
        {
            this.Controler.Commands.SendKeyCommand(FaceplateKey.Right);
            this.Controler.Commands.UpdateDisplayText();
        }

        /// <summary>
        /// Escs this instance.
        /// </summary>
        private void Esc()
        {
            this.Controler.Commands.SendKeyCommand(FaceplateKey.Esc);
            this.Controler.Commands.UpdateDisplayText();
        }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        private void Down()
        {
            this.Controler.Commands.SendKeyCommand(FaceplateKey.Down);
            this.Controler.Commands.UpdateDisplayText();
        }

        /// <summary>
        /// Updates the display text.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="view">The view.</param>
        /// <param name="statusDisplay">The status display.</param>
        /// <param name="controler">The controler.</param>
        public void UpdateDisplayText(string status, string view, Image statusDisplay ,Controler controler)
        {
            if (controler == this.Controler)
            {
                if (!controler.Info.IsP3)
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
                    OnPropertyChanged("DisplayText");
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
                    OnPropertyChanged("ViewText");
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
            if (this.Controler != null)
            {
                this.Controler.Commands.UpdateDisplayText();
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

        internal void Stop()
        {
            this.timerDisplayText.Stop();
            this.timerDisplayText.Dispose();
        }

        private Image displayImage;

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
                    OnPropertyChanged("DisplayImage");
                }
            }
        }
    }
}