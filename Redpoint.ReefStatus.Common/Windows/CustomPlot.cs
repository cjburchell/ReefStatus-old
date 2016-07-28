// <copyright file="CustomPlot.cs" company="RedPoint Games">
// Copyright (c) RedPoint Games. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using RedPoint.ReefStatus.Common.Settings;
using ZedGraph;
using System.Linq;

namespace RedPoint.ReefStatus.Common.Graphs
{
    public class CustomPlot : ZedGraphControl
    {
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        private ReefStatusSettings settings;

        private readonly Dictionary<string, CurveItem> curves = new Dictionary<string, CurveItem>();

        /// <summary>
        /// Setups the specified settings.
        /// </summary>
        /// <param name="currentSettings">The current settings.</param>
        /// <param name="refresh">if set to <c>true</c> [refresh].</param>
        public void Setup(ReefStatusSettings currentSettings, bool refresh)
        {
            this.settings = currentSettings;

            GraphPane.XAxis.Title.Text = Language.GetString("Date/Time");
            GraphPane.Legend.IsVisible = false;

            GraphPane.XAxis.Type = AxisType.Date;
            GraphPane.XAxis.Scale.MaxGrace = 0;
            GraphPane.XAxis.Scale.MinGrace = 0;

            UpdateSettings();

            AxisChange();
            Invalidate();
        }

        /// <summary>
        /// Creates the image.
        /// </summary>
        /// <param name="imageSize">Size of the image.</param>
        /// <param name="datapoints">The datapoints.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>The created image</returns>
        public static Image CreateImage(Size imageSize, Dictionary<string, Collection<Common.ProfiLux.DataPoint>> datapoints, ReefStatusSettings settings)
        {
            CustomPlot control = new CustomPlot();
            control.Setup(settings, false);

            foreach (string graphType in datapoints.Keys)
            {
                string type = graphType;
                ItemSettings graphSetting = settings.Items.FirstOrDefault(item => item.ID == type);
                CurveItem curve = control.GraphPane.AddCurve(graphSetting.DisplayName, new PointPairList(), graphSetting.Colour, SymbolType.None);
                control.UpdateAxis(graphSetting.Units, curve);

                if (curve != null)
                {
                    foreach (Common.ProfiLux.DataPoint point in datapoints[graphType])
                    {
                        curve.AddPoint(new PointPair(new XDate(point.Time), point.Value));
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
        /// Adds the point.
        /// </summary>
        /// <param name="graphType">Type of the graph.</param>
        /// <param name="date">The date.</param>
        /// <param name="value">The value.</param>
        public void AddPoint(string graphType, DateTime date, double value)
        {
            CurveItem curve = GetCurve(graphType);
            if (curve != null)
            {
                curve.AddPoint(new PointPair(new XDate(date), value));

                if (settings.CustomGraph.Range != GraphRange.All)
                {
                    DateTime endTimeRange = DateTime.Now;
                    switch (settings.CustomGraph.Range)
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
        /// Clears the data.
        /// </summary>
        public void ClearAllData()
        {
            GraphPane.CurveList.Clear();
            curves.Clear();
        }

        /// <summary>
        /// Adds the points to the graph.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="refresh">if set to <c>true</c> [refresh].</param>
        public void AddPoints(Dictionary<string, Collection<Common.ProfiLux.DataPoint>> points, bool refresh)
        {
            ClearAllData();

            foreach (string graphType in points.Keys)
            {
                CurveItem curve = GetCurve(graphType);
                if (curve != null)
                {
                    foreach (Common.ProfiLux.DataPoint point in points[graphType])
                    {
                        curve.AddPoint(new PointPair(new XDate(point.Time), point.Value));
                    }
                }
            }

            if(refresh)
            {
                UpdateSettings();

                AxisChange();
                Invalidate();
            }
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

            curve.YAxisIndex = index;
            curve.IsY2Axis = isY2Axis;
        }

        private CurveItem GetCurve(string graphType)
        {
            if (settings.CustomGraph.ShowCurves.Contains(graphType))
            {
                if (curves.ContainsKey(graphType))
                {
                    return curves[graphType];
                }
                ItemSettings itemSettings = settings.Items.FirstOrDefault(item => item.ID == graphType);
                CurveItem curve = GraphPane.AddCurve(itemSettings.DisplayName, new PointPairList(), itemSettings.Colour, SymbolType.None);
                UpdateAxis(itemSettings.Units, curve);
                curves.Add(graphType, curve);
                return curve;
            }

            return null;
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        private void UpdateSettings()
        {
            GraphPane.Title.IsVisible = false;

            UpdateCurves();

            if (settings.CustomGraph.Range == GraphRange.Day || settings.CustomGraph.Range == GraphRange.Week)
            {
                GraphPane.XAxis.Scale.MinorStep = 1.0;
                GraphPane.XAxis.Scale.MinorUnit = DateUnit.Hour;
                GraphPane.XAxis.Scale.MajorStep = 1.0;
                GraphPane.XAxis.Scale.MajorUnit = DateUnit.Day;
            }
            else if (settings.CustomGraph.Range == GraphRange.Month)
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

        private void UpdateCurves()
        {
            GraphPane.YAxisList.Clear();
            GraphPane.Y2AxisList.Clear();

            foreach (string curveName in curves.Keys)
            {
                CurveItem curve = GetCurve(curveName);
                string name = curveName;
                ItemSettings itemSettings = settings.Items.FirstOrDefault(item => item.ID == name);
                curve.Color = itemSettings.Colour;
                UpdateAxis(itemSettings.Units, curve);
            }
        }

        public void AddPoints(string id, Collection<Common.ProfiLux.DataPoint> points)
        {
            CurveItem curve = GetCurve(id);
            if (curve != null)
            {
                foreach (Common.ProfiLux.DataPoint point in points)
                {
                    curve.AddPoint(new PointPair(new XDate(point.Time), point.Value));
                }

                UpdateSettings();

                AxisChange();
                Invalidate();
            }
        }
    }
}