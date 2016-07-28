namespace RedPoint.ReefStatus.Gui.Views
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Research.DynamicDataDisplay;
    using Microsoft.Research.DynamicDataDisplay.Charts;
    using Microsoft.Research.DynamicDataDisplay.DataSources;
    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// Chart plotter
    /// </summary>
    public class ChartPlotter : Microsoft.Research.DynamicDataDisplay.ChartPlotter
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source",
            typeof(ChartGraphItemCollection),
            typeof(ChartPlotter),
            new FrameworkPropertyMetadata(
                new ChartGraphItemCollection(),
                FrameworkPropertyMetadataOptions.AffectsRender, SourceChanged));

        private static bool addedOwner;

        public ChartPlotter()
        {
            
            try
            {
                if (!addedOwner)
                {
                    DataContextProperty.AddOwner(typeof(ChartGraphItem));
                    DataContextProperty.OverrideMetadata(
                        typeof(ChartPlotter),
                        new FrameworkPropertyMetadata(
                            null, FrameworkPropertyMetadataOptions.Inherits, OnDataContextChanged));
                    addedOwner = true;
                }

            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        private static void SourceChanged(DependencyObject source, DependencyPropertyChangedEventArgs eventArgs)
        {
            var chartPlotter = source as ChartPlotter;
            if (chartPlotter != null)
            {
                var collection = (ChartGraphItemCollection)eventArgs.NewValue;

                foreach (var chart in collection)
                {
                    chart.SetValue(DataContextProperty, chartPlotter.DataContext);
                    chart.ChartPlotter = chartPlotter;
                }
            }
        }

        public static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartPlotter = d as ChartPlotter;
            if (chartPlotter != null)
            {
                foreach (var chart in chartPlotter.Source)
                {
                    chart.SetValue(DataContextProperty, e.NewValue);
                }
            }
        }

        public ChartGraphItemCollection Source
        {
            get
            {
                return (ChartGraphItemCollection)GetValue(SourceProperty);
            }

            set
            {
                SetValue(SourceProperty, value);
            }
        }
    }

    public class ChartGraphItemCollection : ObservableCollection<ChartGraphItem>
    {
    }


    public class ChartGraphItem : FrameworkElement
    {
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(ObservableDataSource<DataPoint>), typeof(ChartGraphItem), new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangePoints)));
        public static readonly DependencyProperty LineColourProperty = DependencyProperty.Register("LineColour", typeof(Brush), typeof(ChartGraphItem), new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangedColor)));

        /// <summary>
        /// Gets or sets the line graphs.
        /// </summary>
        /// <value>The line graphs.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "ok")]
        public ObservableDataSource<DataPoint> Points
        {
            get
            {
                return (ObservableDataSource<DataPoint>)GetValue(PointsProperty);
            }

            set
            {
                SetValue(PointsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the line colour.
        /// </summary>
        /// <value>The line colour.</value>
        public Brush LineColour
        {
            get
            {
                return (Brush)GetValue(LineColourProperty);
            }

            set
            {
                SetValue(LineColourProperty, value);
            }
        }

        public ChartPlotter ChartPlotter { get; set; }

        private LineGraph lineGraph;

        private static void ChangedColor(DependencyObject source, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (source is ChartGraphItem)
            {
                (source as ChartGraphItem).UpdateLineColour((Brush)eventArgs.NewValue);
            }
        }


        /// <summary>
        /// Changes the points.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="eventArgs">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void ChangePoints(DependencyObject source, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (source is ChartGraphItem)
            {
                (source as ChartGraphItem).UpdatePoints((ObservableDataSource<DataPoint>)eventArgs.NewValue);
            }
        }

        /// <summary>
        /// Updates the line colour.
        /// </summary>
        /// <param name="color">The color.</param>
        private void UpdateLineColour(Brush color)
        {
            if (this.lineGraph != null)
            {
                this.lineGraph.LinePen = new Pen(color, 1);
            }
        }

        private HorizontalDateTimeAxis axis;

        /// <summary>
        /// Updates the points.
        /// </summary>
        /// <param name="points">The points.</param>
        private void UpdatePoints(ObservableDataSource<DataPoint> points)
        {
            if (this.lineGraph != null)
            {
                ChartPlotter.Children.Remove(this.lineGraph);
            }

            if (axis == null)
            {
                axis = new HorizontalDateTimeAxis();
            }

            if (points != null)
            {
                points.SetXMapping(p => this.axis.ConvertToDouble(p.Time));
                points.SetYMapping(p => p.Value);
                SolidColorBrush brush = this.LineColour as SolidColorBrush;
                this.lineGraph = brush != null ? ChartPlotter.AddLineGraph(points, brush.Color) : ChartPlotter.AddLineGraph(points);
            }

            ChartPlotter.Legend.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
