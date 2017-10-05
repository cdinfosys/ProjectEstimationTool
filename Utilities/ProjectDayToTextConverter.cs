using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectEstimationTool.Utilities
{
    public class ProjectDayToTextConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Int32 inputValue;
            if (Int32.TryParse(value.ToString(), out inputValue))
            {
                if (inputValue > 0)
                {
                    return String.Format(culture, "{0:N}", inputValue);
                }
            }
            return "---";
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
