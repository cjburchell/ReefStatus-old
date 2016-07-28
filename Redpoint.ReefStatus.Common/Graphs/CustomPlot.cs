// <copyright file="CustomPlot.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;

    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;

    using ZedGraph;

    /// <summary>
    /// Custom Plot class
    /// </summary>
    public class CustomPlot : ZedGraphControl
    {
        /// <summary>
        /// a list of curves
        /// </summary>
        private readonly Dictionary<string, CurveItem> curves = new Dictionary<string, CurveItem>();

        /// <summary>
        /// Creates the image.
        /// </summary>
        /// <param name="imageSize">Size of the image.</param>
        /// <param name="datapoints">The datapoints.</param>
        /// <param name="Controller">The Controller.</param>
        /// <returns>The created image</returns>
        public static Image CreateImage(Size imageSize, Dictionary<string, Collection<Common.ProfiLux.DataPoint>> datapoints, Controller Controller)
        {
            CustomPlot control = new CustomPlot();
            control.Setup();

            foreach (string graphType in datapoints.Keys)
            {
                string type = graphType;
                BaseInfo graphSetting = Controller.Items.FirstOrDefault(item => item.Id == type);
                if (graphSetting != null)
                {
                    CurveItem curve = control.GraphPane.AddCurve(
                        graphSetting.DisplayName, new PointPairList(), graphSetting.Colour, SymbolType.None);
                    control.UpdateAxis(graphSetting.Units, curve);

                    if (curve != null)
                    {
                        foreach (Common.ProfiLux.DataPoint point in datapoints[graphType])
                        {
                            curve.AddPoint(new PointPair(new XDate(point.Time), point.Value));
                        }
                    }
                }
            }

            control.Size = imageSize;
            control.AxisChange();
            control.Invalidate();

            Image image = control.GetImage();
            return image;
        }

        /// <summary>
        /// Setups the specified settings.
        /// </summary>
        public void Setup()
        {
            this.GraphPane.XAxis.Title.Text = Language.GetResource("strDateTime");
            this.GraphPane.Legend.IsVisible = false;

            this.GraphPane.XAxis.Type = AxisType.Date;
            this.GraphPane.XAxis.Scale.MaxGrace = 0;
            this.GraphPane.XAxis.Scale.MinGrace = 0;

            this.UpdateSettings();

            this.AxisChange();
            this.Invalidate();
        }

        /// <summary>
        /// Adds the point.
        /// </summary>
        /// <param name="graphType">Type of the graph.</param>
        /// <param name="date">The date.</param>
        /// <param name="value">The value.</param>
        /// <param name="Controller">The Controller.</param>
        public void AddPoint(string graphType, DateTime date, double value, Controller Controller)
        {
            CurveItem curve = this.GetCurve(graphType, Controller);
            if (curve != null)
            {
                curve.AddPoint(new PointPair(new XDate(date), value));

                if (ReefStatusSettings.Instance.CustomGraph.Range != GraphRange.All)
                {
                    DateTime endTimeRange = DateTime.Now;
                    switch (ReefStatusSettings.Instance.CustomGraph.Range)
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

                    XDate lastpointDate = new XDate(curve.Points[0].X);

                    if (lastpointDate.DateTime < endTimeRange)
                    {
                        curve.RemovePoint(0);
                    }
                }

                AxisChange();
                Invalidate();
            }
        }

        /// <summary>
        /// Adds the points to the graph.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="refresh">if set to <c>true</c> [refresh].</param>
        /// <param name="Controller">The Controller.</param>
        public void AddPoints(Dictionary<string, Collection<Common.ProfiLux.DataPoint>> points, bool refresh, Controller Controller)
        {
            this.ClearAllData();

            foreach (string graphType in points.Keys)
            {
                CurveItem curve = this.GetCurve(graphType, Controller);
                if (curve != null)
                {
                    foreach (Common.ProfiLux.DataPoint point in points[graphType])
                    {
                        curve.AddPoint(new PointPair(new XDate(point.Time), point.Value));
                    }
                }
            }

            if (refresh)
            {
                this.UpdateSettings();

                AxisChange();
                Invalidate();
            }
        }

        /// <summary>
        /// Adds the points.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="points">The points.</param>
        /// <param name="Controller">The Controller.</param>
        public void AddPoints(string id, Collection<Common.ProfiLux.DataPoint> points, Controller Controller)
        {
            CurveItem curve = this.GetCurve(id, Controller);
            if (curve != null)
            {
                foreach (Common.ProfiLux.DataPoint point in points)
                {
                    curve.AddPoint(new PointPair(new XDate(point.Time), point.Value));
                }

                this.UpdateSettings();

                this.AxisChange();
                this.Invalidate();
            }
        }

        /// <summary>
        /// Clears the data.
        /// </summary>
        private void ClearAllData()
        {
            this.GraphPane.CurveList.Clear();
            this.curves.Clear();
        }

        /// <summary>
        /// Updates the axis.
        /// </summary>
        /// <param name="units">The units.</param>
        /// <param name="curve">The curve.</param>
        private void UpdateAxis(string units, CurveItem curve)
        {
            bool isY2Axis = false;
            Axis axis = null;
            int index = GraphPane.YAxisList.IndexOfTag(units);
            if (index != -1)
            {
                axis = GraphPane.YAxisList[index];
            }
            else
            {
                index = GraphPane.Y2AxisList.IndexOfTag(units);
                if (index != -1)
                {
                    isY2Axis = true;
                    axis = GraphPane.Y2AxisList[index];
                }
            }

            if (axis == null)
            {
                if (GraphPane.YAxisList.Count <= GraphPane.Y2AxisList.Count)
                {
                    axis = new YAxis();
                    isY2Axis = false;
                    index = GraphPane.YAxisList.Count;
                    GraphPane.YAxisList.Add((YAxis)axis);
                }
                else
                {
                    axis = new Y2Axis();
                    isY2Axis = true;
                    index = GraphPane.Y2AxisList.Count;
                    GraphPane.Y2AxisList.Add((Y2Axis)axis);
                }
            }

            axis.IsVisible = true;
            axis.Title.Text = units;
            axis.Tag = units;
            axis.Scale.MaxGrace = 0.01;
            axis.Scale.MinGrace = 0.01;
            axis.MajorTic.IsInside = false;
            axis.MinorTic.IsInside = false;
            axis.MajorTic.IsOpposite = false;
            axis.MinorTic.IsOpposite = false;
            axis.MajorGrid.IsZeroLine = false;
            axis.Scale.Align = AlignP.Inside;
            axis.MajorGrid.IsVisible = true;

            curve.YAxisIndex = index;
            curve.IsY2Axis = isY2Axis;
        }

        /// <summary>
        /// Gets the curve.
        /// </summary>
        /// <param name="graphType">Type of the graph.</param>
        /// <param name="Controller">The Controller.</param>
        /// <returns>A curve of the given type</returns>
        private CurveItem GetCurve(string graphType, Controller Controller)
        {
            if (ReefStatusSettings.Instance.CustomGraph.ShowCurves.Contains(graphType))
            {
                if (this.curves.ContainsKey(graphType))
                {
                    return this.curves[graphType];
                }

                BaseInfo itemSettings = Controller.Items.FirstOrDefault(item => item.Id == graphType);
                if (itemSettings != null)
                {
                    CurveItem curve = this.GraphPane.AddCurve(itemSettings.DisplayName, new PointPairList(), itemSettings.Colour, SymbolType.None);
                    this.UpdateAxis(itemSettings.Units, curve);
                    this.curves.Add(graphType, curve);
                    return curve;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        private void UpdateSettings()
        {
            GraphPane.Title.IsVisible = false;

            this.UpdateCurves();

            if (ReefStatusSettings.Instance.CustomGraph.Range == GraphRange.Day || ReefStatusSettings.Instance.CustomGraph.Range == GraphRange.Week)
            {
                GraphPane.XAxis.Scale.MinorStep = 1.0;
                GraphPane.XAxis.Scale.MinorUnit = DateUnit.Hour;
                GraphPane.XAxis.Scale.MajorStep = 1.0;
                GraphPane.XAxis.Scale.MajorUnit = DateUnit.Day;
            }
            else if (ReefStatusSettings.Instance.CustomGraph.Range == GraphRange.Month)
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

        /// <summary>
        /// Updates the curves.
        /// </summary>
        private void UpdateCurves()
        {
            GraphPane.YAxisList.Clear();
            GraphPane.Y2AxisList.Clear();

            this.GraphPane.XAxis.MajorGrid.IsVisible = true;

            foreach (string curveName in this.curves.Keys)
            {
                string itemId = curveName.Split('_')[0];
                int ControllerId = int.Parse(curveName.Split('_')[1]);
                Controller Controller = ReefStatusSettings.Instance.Controllers.FirstOrDefault(item => item.Id == ControllerId);
                if (Controller != null)
                {
                    LineItem curve = (LineItem) this.GetCurve(itemId, Controller);
                    

                    BaseInfo itemSettings = Controller.Items.FirstOrDefault(item => item.Id == itemId);
                    if (itemSettings != null)
                    {
                        curve.Color = itemSettings.Colour;
                        curve.Line.Fill = new Fill(Color.FromArgb(200, itemSettings.Colour), Color.FromArgb(0, Color.White), 90F);
                        curve.Symbol.IsVisible = false;
                        this.UpdateAxis(itemSettings.Units, curve);
                    }
                }
            }
        }
    }
}