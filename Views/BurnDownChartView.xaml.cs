using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectEstimationTool.Classes;

namespace ProjectEstimationTool.Views
{
    /// <summary>
    /// Interaction logic for BurnDownChartView.xaml
    /// </summary>
    public partial class BurnDownChartView : UserControl
    {
        private Double mControlWidth = 0.0;

        public static DependencyProperty Series1PointsProperty = DependencyProperty.Register
        (
            "Series1Points",
            typeof(ObservableCollection<GraphPoint>),
            typeof(BurnDownChartView),
            new PropertyMetadata(new ObservableCollection<GraphPoint>(), OnSeries1PointsChangedCallback)
        );

        public ObservableCollection<GraphPoint> Series1Points
        {
            get { return GetValue(Series1PointsProperty) as ObservableCollection<GraphPoint>; }
            set { SetValue(Series1PointsProperty, value); }
        }

        private static void OnSeries1PointsChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            int i = 0;
        }

        public BurnDownChartView()
        {
            InitializeComponent();
            verticalAxis.Data = Geometry.Parse(GetVerticalAxisPoints(5, 100));
        }

        private String GetVerticalAxisPoints(Int32 width, Int32 height)
        {
            StringBuilder result = new StringBuilder();
            result.Append("M0,0");
            Double yStep = (Convert.ToDouble(height) / 10.0);
            Double yPos = 0.0;
            for (Int32 point = 0; point < 10; ++point)
            {
                result.AppendFormat("L{0},{1}", width, Convert.ToInt32(yPos));
                result.AppendFormat("L{0},{1}", width, Convert.ToInt32(yPos + yStep));
                result.AppendFormat("L0,{0}", Convert.ToInt32(yPos + yStep));
                result.AppendFormat("M{0},{1}", width, Convert.ToInt32(yPos + yStep));
                yPos += yStep;
            }

            return result.ToString();
        }

        private void Border_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            verticalAxis.Data = Geometry.Parse(GetVerticalAxisPoints(5, Convert.ToInt32(e.NewSize.Height)));
            mControlWidth = e.NewSize.Width;
        }
    }
}
