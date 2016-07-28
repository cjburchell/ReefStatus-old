// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewTemplateSelector.cs"  company="Redpoint">
//      2010
// </copyright>
// <summary>
//   The view template selector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Gui.Views
{
    using System.Windows;
    using System.Windows.Controls;

    using RedPoint.ReefStatus.Common.ViewModel;
    using RedPoint.ReefStatus.Gui.ViewModels;

    /// <summary>
    /// The view template selector.
    /// </summary>
    public class ViewTemplateSelector : DataTemplateSelector
    {
        #region Properties

        /// <summary>
        /// Gets or sets GraphTemplate.
        /// </summary>
        public DataTemplate GraphTemplate { get; set; }

        /// <summary>
        /// Gets or sets the S port graph template.
        /// </summary>
        /// <value>
        /// The S port graph template.
        /// </value>
        public DataTemplate SPortGraphTemplate { get; set; }

        /// <summary>
        /// Gets or sets the probe graph template.
        /// </summary>
        /// <value>
        /// The probe graph template.
        /// </value>
        public DataTemplate ProbeGraphTemplate { get; set; }


        #endregion

        #region Public Methods

        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate"/> based on custom logic.
        /// </summary>
        /// <param name="item">
        /// The data object for which to select the template.
        /// </param>
        /// <param name="container">
        /// The data-bound object.
        /// </param>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"/> or null. The default value is null.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {

            if (item is SPortGraphViewModel)
            {
                return this.SPortGraphTemplate;
            }

            if (item is ProbeGraphViewModel)
            {
                return this.ProbeGraphTemplate;
            }

            if (item is GraphViewModel)
            {
                return this.GraphTemplate;
            }

            return null;
        }

        #endregion
    }
}