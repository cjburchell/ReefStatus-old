// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reminder.cs" company="RedpointGames">
//   2010
// </copyright>
// <summary>
//   The Controllers Reminders
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;

    using Microsoft.Practices.Prism.Mvvm;
    using System.Windows.Input;

    using Microsoft.Practices.Prism.Commands;

    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.UI.ViewModel;
    

    /// <summary>
    /// The Controllers Reminders
    /// </summary>
    public class Reminder : BindableBase
    {
        /// <summary>
        /// The index.
        /// </summary>
        private int index;

        /// <summary>
        /// The is repeating.
        /// </summary>
        private bool isRepeating;

        /// <summary>
        /// The next.
        /// </summary>
        private DateTime next;

        /// <summary>
        /// The period.
        /// </summary>
        private int period;

        /// <summary>
        /// The sent mail.
        /// </summary>
        private bool sentMail;

        /// <summary>
        /// The text.
        /// </summary>
        private string text;

        /// <summary>
        /// if the remider is overdew
        /// </summary>
        private bool isOverdue;

        private ICommand resetCommand;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is overdue.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is overdue; otherwise, <c>false</c>.
        /// </value>
        public bool IsOverdue
        {
            get
            {
                return this.isOverdue;
            }

            set
            {
                if (this.isOverdue != value)
                {
                    this.isOverdue = value;
                    this.OnPropertyChanged(() => this.IsOverdue);
                }
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type
        {
            get
            {
                return "strReminders";
            }
        }

        /// <summary>
        /// Gets the reset command.
        /// </summary>
        /// <value>The reset command.</value>
        [System.Xml.Serialization.XmlIgnore]
        public ICommand ResetCommand
        {
            get
            {
                return this.resetCommand ?? (this.resetCommand = new DelegateCommand(this.Reset));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [sent mail].
        /// </summary>
        /// <value><c>true</c> if [sent mail]; otherwise, <c>false</c>.</value>
        public bool SentMail
        {
            get
            {
                return this.sentMail;
            }

            set
            {
                if (this.sentMail != value)
                {
                    this.sentMail = value;
                    this.OnPropertyChanged(() => this.SentMail);
                }
            }
        }

        /// <summary>
        /// Gets or sets the next.
        /// </summary>
        /// <value>The next.</value>
        public DateTime Next
        {
            get
            {
                return this.next;
            }

            set
            {
                if (this.next != value)
                {
                    this.next = value;
                    this.OnPropertyChanged(() => this.Next);
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
        /// Gets or sets Index.
        /// </summary>
        public int Index
        {
            get
            {
                return this.index;
            }

            set
            {
                if (this.index != value)
                {
                    this.index = value;
                    this.OnPropertyChanged(() => this.Index);
                }
            }
        }

        /// <summary>
        /// Gets or sets the period.
        /// </summary>
        /// <value>The period.</value>
        public int Period
        {
            get
            {
                return this.period;
            }

            set
            {
                if (this.period != value)
                {
                    this.period = value;
                    this.OnPropertyChanged(() => this.Period);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is repeating.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is repeating; otherwise, <c>false</c>.
        /// </value>
        public bool IsRepeating
        {
            get
            {
                return this.isRepeating;
            }

            set
            {
                if (this.isRepeating != value)
                {
                    this.isRepeating = value;
                    this.OnPropertyChanged(() => this.IsRepeating);
                }
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            ReefStatusSettings.Instance.GetController(this).Commands.SendResetReminder(this);
        }
    }
}
