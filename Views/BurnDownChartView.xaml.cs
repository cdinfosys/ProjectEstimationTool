using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ProjectEstimationTool.Classes;

namespace ProjectEstimationTool.Views
{
    /// <summary>
    /// Interaction logic for BurnDownChartView.xaml
    /// </summary>
    public partial class BurnDownChartView : UserControl
    {
        private Double mControlWidth = 0.0;
        private Double mControlHeight = 0.0;
        private Double mHorizontalScaling = 1.0;
        private Double mVerticalScaling = 1.0;
        private Int32 mHorizontalPoints = 0;
        private readonly Int32 mAxisBarSize = 5;
        private readonly Int32 mAxisBarTextSize = 30;
        private ObservableCollection<GraphPoint> mIdealBurnDownSeriesPoints;
        private ObservableCollection<GraphPoint> mActualBurnDownSeriesPoints;

        public static DependencyProperty sSeries1PointsProperty;
        public static DependencyProperty sSeries2PointsProperty;

        static BurnDownChartView()
        {
            ObservableCollection<GraphPoint> series1Collection = new ObservableCollection<GraphPoint>();
            sSeries1PointsProperty = DependencyProperty.Register
            (
                nameof(BurnDownChartView.Series1Points),
                series1Collection.GetType(),
                typeof(BurnDownChartView),
                new PropertyMetadata(series1Collection, OnSeries1PointsChangedCallback)
            );

            ObservableCollection<GraphPoint> series2Collection = new ObservableCollection<GraphPoint>();
            sSeries1PointsProperty = DependencyProperty.Register
            (
                nameof(BurnDownChartView.Series2Points),
                series2Collection.GetType(),
                typeof(BurnDownChartView),
                new PropertyMetadata(series1Collection, OnSeries2PointsChangedCallback)
            );
        }

        public ObservableCollection<GraphPoint> Series1Points
        {
            get { return GetValue(sSeries1PointsProperty) as ObservableCollection<GraphPoint>; }
            set { SetValue(sSeries1PointsProperty, value); }
        }

        public ObservableCollection<GraphPoint> Series2Points
        {
            get { return GetValue(sSeries2PointsProperty) as ObservableCollection<GraphPoint>; }
            set { SetValue(sSeries2PointsProperty, value); }
        }

        private static void OnSeries1PointsChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            BurnDownChartView control = dependencyObject as BurnDownChartView;
            control.IdealBurnDownSeriesPoints = eventArgs.NewValue as ObservableCollection<GraphPoint>;
        }

        private static void OnSeries2PointsChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            BurnDownChartView control = dependencyObject as BurnDownChartView;
            control.ActualBurnDownSeriesPoints = eventArgs.NewValue as ObservableCollection<GraphPoint>;
        }

        public ObservableCollection<GraphPoint> IdealBurnDownSeriesPoints
        {
            get
            {
                return this.mIdealBurnDownSeriesPoints;
            }
            set
            {
                this.mIdealBurnDownSeriesPoints = value;
                this.mHorizontalPoints = Math.Max(value?.Count ?? 0, ActualBurnDownSeriesPoints?.Count ?? 0);
                horizontalAxis.Data = Geometry.Parse(GetHorizontalAxisPoints(Convert.ToInt32(mControlWidth), 5));
                RecalculateGraphPoints();
            }
        }

        public ObservableCollection<GraphPoint> ActualBurnDownSeriesPoints
        {
            get
            {
                return this.mActualBurnDownSeriesPoints;
            }
            set
            {
                this.mActualBurnDownSeriesPoints = value;
                this.mHorizontalPoints = Math.Max(value?.Count ?? 0, IdealBurnDownSeriesPoints?.Count ?? 0);
                horizontalAxis.Data = Geometry.Parse(GetHorizontalAxisPoints(Convert.ToInt32(mControlWidth), 5));
                RecalculateGraphPoints();
            }
        }

        public BurnDownChartView()
        {
            InitializeComponent();
        }

        private void RecalculateGraphPoints()
        {
            List<Point> seriesPoints = new List<Point>();
            Double xOffset = Convert.ToDouble(mAxisBarSize + mAxisBarTextSize);
            Double yOffset = Convert.ToDouble(mAxisBarTextSize);

            if (this.mIdealBurnDownSeriesPoints != null)
            {
                foreach (GraphPoint graphPoint in this.mIdealBurnDownSeriesPoints)
                {
                    seriesPoints.Add
                    (
                        new Point
                        (
                            (Convert.ToDouble(graphPoint.X) * this.mHorizontalScaling) + xOffset, 
                            (Convert.ToDouble(graphPoint.Y) * this.mVerticalScaling) + yOffset
                        )
                    );
                }
            }
            this.idealBurnDownSeries.Points = new PointCollection(seriesPoints);

            seriesPoints = new List<Point>();
            if (this.mActualBurnDownSeriesPoints != null)
            {
                foreach (GraphPoint graphPoint in this.mActualBurnDownSeriesPoints)
                {
                    seriesPoints.Add
                    (
                        new Point
                        (
                            (Convert.ToDouble(graphPoint.X) * this.mHorizontalScaling) + xOffset, 
                            (Convert.ToDouble(graphPoint.Y) * this.mVerticalScaling) + yOffset
                        )
                    );
                }
            }
            this.actualBurnDownSeries.Points = new PointCollection(seriesPoints);
        }

        private String GetVerticalAxisPoints(Int32 width, Int32 height)
        {
            Double yStep = (Convert.ToDouble(Math.Max(0, mControlHeight - mAxisBarSize - (2 * mAxisBarTextSize))) / 10.0);
            Double yPos = mControlHeight - mAxisBarSize - mAxisBarTextSize;

            StringBuilder result = new StringBuilder();
            result.AppendFormat("M{0},{1}", mAxisBarTextSize, mControlHeight - mAxisBarSize - mAxisBarTextSize);
            result.AppendFormat("l{0},{1}", mAxisBarSize, 0);

            for (Int32 pointIndex = 0; pointIndex < 10; ++pointIndex)
            {
                result.AppendFormat("l{0},{1}", 0, -yStep);
                result.AppendFormat("l{0},{1} ", -mAxisBarSize, 0);
                result.AppendFormat("m{0},{1}", mAxisBarSize, 0);
                TextBlock textBlock = canvas.Children[pointIndex] as TextBlock;
                Canvas.SetTop(canvas.Children[pointIndex], yPos - (textBlock.ActualHeight / 2.0));
                yPos -= yStep;
            }
            TextBlock textBlock10 = canvas.Children[10] as TextBlock;
            Canvas.SetTop(canvas.Children[10], yPos - (textBlock10.ActualHeight / 2.0));
            return result.ToString();
        }


        private String GetHorizontalAxisPoints(Int32 width, Int32 height)
        {
            StringBuilder result = new StringBuilder();
            Double xPos = Convert.ToDouble(mAxisBarSize + mAxisBarTextSize);

            result.AppendFormat("M{0},{1}", mAxisBarTextSize, mControlHeight - mAxisBarTextSize);
            result.AppendFormat("l{0},{1}", 0, mAxisBarSize);

            Double xStep = 0.0;
            if (this.mHorizontalPoints > 0)
            {
                xStep = (Convert.ToDouble(Math.Max(0, mControlWidth - mAxisBarSize - (2 * mAxisBarTextSize))) / Convert.ToDouble(this.mHorizontalPoints));
            }

            for (Int32 point = 0; point < this.mHorizontalPoints; ++point)
            {
                result.AppendFormat("l{0},{1}", xStep, 0);
                result.AppendFormat("l{0},{1}", 0, mAxisBarSize);
                result.AppendFormat("m{0},{1}", 0, -mAxisBarSize);
                xPos += xStep;
            }

            return result.ToString();
        }

        private void OnBorderSizeChanged(Object sender, SizeChangedEventArgs e)
        {
            Double areaLost = Convert.ToDouble(this.mAxisBarSize + (2 * mAxisBarTextSize));
            this.mVerticalScaling = (e.NewSize.Height - areaLost) / 100.0;
            this.mHorizontalScaling =  (this.mHorizontalPoints < 1) ? 1.0 : ((e.NewSize.Width - areaLost) / Convert.ToDouble(this.mHorizontalPoints));
            mControlWidth = e.NewSize.Width;
            mControlHeight = e.NewSize.Height;
            verticalAxis.Data = Geometry.Parse(GetVerticalAxisPoints(5, Convert.ToInt32(e.NewSize.Height)));
            horizontalAxis.Data = Geometry.Parse(GetHorizontalAxisPoints(Convert.ToInt32(e.NewSize.Width), 5));
            RecalculateGraphPoints();
        }
    }
}
