using System;
using System.Windows;

namespace ProjectEstimationTool.Views
{
    /// <summary>
    /// Interaction logic for EditWorkDayDate.xaml
    /// </summary>
    public partial class EditWorkDayDateDialog : Window
    {
        public EditWorkDayDateDialog()
        {
            InitializeComponent();
        }

        private void OnOkButtonClick(Object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
