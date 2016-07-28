namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System.Windows.Controls;
    using System;

    internal class FontSizeListItem : TextBlock, IComparable
    {
        private double sizeInPoints;

        public FontSizeListItem(double sizeInPoints)
        {
            this.sizeInPoints = sizeInPoints;
            this.Text = sizeInPoints.ToString();
        }

        public override string ToString()
        {
            return sizeInPoints.ToString();
        }

        public double SizeInPoints
        {
            get { return sizeInPoints; }
        }

        public double SizeInPixels
        {
            get { return PointsToPixels(sizeInPoints); }
        }

        public static bool FuzzyEqual(double a, double b)
        {
            return Math.Abs(a - b) < 0.01;
        }

        int IComparable.CompareTo(object obj)
        {
            double value;

            if (obj is double)
            {
                value = (double)obj;
            }
            else
            {
                if (!double.TryParse(obj.ToString(), out value))
                {
                    return 1;
                }
            }

            return
                FuzzyEqual(sizeInPoints, value) ? 0 :
                (sizeInPoints < value) ? -1 : 1;
        }

        public static double PointsToPixels(double value)
        {
            return value * (96.0 / 72.0);
        }

        public static double PixelsToPoints(double value)
        {
            return value * (72.0 / 96.0);
        }
    }
}
