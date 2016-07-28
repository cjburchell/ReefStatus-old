
namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The bindable data grid.
    /// </summary>
    public class BindableDataGrid : DataGrid
    {
        #region Constants and Fields

        /// <summary>
        /// The grid column width property.
        /// </summary>
        public static readonly DependencyProperty GridColumnWidthProperty =
            DependencyProperty.RegisterAttached(
                "GridColumnWidth", 
                typeof(double), 
                typeof(BindableDataGrid), 
                new FrameworkPropertyMetadata(
                    (double)-1, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.AffectsRender, 
                    ColumnWidthChanged));

        /// <summary>
        /// The changeing width property.
        /// </summary>
        private static readonly DependencyProperty ChangeingWidthProperty =
            DependencyProperty.RegisterAttached(
                "ChangeingWidth", typeof(bool), typeof(BindableDataGrid), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// The init width property.
        /// </summary>
        private static readonly DependencyProperty InitWidthProperty = DependencyProperty.RegisterAttached(
            "InitWidth", typeof(bool), typeof(BindableDataGrid), new FrameworkPropertyMetadata(false));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="BindableDataGrid"/> class.
        /// </summary>
        static BindableDataGrid()
        {
            if (!IsDesignTime())
            {
                try
                {
                    DataContextProperty.AddOwner(typeof(DataGridColumn));
                    DataContextProperty.OverrideMetadata(
                        typeof(DataGrid), 
                        new FrameworkPropertyMetadata(
                            null, FrameworkPropertyMetadataOptions.Inherits, OnDataContextChanged));
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableDataGrid"/> class.
        /// </summary>
        public BindableDataGrid()
        {
            this.Columns.CollectionChanged += this.ColumnsCollectionChanged;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the width of the grid column.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// the grid column width
        /// </returns>
        public static double GetGridColumnWidth(DependencyObject target)
        {
            return (double)target.GetValue(GridColumnWidthProperty);
        }

        /// <summary>
        /// The is design time.
        /// </summary>
        /// <returns>
        /// The is design time.
        /// </returns>
        public static bool IsDesignTime()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }

        /// <summary>
        /// Called when [data context changed].
        /// </summary>
        /// <param name="d">
        /// The Dependency Object.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        public static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as DataGrid;
            if (grid != null)
            {
                foreach (DataGridColumn col in grid.Columns)
                {
                    col.SetValue(DataContextProperty, e.NewValue);

                    if (!GetInitWidth(col))
                    {
                        DependencyPropertyDescriptor dpd =
                            DependencyPropertyDescriptor.FromProperty(
                                DataGridColumn.ActualWidthProperty, typeof(DataGridColumn));
                        if (dpd != null)
                        {
                            dpd.AddValueChanged(col, ActualColumnWidthChanged);
                        }

                        SetInitWidth(col, true);
                    }
                }
            }
        }

        /// <summary>
        /// The set grid column width.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetGridColumnWidth(DependencyObject target, double value)
        {
            target.SetValue(GridColumnWidthProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Actuals the column width changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private static void ActualColumnWidthChanged(object sender, EventArgs e)
        {
            var column = sender as DataGridColumn;
            if (column != null && !GetChangeingWidth(column))
            {
                SetChangeingWidth(column, true);
                SetGridColumnWidth(column, column.ActualWidth);
                SetChangeingWidth(column, false);
            }
        }

        /// <summary>
        /// Columns the width changed.
        /// </summary>
        /// <param name="d">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        private static void ColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridColumn;
            if (column != null && e.NewValue is double && !GetChangeingWidth(column))
            {
                SetChangeingWidth(column, true);
                column.Width = new DataGridLength(
                    (double)e.NewValue, 
                    (double)e.NewValue == -1 ? DataGridLengthUnitType.Auto : DataGridLengthUnitType.Pixel);
                SetChangeingWidth(column, false);
            }
        }

        /// <summary>
        /// The get changeing width.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// if we are changeing the width.
        /// </returns>
        private static bool GetChangeingWidth(DependencyObject target)
        {
            return (bool)target.GetValue(ChangeingWidthProperty);
        }

        /// <summary>
        /// The get init width.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// if we have init the with funtion.
        /// </returns>
        private static bool GetInitWidth(DependencyObject target)
        {
            return (bool)target.GetValue(InitWidthProperty);
        }

        /// <summary>
        /// The set changeing width.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private static void SetChangeingWidth(DependencyObject target, bool value)
        {
            target.SetValue(ChangeingWidthProperty, value);
        }

        /// <summary>
        /// The set init width.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private static void SetInitWidth(DependencyObject target, bool value)
        {
            target.SetValue(InitWidthProperty, value);
        }

        /// <summary>
        /// Columnses the collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void ColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DataGridColumn col in e.NewItems)
                {
                    col.SetValue(DataContextProperty, this.DataContext);

                    if (!GetInitWidth(col))
                    {
                        DependencyPropertyDescriptor dpd =
                            DependencyPropertyDescriptor.FromProperty(
                                DataGridColumn.ActualWidthProperty, typeof(DataGridColumn));
                        if (dpd != null)
                        {
                            dpd.AddValueChanged(col, ActualColumnWidthChanged);
                        }

                        SetInitWidth(col, true);
                    }
                }
            }
        }

        #endregion
    }
}