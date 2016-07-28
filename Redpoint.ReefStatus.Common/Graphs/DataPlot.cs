// <copyright file="DataPlot.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Graphs
{
    using System;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using ZedGraph;

    public class DataPlot : ZedGraphControl
    {
        public void Setup(BaseInfo settings, bool refresh)
        {
            Settings = settings;

            GraphPane.Legend.IsVisible = false;

            GraphPane.Title.Text = settings.DisplayName;
            GraphPane.YAxis.Title.Text = settings.Units;
            GraphPane.YAxis.Scale.MaxGrace = 0.01;
            GraphPane.YAxis.Scale.MinGrace = 0.01;

            GraphPane.XAxis.Type = AxisType.Date;
            GraphPane.XAxis.Scale.MaxGrace = 0;
            GraphPane.XAxis.Scale.MinGrace = 0;

            GraphPane.AddCurve(settings.DisplayName, new PointPairList(), settings.Colour, SymbolType.None);
            GraphPane.XAxis.Title.Text = Language.GetResource("strDateTime");

            this.UpdateSettings(this.Settings.Range);

            if (refresh)
            {
                AxisChange();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public BaseInfo Settings { get; private set; }

        /// <summary>
        /// Creates the graph image.
        /// </summary>
        /// <param name="imageSize">Size of the image.</param>
        /// <param name="datapoints">The datapoints.</param>
        /// <param name="graphSetting">The graph setting.</param>
        /// <param name="range">The range.</param>
        /// <returns>The Image of the graph</returns>
        public static Image CreateImage(Size imageSize, Collection<Common.ProfiLux.DataPoint> datapoints, BaseInfo graphSetting, GraphRange range)
        {
            DataPlot control = new DataPlot();
            control.Setup(graphSetting, false);
            control.Size = imageSize;
            control.AddPoints(datapoints, false);
            control.UpdateSettings(range);
            control.AxisChange();
            control.Invalidate();

            Image image = control.GetImage();
            return image;
        }

        /// <summary>
        /// Clears the data.
        /// </summary>
        public void ClearData()
        {
            GraphPane.CurveList[0].Clear();
        }

        /// <summary>
        /// Adds the point.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="value">The value.</param>
        public void AddPoint(DateTime date, double value)
        {
            GraphPane.CurveList[0].AddPoint(new PointPair(new XDate(date), value));

            if (Settings.Range != GraphRange.All)
            {
                DateTime endTimeRange = DateTime.Now;
                switch (Settings.Range)
                {
                    case GraphRange.Day:
                        endTimeRange = DateTime.Now.AddDays(-1);
                        break;
                    case GraphRange.Week:
                        endTimeRange = DateTime.Now.AddDays(-7);
                        break;
                    case GraphRange.Month:
                        endTimeRange = DateTime.Now.AddMonths(-1);
                        break;
                    case GraphRange.Year:
                        endTimeRange = DateTime.Now.AddYears(-1);
                        break;
                }

                XDate lastpointDate = new XDate(GraphPane.CurveList[0].Points[0].X);

                if (lastpointDate.DateTime < endTimeRange)
                {
                    GraphPane.CurveList[0].RemovePoint(0);
                }
            }

            AxisChange();
            Invalidate();
        }

        /// <summary>
        /// Adds the points to the graph.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="refresh">if set to <c>true</c> [refresh].</param>
        public void AddPoints(Collection<Common.ProfiLux.DataPoint> points, bool refresh)
        {
            ClearData();

            foreach (Common.ProfiLux.DataPoint point in points)
            {
                GraphPane.CurveList[0].AddPoint(new PointPair(new XDate(point.Time), point.Value));
            }

            if (refresh)
            {
                UpdateSettings(Settings.Range);
                AxisChange();
                Invalidate();
            }
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        private void UpdateSettings(GraphRange range)
        {
            this.Text = this.Settings.DisplayName;
            this.GraphPane.Title.Text = this.Settings.DisplayName;
            this.GraphPane.YAxis.Title.Text = this.Settings.Units;
            this.GraphPane.CurveList[0].Color = this.Settings.Colour;

            this.GraphPane.XAxis.MajorGrid.IsVisible = true;
            this.GraphPane.YAxis.MajorGrid.IsVisible = true;

            LineItem curve = (LineItem)this.GraphPane.CurveList[0];
            curve.Line.Fill = new Fill(Color.FromArgb(200, this.Settings.Colour), Color.FromArgb(0, Color.White), 90F);
            curve.Symbol.IsVisible = false;

            if (range == GraphRange.Day || range == GraphRange.Week)
            {
                GraphPane.XAxis.Scale.MinorStep = 1.0;
                GraphPane.XAxis.Scale.MinorUnit = DateUnit.Hour;
                GraphPane.XAxis.Scale.MajorStep = 1.0;
                GraphPane.XAxis.Scale.MajorUnit = DateUnit.Day;
            }
            else if (range == GraphRange.Month)
            {
                GraphPane.XAxis.Scale.MinorStep = 1.0;
                GraphPane.XAxis.Scale.MinorUnit = DateUnit.Day;
                GraphPane.XAxis.Scale.MajorStep = 1.0;
                GraphPane.XAxis.Scale.MajorUnit = DateUnit.Day;
            }
            else
            {
                GraphPane.XAxis.Scale.MinorStep = 1.0;
                GraphPane.XAxis.Scale.MinorUnit = DateUnit.Day;
                GraphPane.XAxis.Scale.MajorStep = 1.0;
                GraphPane.XAxis.Scale.MajorUnit = DateUnit.Month;
            }
        }
    
    }
}