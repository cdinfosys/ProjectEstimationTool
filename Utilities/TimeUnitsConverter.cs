using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ProjectEstimationTool.Classes;

namespace ProjectEstimationTool.Utilities
{
    class TimeUnitsConverter : Freezable, IValueConverter
    {
        public static readonly DependencyProperty MeasurementTypeProperty = DependencyProperty.Register
        (
            "MeasurementType",
            typeof(TimeMeasurementUnits),
            typeof(TimeUnitsConverter)
        );

        private static CultureInfo mFormatProvider = System.Globalization.CultureInfo.CurrentUICulture.Clone() as CultureInfo;

        static TimeUnitsConverter()
        {
            mFormatProvider.NumberFormat.NumberGroupSeparator = String.Empty;
            mFormatProvider.NumberFormat.NumberGroupSizes = new Int32[0];// { Int32.MaxValue };
        }

        protected override Freezable CreateInstanceCore()
        {
            return new TimeUnitsConverter();
        }

        public TimeMeasurementUnits MeasurementType 
        { 
            get 
            {
                return (TimeMeasurementUnits)GetValue(MeasurementTypeProperty);
            }
            set
            {
                SetValue(MeasurementTypeProperty, value);
            }
        }

        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Double inputMinutes;
            if (!Double.TryParse(value.ToString(), out inputMinutes))
            {
                return String.Empty;
            }

            switch (MeasurementType)
            {
                case TimeMeasurementUnits.Hours:
                    return String.Format(mFormatProvider, "{0:N2}", inputMinutes / 60.0);

                case TimeMeasurementUnits.Minutes:
                default:
                    return String.Format(mFormatProvider, "{0:N0}", inputMinutes);
            }
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if ((value == null) || (String.IsNullOrWhiteSpace(value.ToString())))
            {
                return 0.0;
            }

            String inputString = value.ToString().Trim();

            switch (MeasurementType)
            {
                case TimeMeasurementUnits.Hours:
                {
                    Double hours;
                    if (Double.TryParse(inputString, out hours))
                    {
                        return hours * 60.0;
                    }

                    return 0.0;
                }

                case TimeMeasurementUnits.Minutes:
                default:
                {
                    Double minutes;
                    if (Double.TryParse(inputString, out minutes))
                    {
                        return minutes;
                    }

                    return 0.0;
                }
            }
        }
    }
}
