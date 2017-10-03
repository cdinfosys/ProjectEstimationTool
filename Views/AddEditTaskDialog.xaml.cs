using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ProjectEstimationTool.Views
{
    /// <summary>
    /// Interaction logic for AddEditTaskDialog.xaml
    /// </summary>
    public partial class AddEditTaskDialog : Window
    {
        public AddEditTaskDialog()
        {
            InitializeComponent();
        }

        private void OnTextBoxGotFocus(Object sender, RoutedEventArgs eventArgs)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        private void OnOkButtonClick(Object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnTextBoxTextChanged(Object sender, TextChangedEventArgs e)
        {
            if (!Object.ReferenceEquals(sender, MinimumTimeTextBox))
            {
                MinimumTimeTextBox?.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }

            if (!Object.ReferenceEquals(sender, MaximumTimeTextBox))
            {
                MaximumTimeTextBox?.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }

            if (!Object.ReferenceEquals(sender, EstimatedTimeTextBox))
            {
                EstimatedTimeTextBox?.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }
        }
    }
}
