// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomGraphSettings.cs" company="Redpoint">
//   2009
// </copyright>
// <summary>
//   Custom Graph Settings
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Settings
{
    using System.Collections.ObjectModel;

    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    /// <summary>
    /// Custom Graph Settings
    /// </summary>
    public class CustomGraphSettings : BindableBase
    {
        /// <summary>
        /// Graph range
        /// </summary>
        private GraphRange customGraphRange;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomGraphSettings"/> class.
        /// </summary>
        public CustomGraphSettings()
        {
            this.ShowCurves = new SafeObservableCollection<string>();
            this.Range = GraphRange.Week;
        }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        public GraphRange Range
        {
            get
            {
                return this.customGraphRange;
            }

            set
            {
                this.customGraphRange = value;
                this.OnPropertyChanged(() => this.Range);
            }
        }

        /// <summary>
        /// Gets or sets the show curves.
        /// </summary>
        /// <value>The show curves.</value>
        public SafeObservableCollection<string> ShowCurves { get; set; }
    }
}
