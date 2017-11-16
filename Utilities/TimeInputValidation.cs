using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ProjectEstimationTool.Classes;
using ProjectEstimationTool.Properties;
using ProjectEstimationTool.ViewModels;

namespace ProjectEstimationTool.Utilities
{
    class TimeInputValidation : ValidationRule
    {
        private MainWindowViewModel mViewModel;
        private TimeMeasurementUnits mMeasurementUnits = TimeMeasurementUnits.Unknown;
        private String mPropertyName;
        private Boolean mValidateForAddEditDialog = false;
        private Double mMinimumValue = Double.MinValue;
        private Double mMaximumValue = Double.MaxValue;

        public static DependencyProperty MinimumValueProperty = DependencyProperty.RegisterAttached
        (
            "MinimumValue",
            typeof(Double),
            typeof(TimeInputValidation),
            new PropertyMetadata(Double.MinValue)
        );

        public static DependencyProperty MaximumValueProperty = DependencyProperty.RegisterAttached
        (
            "MaximumValue",
            typeof(Double),
            typeof(TimeInputValidation),
            new PropertyMetadata(Double.MaxValue)
        );

        public static Double GetMinimumValue(UIElement uiElement)
        {
            return (Double)(uiElement.GetValue(MinimumValueProperty));
        }

        public static void SetMinimumValue(UIElement uiElement, Double value)
        {
            uiElement.SetValue(MinimumValueProperty, value);
        }


        public static Double GetMaximumValue(UIElement uiElement)
        {
            return (Double)(uiElement.GetValue(MaximumValueProperty));
        }

        public static void SetMaximumValue(UIElement uiElement, Double value)
        {
            uiElement.SetValue(MaximumValueProperty, value);
        }

        public override ValidationResult Validate(Object value, CultureInfo cultureInfo)
        {
            switch (mMeasurementUnits)
            {
                case TimeMeasurementUnits.Hours:
                    {
                        Double convertedValue;
                        if (!Double.TryParse(value.ToString(), out convertedValue))
                        {
                            if (mValidateForAddEditDialog)
                            {
                                this.mViewModel.AddEditTaskDialogErrorsCollection.SetError(this.mPropertyName);
                            }
                            return new ValidationResult(false, Resources.ErrorNotNumeric);
                        }
                        if (convertedValue < (this.mMinimumValue / 60.0))
                        {
                            if (mValidateForAddEditDialog)
                            {
                                this.mViewModel.AddEditTaskDialogErrorsCollection.SetError(this.mPropertyName);
                            }
                            return new ValidationResult(false, String.Format(Resources.ErrorLessThanMinimumHours, (this.mMinimumValue / 60.0)));
                        }
                        if (convertedValue > (this.mMaximumValue / 60.0))
                        {
                            if (mValidateForAddEditDialog)
                            {
                                this.mViewModel.AddEditTaskDialogErrorsCollection.SetError(this.mPropertyName);
                            }
                            return new ValidationResult(false, String.Format(Resources.ErrorGreaterThanMaxHours, (this.mMaximumValue / 60.0)));
                        }
                    }
                    break;

                case TimeMeasurementUnits.Minutes:
                default:
                    {
                        Int32 convertedValue;
                        if (!Int32.TryParse(value.ToString(), out convertedValue))
                        {
                            if (mValidateForAddEditDialog)
                            {
                                this.mViewModel.AddEditTaskDialogErrorsCollection.SetError(this.mPropertyName);
                            }
                            return new ValidationResult(false, Resources.ErrorNotNumeric);
                        }
                        if (convertedValue < this.mMinimumValue)
                        {
                            if (mValidateForAddEditDialog)
                            {
                                this.mViewModel.AddEditTaskDialogErrorsCollection.SetError(this.mPropertyName);
                            }
                            return new ValidationResult(false, String.Format(Resources.ErrorLessThanMinimumMinutes, this.mMinimumValue));
                        }
                        if (convertedValue > this.mMaximumValue)
                        {
                            if (mValidateForAddEditDialog)
                            {
                                this.mViewModel.AddEditTaskDialogErrorsCollection.SetError(this.mPropertyName);
                            }
                            return new ValidationResult(false, String.Format(Resources.ErrorGreaterThanMaxMinutes, this.mMaximumValue));
                        }
                    }

                    break;
            }

            if (mValidateForAddEditDialog)
            {
                this.mViewModel.AddEditTaskDialogErrorsCollection.ClearError(this.mPropertyName);
            }
            return new ValidationResult(true, null);
        }

        public override ValidationResult Validate(Object value, CultureInfo cultureInfo, BindingExpressionBase owner)
        {
            this.mViewModel = (owner as BindingExpression).DataItem as MainWindowViewModel;
            this.mPropertyName = (owner as BindingExpression).ResolvedSourcePropertyName;
            this.mMeasurementUnits = ((owner as BindingExpression).DataItem as MainWindowViewModel).SelectedTimeUnits;
            this.mMinimumValue = (Double)((owner as BindingExpression).Target as DependencyObject).GetValue(MinimumValueProperty);
            this.mMaximumValue = (Double)((owner as BindingExpression).Target as DependencyObject).GetValue(MaximumValueProperty);
            return base.Validate(value, cultureInfo, owner);
        }

        public Boolean ValidateForAddEditDialog
        {
            get
            {
                return this.mValidateForAddEditDialog;
            }
            set
            {
                this.mValidateForAddEditDialog = value;
            }
        }
    }
}
