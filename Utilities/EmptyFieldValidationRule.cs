using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using ProjectEstimationTool.Properties;
using ProjectEstimationTool.ViewModels;

namespace ProjectEstimationTool.Utilities
{
    class EmptyFieldValidationRule : ValidationRule
    {
        private MainWindowViewModel mViewModel;
        private String mPropertyName;

        public override ValidationResult Validate(Object value, CultureInfo cultureInfo)
        {
            String text = value as String;
            if (String.IsNullOrWhiteSpace(text))
            {
                this.mViewModel.AddEditTaskDialogErrorsCollection.SetError(this.mPropertyName);
                return new ValidationResult(false, Resources.TaskItemValidation_EmptyField);
            }

            this.mViewModel.AddEditTaskDialogErrorsCollection.ClearError(this.mPropertyName);
            return new ValidationResult(true, null);
        }

        public override ValidationResult Validate(Object value, CultureInfo cultureInfo, BindingExpressionBase owner)
        {
            mViewModel = (owner as BindingExpression).DataItem as MainWindowViewModel;
            mPropertyName = (owner as BindingExpression).ResolvedSourcePropertyName;
            return base.Validate(value, cultureInfo, owner);
        }
    } // class EmptyFieldValidationRule
} // namespace ProjectEstimationTool.Utilities
