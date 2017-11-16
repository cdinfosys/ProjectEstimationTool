using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ProjectEstimationTool.Classes;
using ProjectEstimationTool.Events;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool.Views
{
    /// <summary>
    /// Interaction logic for BurnDownChartView.xaml
    /// </summary>
    partial class BurnDownChartView : UserControl
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
        private CancellationTokenSource mReloadCancellationTokenSource;
        private SynchronizationContext mUISynchronizationContext;

        private static Object CancellationTokenSourceSynchLock = new Object();

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

        public CancellationToken GetUpdateCancellationToken()
        {
            lock (CancellationTokenSourceSynchLock)
            {
                if (this.mReloadCancellationTokenSource != null)
                {
                    this.mReloadCancellationTokenSource.Cancel();
                    this.mReloadCancellationTokenSource.Dispose();
                    this.mReloadCancellationTokenSource = null;
                }

                this.mReloadCancellationTokenSource = new CancellationTokenSource();
                return mReloadCancellationTokenSource.Token;
            }
        }

        private static async void OnSeries1PointsChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            BurnDownChartView control = dependencyObject as BurnDownChartView;

            try
            {
                CancellationToken cancellationToken = control.GetUpdateCancellationToken();
                await Task.Run
                (
                    () =>
                    {
                        control.IdealBurnDownSeriesPoints = eventArgs.NewValue as ObservableCollection<GraphPoint>;
                        // Create new set of labels for the x-axis
                        control.PostAction(() => control.horizontalAxisCanvas.Children.Clear());
                    },
                    cancellationToken
                );
            }
            catch (OperationCanceledException)
            {
                // Ignore cancellation exception.
            }
        }


        private static async void OnSeries2PointsChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            try
            {
                BurnDownChartView control = dependencyObject as BurnDownChartView;
                CancellationToken cancellationToken = control.GetUpdateCancellationToken();
                await Task.Run
                (
                    () =>
                    {
                        control.ActualBurnDownSeriesPoints = eventArgs.NewValue as ObservableCollection<GraphPoint>;
                        // Create new set of labels for the x-axis
                        control.PostAction(() => control.horizontalAxisCanvas.Children.Clear());
                    },
                    cancellationToken
                );
            }
            catch (OperationCanceledException)
            {
                // Ignore cancellation exception.
            }
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
                PostAction
                (
                    (o) => { horizontalAxis.Data = o as Geometry; }, 
                    Geometry.Parse(GetHorizontalAxisPoints(Convert.ToInt32(mControlWidth), 5))
                );
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
                PostAction
                (
                    (o) => { horizontalAxis.Data = o as Geometry; }, 
                    Geometry.Parse(GetHorizontalAxisPoints(Convert.ToInt32(mControlWidth), 5))
                );
                RecalculateGraphPoints();
            }
        }

        public BurnDownChartView()
        {
            this.mUISynchronizationContext = SynchronizationContext.Current;
            InitializeComponent();

            Utility.EventAggregator.GetEvent<ProjectModelChangedEvent>().Subscribe(OnProjectModelChanged);
        }

        public void RecalculateScale()
        {
            CalculateOnSizeChanged(new Size { Width = this.mControlWidth, Height = this.mControlHeight });
        }

        private void PostAction(Action action)
        {
            this.mUISynchronizationContext.Post
            (
                (o) => action(),
                null
            );
        }

        private void PostAction(Action<Object> action, Object state)
        {
            this.mUISynchronizationContext.Post
            (
                (o) => action(o),
                state
            );
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
            PostAction((pointsCollection) => this.idealBurnDownSeries.Points = new PointCollection(pointsCollection as IEnumerable<Point>), seriesPoints);

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
            PostAction((pointsCollection) => this.actualBurnDownSeries.Points = new PointCollection(pointsCollection as IEnumerable<Point>), seriesPoints);
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

                this.mUISynchronizationContext.Post
                (
                    new SendOrPostCallback
                    (
                        (labelPos) =>
                        {
                            var pos = labelPos as Tuple<Int32, Double>;
                            TextBlock textBlock = verticalAxisCanvas.Children[pos.Item1] as TextBlock;
                            Canvas.SetTop(verticalAxisCanvas.Children[pos.Item1], pos.Item2 - (textBlock.ActualHeight / 2.0));
                        }
                    ),
                    new Tuple<Int32, Double>(pointIndex, yPos)
                );

                yPos -= yStep;
            }

                this.mUISynchronizationContext.Post
                (
                    new SendOrPostCallback
                    (
                        (position) =>
                        {
                            TextBlock textBlock10 = verticalAxisCanvas.Children[10] as TextBlock;
                            Canvas.SetTop(verticalAxisCanvas.Children[10], (Double)yPos - (textBlock10.ActualHeight / 2.0));
                        }
                    ),
                    (Object)yPos
                );
            return result.ToString();
        }


        private String GetHorizontalAxisPoints(Int32 width, Int32 height)
        {
            StringBuilder result = new StringBuilder();
            Double xPos = Convert.ToDouble(mAxisBarSize + mAxisBarTextSize);

            result.AppendFormat("M{0},{1}", mAxisBarTextSize + mAxisBarSize, mControlHeight - mAxisBarTextSize);
            result.AppendFormat("l{0},{1}", 0, -mAxisBarSize);

            Double xStep = 0.0;
            if (this.mHorizontalPoints > 0)
            {
                xStep = (Convert.ToDouble(Math.Max(0, mControlWidth - mAxisBarSize - (2 * mAxisBarTextSize))) / Convert.ToDouble(this.mHorizontalPoints));
            }

            var barLabelPositions = new List<Double>();

            for (Int32 point = 0; point < this.mHorizontalPoints; ++point)
            {
                barLabelPositions.Add(xPos - 5.0);
                result.AppendFormat("l{0},{1}", xStep, 0);
                result.AppendFormat("l{0},{1}", 0, mAxisBarSize);
                result.AppendFormat("m{0},{1}", 0, -mAxisBarSize);
                xPos += xStep;
            }

            PostAction
            (
                (positions) => 
                {
                    var labelPositions = positions as Tuple<Double, List<Double>>;
                    if (horizontalAxisCanvas.Children.Count  < 1)
                    {
                        for (Int32 labelIndex = 1; labelIndex < labelPositions.Item2.Count; ++labelIndex)
                        {
                            var label = new TextBlock() { Text = labelIndex.ToString() };
                            Canvas.SetTop(label, labelPositions.Item1);
                            Canvas.SetLeft(label, labelPositions.Item2[labelIndex]);
                            horizontalAxisCanvas.Children.Add(label);
                        }
                    }
                    else
                    {
                        for (Int32 labelIndex = 1; labelIndex < labelPositions.Item2.Count; ++labelIndex)
                        {
                            var label = horizontalAxisCanvas.Children[labelIndex - 1] as TextBlock;
                            if (label != null)
                            {
                                Canvas.SetTop(label, labelPositions.Item1);
                                Canvas.SetLeft(label, labelPositions.Item2[labelIndex]);
                            }
                        }
                    }
                },
                new Tuple<Double, List<Double>>
                (
                    mControlHeight - Convert.ToDouble(mAxisBarTextSize),
                    barLabelPositions
                )
            );
            return result.ToString();
        }

        private async void OnBorderSizeChanged(Object sender, SizeChangedEventArgs e)
        {
            try
            {
                await Task.Run
                (
                    () => CalculateOnSizeChanged(e.NewSize),
                    GetUpdateCancellationToken()
                );
            }
            catch (OperationCanceledException)
            {
                // Ignore cancellation exception.
            }
        }

        private void CalculateOnSizeChanged(Size size)
        {
            Double areaLost = Convert.ToDouble(this.mAxisBarSize + (2 * mAxisBarTextSize));
            this.mVerticalScaling = (size.Height - areaLost) / 100.0;
            this.mHorizontalScaling =  (this.mHorizontalPoints < 1) ? 1.0 : ((size.Width - areaLost) / Convert.ToDouble(this.mHorizontalPoints));
            mControlWidth = size.Width;
            mControlHeight = size.Height;

            PostAction
            (
                (geometry) => verticalAxis.Data = geometry as Geometry,
                Geometry.Parse(GetVerticalAxisPoints(5, Convert.ToInt32(size.Height)))
            );
            PostAction
            (
                (geometry) => horizontalAxis.Data = geometry as Geometry,
                Geometry.Parse(GetHorizontalAxisPoints(Convert.ToInt32(size.Width), 5))
            );

            RecalculateGraphPoints();
        }

        private async void OnProjectModelChanged(ProjectModelState newProjectModelState)
        {
            if 
            (
                (newProjectModelState == ProjectModelState.NoProject) ||
                (newProjectModelState == ProjectModelState.Open)
            )
            {
                await Task.Run
                (
                    () =>
                    {
                        RecalculateScale();
                        RecalculateGraphPoints();
                    },
                    GetUpdateCancellationToken()
                );
            }
        }
    } // class BurnDownChartView
} // namespace ProjectEstimationTool.Views
