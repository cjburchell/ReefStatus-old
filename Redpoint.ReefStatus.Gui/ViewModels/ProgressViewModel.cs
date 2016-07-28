namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using System.Windows.Input;
    using Common.UI.ViewModel;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common.UI;

    public class ProgressViewModel : BindableBase, IProgressCallback
    {
        /// <summary>
        /// cancel Command
        /// </summary>
        private ICommand cancelCommand;

        private string title;
        private string text;

        private double progressValue;
        private bool hasCancel;

        private int max;
        private int value;

        private int min;

        private bool display;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressViewModel"/> class.
        /// </summary>
        public ProgressViewModel()
        {
            this.Display = false;
            this.HasCancel = true;
            this.Text = string.Empty;
            this.Title = string.Empty;
            this.min = 0;
            this.max = 100;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get
            {
                return this.title;
            }

            set 
            { 
                if (this.title != value)
                {
                    this.title = value;
                    this.OnPropertyChanged(() => this.Title);
                }
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.OnPropertyChanged(() => this.Text);
                }
            }
        }

        /// <summary>
        /// Gets or sets the progress value.
        /// </summary>
        /// <value>The progress value.</value>
        public double ProgressValue
        {
            get
            {
                return this.progressValue;
            }

            set
            {
                if (this.progressValue != value)
                {
                    this.progressValue = value;
                    this.OnPropertyChanged(() => this.ProgressValue);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has cancel.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has cancel; otherwise, <c>false</c>.
        /// </value>
        public bool HasCancel
        {
            get
            {
                return this.hasCancel;
            }

            set
            {
                if (this.hasCancel != value)
                {
                    this.hasCancel = value;
                    this.OnPropertyChanged(() => this.HasCancel);
                }
            }
        }

        #region IProgressCallback Members

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        public ICommand CancelCommand
        {
            get
            {
                if (this.cancelCommand == null)
                {
                    this.cancelCommand = new DelegateCommand(this.Cancel);    
                }

                return this.cancelCommand;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ProgressViewModel"/> is display.
        /// </summary>
        /// <value><c>true</c> if display; otherwise, <c>false</c>.</value>
        public bool Display
        {
            get
            {
                return this.display;
            }

            set
            {
                if (this.display != value)
                {
                    this.display = value;
                    this.OnPropertyChanged(() => this.Display);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is aborting.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is aborting; otherwise, <c>false</c>.
        /// </value>
        public bool IsAborting
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag to set.</value>
        public object Tag { get; set; }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Cancel()
        {
            this.IsAborting = true;
        }

        /// <summary>
        /// Call this method from the worker thread to initialize
        /// the progress callback.
        /// </summary>
        /// <param name="minimum">The minimum value in the progress range (e.g. 0)</param>
        /// <param name="maximum">The maximum value in the progress range (e.g. 100)</param>
        /// <param name="title"> title of the dialog</param>
        public void Begin(int minimum, int maximum, string title)
        {
            SetRange(minimum, maximum);
            this.Begin(title);
        }

        /// <summary>
        /// Call this method from the worker thread to initialize
        /// the progress callback, without setting the range
        /// </summary>
        /// <param name="title">title of the dialog</param>
        public void Begin(string title)
        {
            this.Title = title;
            this.Display = true;
        }

        /// <summary>
        /// Call this method from the worker thread to reset the range in the progress callback
        /// </summary>
        /// <param name="minimum">The minimum value in the progress range (e.g. 0)</param>
        /// <param name="maximum">The maximum value in the progress range (e.g. 100)</param>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        public void SetRange(int minimum, int maximum)
        {
            this.min = minimum;
            this.max = maximum;
            this.Recaculate();
        }

        /// <summary>
        /// Call this method from the worker thread to update the progress text.
        /// </summary>
        /// <param name="text">The progress text to display</param>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        public void SetText(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// Call this method from the worker thread to increase the progress counter by a specified value.
        /// </summary>
        /// <param name="value">The amount by which to increment the progress indicator</param>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        public void StepTo(int value)
        {
            this.value = value;
            this.Recaculate();
        }

        /// <summary>
        /// Call this method from the worker thread to step the progress meter to a particular value.
        /// </summary>
        /// <param name="value">The value to which to step the meter</param>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        public void Increment(int value)
        {
            this.value += value;
            this.Recaculate();
        }

        /// <summary>
        /// Call this method from the worker thread to finalize the progress meter
        /// </summary>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        public void End()
        {
            this.Display = false;
            this.HasCancel = true;
            this.Text = string.Empty;
            this.Title = string.Empty;
            this.min = 0;
            this.max = 100;
        }

        /// <summary>
        /// Recaculates this instance.
        /// </summary>
        private void Recaculate()
        {
            int range = this.max - this.min;
            int actval = this.value - this.min;
            this.ProgressValue = (actval / (double)range) * 100.0;
        }

        #endregion

        #region IProgressCallback Members

        /// <summary>
        /// an item for locking
        /// </summary>
        public object lockItem = new object();

        /// <summary>
        /// Gets the lock.
        /// </summary>
        /// <value>The lock.</value>
        public object Lock
        { 
            get
            {
                return this.lockItem;
            }
        }

        #endregion
    }
}
