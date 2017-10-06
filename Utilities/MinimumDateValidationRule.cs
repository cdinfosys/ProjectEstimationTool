using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ProjectEstimationTool.Properties;

namespace ProjectEstimationTool.Utilities
{
    public class MinimumDateValidationRule : ValidationRule
    {
        private DateTime mMinimumDate = DateTime.MinValue;

        public static DependencyProperty MinimumDateProperty = DependencyProperty.RegisterAttached
        (
            "MinimumDate",
            typeof(DateTime),
            typeof(MinimumDateValidationRule),
            new PropertyMetadata(DateTime.MinValue)
        );

        public static DateTime GetMinimumDate(UIElement uiElement)
        {
            return (DateTime)(uiElement.GetValue(MinimumDateProperty));
        }

        public static void SetMinimumDate(UIElement uiElement, DateTime value)
        {
            uiElement.SetValue(MinimumDateProperty, value.Date);
        }

        public override ValidationResult Validate(Object value, CultureInfo cultureInfo)
        {
            DateTime enteredDate = (DateTime)value;
            if (enteredDate.Date <= this.mMinimumDate.Date)
            {
                return new ValidationResult(false, String.Format(Resources.ErrorDateBeforeLastWorkDay, this.mMinimumDate.ToString(cultureInfo.DateTimeFormat.ShortDatePattern)));
            }

            return new ValidationResult(true, null);
        }

        public override ValidationResult Validate(Object value, CultureInfo cultureInfo, BindingExpressionBase owner)
        {
            this.mMinimumDate = (DateTime)((owner as BindingExpression).Target as DependencyObject).GetValue(MinimumDateProperty);
            return base.Validate(value, cultureInfo, owner);
        }
    } // class MinimumDateValidationRule
} // namespace ProjectEstimationTool.Utilities
