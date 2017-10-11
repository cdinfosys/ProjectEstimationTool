using System;
using System.Windows;
using System.Windows.Controls;

namespace ProjectEstimationTool.Views
{
    /// <summary>
    /// Interaction logic for EditProjectPropertiesDialog.xaml
    /// </summary>
    public partial class EditProjectPropertiesDialog : Window
    {
        public EditProjectPropertiesDialog()
        {
            InitializeComponent();
        }
 
        private void OnOkButtonClick(Object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            e.Handled = true;
        }

        private void OnTextBoxGotFocus(Object sender, RoutedEventArgs eventArgs)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }
   } // class EditProjectPropertiesDialog
} // namespace ProjectEstimationTool.Views
