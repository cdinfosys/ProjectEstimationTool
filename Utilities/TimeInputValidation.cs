using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using ProjectEstimationTool.Classes;
using ProjectEstimationTool.Properties;
using ProjectEstimationTool.ViewModels;

namespace ProjectEstimationTool.Utilities
{
    public class TimeInputValidation : ValidationRule
    {
        TimeMeasurementUnits mMeasurementUnits = TimeMeasurementUnits.Unknown;

        public override ValidationResult Validate(Object value, CultureInfo cultureInfo)
        {
            switch (mMeasurementUnits)
            {
                case TimeMeasurementUnits.Hours:
                    {
                        Double convertedValue;
                        if (!Double.TryParse(value.ToString(), out convertedValue))
                        {
                            return new ValidationResult(false, Resources.ErrorNotNumeric);
                        }
                        if (convertedValue < 0.0)
                        {
                            return new ValidationResult(false, Resources.ErrorLessThanZero);
                        }

                        if (convertedValue > (99999.0 / 60.0))
                        {
                            return new ValidationResult(false, Resources.ErrorGreaterThanMaxHours);
                        }
                    }
                    break;

                case TimeMeasurementUnits.Minutes:
                default:
                    {
                        Int32 convertedValue;
                        if (!Int32.TryParse(value.ToString(), out convertedValue))
                        {
                            return new ValidationResult(false, Resources.ErrorNotNumeric);
                        }
                        if (convertedValue < 0)
                        {
                            return new ValidationResult(false, Resources.ErrorLessThanZero);
                        }

                        if (convertedValue > 99999)
                        {
                            return new ValidationResult(false, Resources.ErrorGreaterThanMaxMinutes);
                        }
                    }

                    break;
            }

            return new ValidationResult(true, null);
        }

        public override ValidationResult Validate(Object value, CultureInfo cultureInfo, BindingExpressionBase owner)
        {
            this.mMeasurementUnits = ((owner as BindingExpression).DataItem as MainWindowViewModel).SelectedTimeUnits;
            return base.Validate(value, cultureInfo, owner);
        }
    }
}
