namespace RedPoint.ReefStatus.Gui
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Animation;

    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Gui.ViewModels;
    using RedPoint.ReefStatus.Gui.Views;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private System.Windows.Forms.NotifyIcon trayNotify;


        private bool IsWindows7
        {
            get
            {
                OperatingSystem os = Environment.OSVersion;
                Version vs = os.Version;
                return os.Platform == PlatformID.Win32NT && vs.Major == 6 && vs.Minor != 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            // Animation Optimization
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 30 });

            InitializeComponent();

            this.StateChanged += this.MainWindowStateChanged;

            if (!IsWindows7)
            {
                // Create a object for the context menu
                System.Windows.Forms.ContextMenuStrip trayMenu = new System.Windows.Forms.ContextMenuStrip();

                // Add the Menu Item to the context menu
                System.Windows.Forms.ToolStripMenuItem mnuExit = new System.Windows.Forms.ToolStripMenuItem
                    {
                        Text = "Exit" 
                    };
                mnuExit.Click += this.ExitClick;
                trayMenu.Items.Add(mnuExit);

                // Create an instance of the NotifyIcon Class
                this.trayNotify = new System.Windows.Forms.NotifyIcon
                    {
                        Text = "Reef Status",
                        Icon = RedPoint.ReefStatus.Gui.Properties.Resources.starfish,
                        Visible = true,
                        ContextMenuStrip = trayMenu,
                        BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info,
                        BalloonTipTitle = "Reef Status",
                        BalloonTipText =
                            "Reef Status has been minimized to the system tray. To open the application, double-click the icon in the system tray. To close right click and select exit",
                    };

                this.trayNotify.DoubleClick += this.TrayNotifyDoubleClick;
            }
            else
            {
                this.ShowInTaskbar = true;
            }

            // Add the Context menu to the Notify Icon Object
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = MainWindowViewModel.Instance;

                if (
                    !(ReefStatusSettings.Instance.Window.Size.Width == 0 &&
                      ReefStatusSettings.Instance.Window.Size.Height == 0))
                {
                    this.Width = ReefStatusSettings.Instance.Window.Size.Width;
                    this.Height = ReefStatusSettings.Instance.Window.Size.Height;
                    this.Left = ReefStatusSettings.Instance.Window.Location.X;
                    this.Top = ReefStatusSettings.Instance.Window.Location.Y;
                    WindowState = (WindowState)ReefStatusSettings.Instance.Window.WindowState;
                }
            }
        }

        public static readonly DependencyProperty CaptionButtonsReservedWidthProperty =
    DependencyProperty.Register("CaptionButtonsReservedWidth", typeof(double), typeof(MainWindow), new PropertyMetadata(32.0));


        /// <summary>
        /// Gets the width of the caption buttons reserved.
        /// </summary>
        /// <value>The width of the caption buttons reserved.</value>
        public double CaptionButtonsReservedWidth
        {
            get { return (double)GetValue(CaptionButtonsReservedWidthProperty); }
            set { SetValue(CaptionButtonsReservedWidthProperty, value); }
        }

        private WindowState storedWindowState = WindowState.Normal;

        /// <summary>
        /// Handles the StateChanged event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MainWindowStateChanged(object sender, EventArgs e)
        {
            if (!this.IsWindows7)
            {
                if (WindowState == WindowState.Minimized)
                {
                    this.Visibility = Visibility.Collapsed;
                    if (this.trayNotify != null)
                    {
                        this.trayNotify.ShowBalloonTip(400);
                    }

                    this.WindowState = this.storedWindowState;
                }
                else
                {
                    this.storedWindowState = this.WindowState;
                }
            }
        }

        /// <summary>
        /// Handles the DoubleClick event of the trayNotify control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TrayNotifyDoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = this.storedWindowState;
            this.Activate();
        }

        /// <summary>
        /// Handles the Click event of the Exit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ExitClick(object sender, EventArgs e)
        {
            if (this.trayNotify != null)
            {
                this.trayNotify.Visible = false;
            }
            this.Close();
        }

        /// <summary>
        /// Windows the closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.IsWindows7 || (this.trayNotify != null && !this.trayNotify.Visible))
            {
                ReefStatusSettings.Instance.Window.Size = new System.Drawing.Size((int) ActualWidth,
                                                                                    (int) ActualHeight);
                ReefStatusSettings.Instance.Window.Location = new System.Drawing.Point((int) Left, (int) Top);
                ReefStatusSettings.Instance.Window.WindowState = (System.Windows.Forms.FormWindowState) WindowState;
                ((MainWindowViewModel) DataContext).WindowClosingCommand.Execute(null);

            }
            else
            {
                e.Cancel = true;
                this.Visibility = Visibility.Collapsed;
                if (this.trayNotify != null)
                {
                    this.trayNotify.ShowBalloonTip(400);
                }
            }
        }

        private void AboutClick(object sender, RoutedEventArgs e)
        {
            AboutView box = new AboutView();
            box.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            optionsButton.IsChecked = false;
        }
    }
}
