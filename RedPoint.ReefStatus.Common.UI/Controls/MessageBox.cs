namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The Message Box
    /// </summary>
    [TemplatePart(Name = "PART_OK", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Yes", Type = typeof(Button))]
    [TemplatePart(Name = "PART_No", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Cancel", Type = typeof(Button))]
    public class MessageBox : CustomWindow
    {
        /// <summary>
        /// Image Property
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(MessageBoxImage), typeof(MessageBox), new FrameworkPropertyMetadata(MessageBoxImage.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Buttons Property
        /// </summary>
        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register("Buttons", typeof(MessageBoxButton), typeof(MessageBox), new FrameworkPropertyMetadata(MessageBoxButton.OK, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Message Property
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MessageBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public MessageBoxImage Image
        {
            get { return (MessageBoxImage)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the buttons.
        /// </summary>
        /// <value>The buttons.</value>
        public MessageBoxButton Buttons
        {
            get { return (MessageBoxButton)GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        protected MessageBoxResult Result { get; set; }

        /// <summary>
        /// Shows the specified message box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The result from showing the dialog box</returns>
        public static MessageBoxResult Show(string message)
        {
            return Show(null, message, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None);
        }

        /// <summary>
        /// Shows the specified message box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <returns>The result from showing the dialog box</returns>
        public static MessageBoxResult Show(string message, string caption)
        {
            return Show(null, message, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None);
        }

        /// <summary>
        /// Shows the specified message box.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="message">The message.</param>
        /// <returns>The result from showing the dialog box</returns>
        public static MessageBoxResult Show(Window window, string message)
        {
            return Show(window, message, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None);
        }

        /// <summary>
        /// Shows the specified message box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <returns>The result from showing the dialog box</returns>
        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button)
        {
            return Show(null, message, caption, button, MessageBoxImage.None, MessageBoxResult.None);
        }

        /// <summary>
        /// Shows the specified message box.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <returns>The result from showing the dialog box</returns>
        public static MessageBoxResult Show(Window window, string message, string caption)
        {
            return Show(window, message, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None);
        }

        /// <summary>
        /// Shows the specified message box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="image">The image.</param>
        /// <returns>The result from showing the dialog box</returns>
        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            return Show(null, message, caption, button, image, MessageBoxResult.None);
        }

        /// <summary>
        /// Shows the specified message box.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <returns>The result from showing the dialog box</returns>
        public static MessageBoxResult Show(Window window, string message, string caption, MessageBoxButton button)
        {
            return Show(window, message, caption, button, MessageBoxImage.None, MessageBoxResult.None);
        }

        /// <summary>
        /// Shows the specified message box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="image">The image.</param>
        /// <param name="result">The result.</param>
        /// <returns>The result from showing the dialog box</returns>
        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage image, MessageBoxResult result)
        {
            return Show(null, message, caption, button, image, result);
        }

        /// <summary>
        /// Shows the specified message box.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="image">The image.</param>
        /// <param name="result">The result.</param>
        /// <returns>The result from showing the dialog box</returns>
        public static MessageBoxResult Show(Window window, string message, string caption, MessageBoxButton button, MessageBoxImage image, MessageBoxResult result)
        {
            MessageBox messageBox = new MessageBox
                                        {
                                            Style = (Style) Application.Current.Resources["DefaultMessageBoxStyle"],
                                            Title = caption,
                                            Message = message,
                                            Buttons = button,
                                            Image = image,
                                            Owner = window,
                                            Result = result,
                                            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
                                        };

            switch (image)
            {
                case MessageBoxImage.Asterisk:
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
                case MessageBoxImage.Hand:
                    System.Media.SystemSounds.Hand.Play();
                    break;
                case MessageBoxImage.Exclamation:
                    System.Media.SystemSounds.Exclamation.Play();
                    break;
                case MessageBoxImage.None:
                    System.Media.SystemSounds.Beep.Play();
                    break;
                case MessageBoxImage.Question:
                    System.Media.SystemSounds.Question.Play();
                    break;
            }

            messageBox.ShowDialog();
            return messageBox.Result;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button okButton = GetTemplateChild("PART_OK") as Button;
            if (okButton != null)
            {
                okButton.Click += this.okButton_Click;
                if (this.Buttons == MessageBoxButton.OK)
                {
                    okButton.IsDefault = true;
                    okButton.IsCancel = true;
                }

                if (this.Buttons == MessageBoxButton.OKCancel)
                {
                    okButton.IsDefault = true;
                }
            }

            Button yesButton = GetTemplateChild("PART_Yes") as Button;
            if (yesButton != null)
            {
                yesButton.Click += this.yesButton_Click;
                if (this.Buttons == MessageBoxButton.YesNo || this.Buttons == MessageBoxButton.YesNoCancel)
                {
                    yesButton.IsDefault = true;
                }
            }

            Button noButton = GetTemplateChild("PART_No") as Button;
            if (noButton != null)
            {
                noButton.Click += this.noButton_Click;
                if (this.Buttons == MessageBoxButton.YesNo)
                {
                    noButton.IsCancel = true;
                }
            }

            Button cancelButton = GetTemplateChild("PART_Cancel") as Button;
            if (cancelButton != null)
            {
                cancelButton.Click += this.cancelButton_Click;
                if (this.Buttons == MessageBoxButton.OKCancel || this.Buttons == MessageBoxButton.YesNoCancel)
                {
                    cancelButton.IsCancel = true;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the cancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Result = MessageBoxResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the noButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            this.Result = MessageBoxResult.No;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the yesButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Result = MessageBoxResult.Yes;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the okButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.Result = MessageBoxResult.OK;
            this.Close();
        }
    }
}
