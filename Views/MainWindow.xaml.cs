using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Prism.Events;
using ProjectEstimationTool.Classes;
using ProjectEstimationTool.Events;
using ProjectEstimationTool.Interfaces;
using ProjectEstimationTool.Properties;
using ProjectEstimationTool.Utilities;
using ProjectEstimationTool.ViewModels;

namespace ProjectEstimationTool.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    partial class MainWindow : Window, IMainWindowView
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        public MainWindow()
        {
            Utility.CaptureMainWindowSynchronizationContext();

            InitializeComponent();

            Utility.EventAggregator.GetEvent<ProjectModelUserInputRequiredEvent>().Subscribe(u => GetUserInput(u));
            Utility.EventAggregator.GetEvent<ProjectModelFilePathRequiredEvent>().Subscribe(u => GetFilePath(u));
            Utility.EventAggregator.GetEvent<ExitProgramEvent>().Subscribe(u => CloseMainWindow(u));
            Utility.EventAggregator.GetEvent<ShowEditItemEvent>().Subscribe(() => EditTask());
            Utility.EventAggregator.GetEvent<ShowAddItemEvent>().Subscribe(() => AddTask());
            Utility.EventAggregator.GetEvent<ShowWorkDayDialogEvent>().Subscribe(() => ShowWorkDayDialog());
            Utility.EventAggregator.GetEvent<ShowEditProjectPropertiesEvent>().Subscribe(() => ShowProjectPropertiesDialog());
            Utility.EventAggregator.GetEvent<GetExportFileNameEvent>().Subscribe(ShowExportFileNameDialog, ThreadOption.UIThread);
            (this.DataContext as MainWindowViewModel).OnNewDocument();
        }

        private void CloseMainWindow(Int32 programExitCode)
        {
            this.Close();
        }

        private void ExitCommandHandler(Object sender, ExecutedRoutedEventArgs eventArgs)
        {
            eventArgs.Handled = true;
            this.Close();
        }

        private void AboutCommandHandler(Object sender, ExecutedRoutedEventArgs eventArgs)
        {
            MessageBox.Show(this, "Not implemented", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void EditTask()
        {
            AddEditTaskDialog dialog = new AddEditTaskDialog()
            {
                Owner = this
            };
            dialog.ShowDialog();
        }

        private void AddTask()
        {
            AddEditTaskDialog dialog = new AddEditTaskDialog()
            {
                Owner = this
            };
            dialog.ShowDialog();
        }

        private void ShowWorkDayDialog()
        {
            EditWorkDayDateDialog dialog = new EditWorkDayDateDialog()
            {
                Owner = this
            };
            dialog.ShowDialog();
        }

        private void ShowProjectPropertiesDialog()
        {
            EditProjectPropertiesDialog dialog = new EditProjectPropertiesDialog()
            {
                Owner = this,
                DataContext = this.DataContext
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
            // Set the initial directory to the previous directory that was used.
            String initialDirectory = Settings.Default.PreviousProjectFolderPath;
            if (String.IsNullOrWhiteSpace(initialDirectory))
            {
                initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            MessageBoxResult mbResult = MessageBoxResult.None;

            // Directory that was selected for the file
            String selectedFilePath;

            if (payload.ShowOpenDialog)
            {
                var openFileDialog = new OpenFileDialog()
                {
                    AddExtension = true,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    DereferenceLinks = true,
                    InitialDirectory = initialDirectory,
                    ValidateNames = true,
                    DefaultExt = ProjectEstimationTool.Properties.Resources.ProjectFileDefaultExtension,
                    Filter = ProjectEstimationTool.Properties.Resources.ProjectFileOpenSaveFilter,
                    Multiselect = false,
                    ShowReadOnly = false
                };

                mbResult = openFileDialog.ShowDialog(this).GetValueOrDefault(false) ? MessageBoxResult.OK : MessageBoxResult.Cancel;
                selectedFilePath = openFileDialog.FileName;
                payload.CurrentStep.ContinueWithUserInput(mbResult, openFileDialog.FileName)?.Execute();
            }
            else
            {
                var saveFileDialog = new SaveFileDialog()
                {
                    AddExtension = true,
                    CheckFileExists = false,
                    CheckPathExists = true,
                    DereferenceLinks = true,
                    InitialDirectory = initialDirectory,
                    ValidateNames = true,
                    DefaultExt = ProjectEstimationTool.Properties.Resources.ProjectFileDefaultExtension,
                    Filter = ProjectEstimationTool.Properties.Resources.ProjectFileOpenSaveFilter
                };

                mbResult = saveFileDialog.ShowDialog(this).GetValueOrDefault(false) ? MessageBoxResult.OK : MessageBoxResult.Cancel;
                selectedFilePath = saveFileDialog.FileName;
                payload.CurrentStep.ContinueWithUserInput(mbResult, saveFileDialog.FileName)?.Execute();
            }

            // Store the directory last used to save or open a file.
            if (mbResult == MessageBoxResult.OK)
            {
                String selectedDir = Path.GetDirectoryName(selectedFilePath);
                if (Directory.Exists(selectedDir))
                {
                    Settings.Default.PreviousProjectFolderPath = selectedDir;
                    Utility.SaveSettingsAsync(Settings.Default);
                }
            }
        }

        /// <summary>
        /// Show a dialog box where the user can set the path for the export file.
        /// </summary>
        private void ShowExportFileNameDialog()
        {
            // Set the initial directory to the previous directory that was used.
            String initialDirectory = Settings.Default.PreviousExcelExportFolderPath;
            if (String.IsNullOrWhiteSpace(initialDirectory))
            {
                initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            var mExportFileDialog = new SaveFileDialog()
            {
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = true,
                DereferenceLinks = true,
                InitialDirectory = initialDirectory,
                ValidateNames = true,
                DefaultExt = ProjectEstimationTool.Properties.Resources.ProjectExportDefaultExtension,
                Filter = ProjectEstimationTool.Properties.Resources.ProjectExportFileNameFilter
            };

            if (mExportFileDialog.ShowDialog(this) ?? false)
            {
                String selectedDir = Path.GetDirectoryName(mExportFileDialog.FileName);
                if (Directory.Exists(selectedDir))
                {
                    Settings.Default.PreviousExcelExportFolderPath = selectedDir;
                    Utility.SaveSettingsAsync(Settings.Default);
                }

                Utility.EventAggregator.GetEvent<ExportProjectToExcelEvent>().Publish(mExportFileDialog.FileName);
            }
        }

        private void OnClosingMainWindow(Object sender, System.ComponentModel.CancelEventArgs eventArgs)
        {
            (this.DataContext as MainWindowViewModel).OnCloseMainWindow();
            eventArgs.Cancel = (this.DataContext as MainWindowViewModel).CanCloseMainWindow ? false : true;
        }

        private void OnTreeViewMouseDoubleClick(Object sender, MouseButtonEventArgs eventArgs)
        {
            (DataContext as MainWindowViewModel).EditTaskCommand.Execute(null);
        }

        private void OnTabItemGotFocus(Object sender, RoutedEventArgs e)
        {
            if (Object.ReferenceEquals(sender, burnDownChartTab))
            {
                burnDownChartView.RecalculateScale();
            }
        }
    } // class MainWindow 

    /// <summary>
    /// Commands for main menu bar
    /// </summary>
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
} // namespace ProjectEstimationTool.Views
