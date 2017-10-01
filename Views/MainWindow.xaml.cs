using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using ProjectEstimationTool.Classes;
using ProjectEstimationTool.Events;
using ProjectEstimationTool.Interfaces;
using ProjectEstimationTool.Utilities;
using ProjectEstimationTool.ViewModels;

namespace ProjectEstimationTool.Views
{
    internal static class UserInterfaceCommands
    {
        /// <summary>
        ///     Event for the About menu item
        /// </summary>
        public static RoutedUICommand About = new RoutedUICommand();
        public static RoutedUICommand Exit = new RoutedUICommand();
        public static RoutedUICommand EditTaskCommand = new RoutedUICommand("EditTask", nameof(EditTaskCommand), typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.Enter, ModifierKeys.Control) });

        //public static RoutedCommand 
        static UserInterfaceCommands()
        {
            ApplicationCommands.New.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            ApplicationCommands.Open.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            ApplicationCommands.Save.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            ApplicationCommands.Close.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));
        }
    } // class UserInterfaceCommands

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindowView
    {
        private SaveFileDialog mSaveFileDialog;
        private OpenFileDialog mOpenFileDialog;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            mSaveFileDialog = new SaveFileDialog()
            {
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = true,
                DereferenceLinks = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                ValidateNames = true,
                DefaultExt = ".petproj",
                Filter = "Estimation Tool Projects|*.petproj|All Files|*.*"
            };

            mOpenFileDialog = new OpenFileDialog()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                ValidateNames = true,
                DefaultExt = ".petproj",
                Filter = "Estimation Tool Projects|*.petproj|All Files|*.*",
                Multiselect = false,
                ShowReadOnly = false
            };

            Utility.EventAggregator.GetEvent<ProjectModelUserInputRequiredEvent>().Subscribe(u => GetUserInput(u));
            Utility.EventAggregator.GetEvent<ProjectModelFilePathRequiredEvent>().Subscribe(u => GetFilePath(u));
            Utility.EventAggregator.GetEvent<ExitProgramEvent>().Subscribe(u => CloseMainWindow(u));

            (this.DataContext as MainWindowViewModel).OnNewDocument();
        }

        private void CloseMainWindow(Int32 programExitCode)
        {
            this.Close();
        }

        private void NewCommandHandler(Object sender, ExecutedRoutedEventArgs eventArgs)
        {
            (this.DataContext as MainWindowViewModel).OnNewDocument();
            eventArgs.Handled = true;
        }

        private void OpenCommandHandler(Object sender, ExecutedRoutedEventArgs eventArgs)
        {
            (this.DataContext as MainWindowViewModel).OnOpenDocument();
            eventArgs.Handled = true;
        }

        private void SaveCommandHandler(Object sender, ExecutedRoutedEventArgs eventArgs)
        {
            (this.DataContext as MainWindowViewModel).OnSaveDocument();
            eventArgs.Handled = true;
        }

        private void SaveAsCommandHandler(Object sender, ExecutedRoutedEventArgs eventArgs)
        {
            (this.DataContext as MainWindowViewModel).OnSaveDocumentAs();
            eventArgs.Handled = true;
        }

        private void CloseCommandHandler(Object sender, ExecutedRoutedEventArgs eventArgs)
        {
            (this.DataContext as MainWindowViewModel).OnCloseDocument();
            eventArgs.Handled = true;
        }

        private void ExitCommandHandler(Object sender, ExecutedRoutedEventArgs eventArgs)
        {
            (this.DataContext as MainWindowViewModel).OnCloseMainWindow();
            eventArgs.Handled = true;
        }

        private void AboutCommandHandler(Object sender, ExecutedRoutedEventArgs eventArgs)
        {
            MessageBox.Show(this, "Not implemented", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void AddEditTaskCommandHandler(Object sender, ExecutedRoutedEventArgs eventArgs)
        {
            AddEditTaskDialog dialog = new AddEditTaskDialog()
            {
                Owner = this
            };
            dialog.ShowDialog();
        }

        private void TreeView_SelectedItemChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            ProjectTreeItemBase selectedTreeItem = e.NewValue as ProjectTreeItemBase;
            (this.DataContext as MainWindowViewModel).SelectedTaskItem = selectedTreeItem;
        }

        private void GetUserInput(ProjectModelUserInputRequiredEventPayload payload)
        {
            payload.CurrentStep.ContinueWithUserInput
            (
                MessageBox.Show
                (
                    this,
                    payload.MessageText,
                    payload.MessageBoxCaption,
                    payload.Buttons,
                    payload.Icon,
                    payload.DefaultButton
                ),
                null
            )?.Execute();
        }

        private void GetFilePath(ProjectModelFilePathRequiredEventPayload payload)
        {
            MessageBoxResult mbResult = MessageBoxResult.None;

            if (payload.ShowOpenDialog)
            {
                mbResult = mOpenFileDialog.ShowDialog(this).GetValueOrDefault(false) ? MessageBoxResult.OK : MessageBoxResult.Cancel;
                payload.CurrentStep.ContinueWithUserInput(mbResult, mOpenFileDialog.FileName)?.Execute();
            }
            else
            {
                mbResult = mSaveFileDialog.ShowDialog(this).GetValueOrDefault(false) ? MessageBoxResult.OK : MessageBoxResult.Cancel;
                payload.CurrentStep.ContinueWithUserInput(mbResult, mSaveFileDialog.FileName)?.Execute();
            }
        }

        private void OnClosingMainWindow(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            (this.DataContext as MainWindowViewModel).OnCloseMainWindow();
        }
    } // class MainWindow 
} // namespace ProjectEstimationTool.Views
