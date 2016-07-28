// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorWindow.cs" company="Redpoint Apps">
//   2010
// </copyright>
// <summary>
//   The Error Dialog
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.UI
{
    using System.Windows.Forms;

    /// <summary>
    /// The Error Dialog
    /// </summary>
    public partial class ErrorWindow : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWindow"/> class.
        /// </summary>
        public ErrorWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the discription.
        /// </summary>
        /// <value>The discription.</value>
        public string Discription
        {
            get
            {
                return this.discription.Text;
            }

            set
            {
                this.discription.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public string Details
        {
            get
            {
                return this.details.Text;
            }

            set
            {
                this.details.Text = value;
            }
        }
    }
}
