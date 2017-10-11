﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Commands;
using ProjectEstimationTool.Classes;
using ProjectEstimationTool.Events;
using ProjectEstimationTool.Interfaces;
using ProjectEstimationTool.Properties;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        #region Fields
        private ObservableCollection<ProjectTreeItemBase> mProjectItemsTreeData = new ObservableCollection<ProjectTreeItemBase>();
        private ProjectTreeItemBase mSelectedTaskItem = null;
        private TimeMeasurementUnits mSelectedTimeUnits = TimeMeasurementUnits.Unknown;
        private ProjectEstimationTool.Model.ProjectModel mProjectModel = new ProjectEstimationTool.Model.ProjectModel();
        private Boolean mCanCloseMainWindow = false;
        private PropertyValidationErrorsCollection mAddEditTaskDialogErrorsCollection;
        private PropertyValidationErrorsCollection mEditWorkDayDateDialogErrorsCollection;
        private ICommand mSaveCommand;
        private ICommand mCloseCommand;
        private ICommand mNewCommand;
        private ICommand mLoadCommand;
        private ICommand mProjectPropertiesCommand;
        private ICommand mAddTaskCommand;
        private ICommand mEditTaskCommand;
        private ICommand mDeleteTaskCommand;
        private ICommand mAddWorkDayCommand;
        private ICommand mAddEditTaskDialogOkButtonCommand;
        private ICommand mEditWorkDayDateOkButtonCommand;
        private ICommand mEditProjectPropertiesDialogOkButtonCommand;
        private Boolean mIsEditingExistingItem;
        private Boolean mIsMainWindowDisabled = false;
        private DateTime mEditWorkDayDate;
        private Int32 mEditingWorkTimePerDayMinutes = 450;
        private ObservableCollection<GraphPoint> mIdealBurnDown = new ObservableCollection<GraphPoint>();
        private ObservableCollection<GraphPoint> mActualBurnDown = new ObservableCollection<GraphPoint>();
        private PointCollection mBurnDownChartIdealPoints = new PointCollection();

        /// <summary>
        ///     Copy of the values of the selected item in the tree view for editing purposes.
        /// </summary>
        private ProjectTreeBranchItem mEditBoxSelectedTaskItem = null;
        #endregion Fields

        #region Construction
        /// <summary>
        ///     Default constructor
        /// </summary>
        public MainWindowViewModel()
        {
            SelectedTimeUnits = TimeMeasurementUnits.Hours;
            mAddEditTaskDialogErrorsCollection = new PropertyValidationErrorsCollection(AddEditTaskDialogErrorsCollectionChangedProc);
            mEditWorkDayDateDialogErrorsCollection = new PropertyValidationErrorsCollection(EditWorkDayDateDialogErrorsCollectionChangedProc);

            Utility.TaskItemChanged += OnTaskItemChanged;
            Utility.EventAggregator.GetEvent<ProjectModelChangedEvent>().Subscribe(OnProjectModelChanged);
            Utility.EventAggregator.GetEvent<ProjectWorkDayCreatedEvent>().Subscribe(OnWorkDayCreated);
        }
        #endregion Construction

        #region Public accessor methods
        public void OnCloseMainWindow()
        {
            ProjectModelProcessingStepBase steps = new DisableMainWindowStep(this, new CloseProjectModelStep(this.ProjectModel, new ExitProgramStep(this)));
            steps.Execute();
        }

        public String SelectedTimeUnitsLabel
        {
            get
            {
                switch (this.mSelectedTimeUnits)
                {
                    case TimeMeasurementUnits.Minutes:
                        return Resources.MeasurementMinutesLabel;

                    case TimeMeasurementUnits.Hours:
                        return Resources.MeasurementHoursLabel;

                    default:
                        return "?";
                }
            }
        }
        #endregion // Public accessor methods

        #region Public properties
        public PropertyValidationErrorsCollection AddEditTaskDialogErrorsCollection => this.mAddEditTaskDialogErrorsCollection;

        public PropertyValidationErrorsCollection EditWorkDayDateDialogErrorsCollection => this.mEditWorkDayDateDialogErrorsCollection;

        public ICommand AddWorkDayCommand => this.mAddWorkDayCommand ?? (mAddWorkDayCommand = new DelegateCommand(OnAddWorkDayCommand, IsAddWorkDayCommandEnabled));

        public ICommand EditWorkDayDateOkButtonCommand => this.mEditWorkDayDateOkButtonCommand ?? (mEditWorkDayDateOkButtonCommand = new DelegateCommand(OnEditWorkDayDateOkButtonCommand, IsEditWorkDayDateOkButtonCommandEnabled));

        public ICommand AddEditTaskDialogOkButtonCommand => this.mAddEditTaskDialogOkButtonCommand ?? (mAddEditTaskDialogOkButtonCommand = new DelegateCommand(OnAddEditTaskDialogOkButtonCommand, IsAddEditTaskDialogOkButtonCommandEnabled));

        public ICommand SaveCommand => this.mSaveCommand ?? (mSaveCommand = new DelegateCommand(OnSaveCommand, IsSaveCommandEnabled));

        public ICommand CloseCommand => this.mCloseCommand ?? (mCloseCommand = new DelegateCommand(OnCloseCommand, IsCloseCommandEnabled));

        public ICommand NewCommand => this.mNewCommand ?? (mNewCommand = new DelegateCommand(OnNewCommand));

        public ICommand LoadCommand => this.mLoadCommand ?? (mLoadCommand = new DelegateCommand(OnLoadCommand));

        public ICommand ProjectPropertiesCommand => this.mProjectPropertiesCommand ?? (this.mProjectPropertiesCommand = new DelegateCommand(OnProjectPropertiesCommand, IsProjectPropertiesCommandEnabled));

        public ICommand AddTaskCommand => this.mAddTaskCommand ?? (mAddTaskCommand = new DelegateCommand(OnAddTaskCommand, IsAddTaskCommandEnabled));

        public ICommand EditTaskCommand => this.mEditTaskCommand ?? (mEditTaskCommand = new DelegateCommand(OnEditTaskCommand, IsEditTaskCommandEnabled));

        public ICommand DeleteTaskCommand => this.mDeleteTaskCommand ?? (mDeleteTaskCommand = new DelegateCommand(OnDeleteTaskCommand, IsDeleteTaskCommandEnabled));

        public ICommand EditProjectPropertiesDialogOkButtonCommand => this.mEditProjectPropertiesDialogOkButtonCommand ?? (mEditProjectPropertiesDialogOkButtonCommand = new DelegateCommand(OnEditProjectPropertiesDialogOkButtonCommand));

        public Int32 ProjectDay => (this.ProjectModel != null) ? this.ProjectModel.CurrentWorkDayID : 0;

        public void OnNewDocument()
        {
            ProjectModelProcessingStepBase steps = new DisableMainWindowStep(this, new CloseProjectModelStep(this.ProjectModel, new NewProjectModelStep(this.ProjectModel)));
            steps.Execute();
        }

        /// <summary>
        ///     Gets the graph points for the ideal burn down 
        /// </summary>
        public ObservableCollection<GraphPoint> IdealBurnDown => this.mIdealBurnDown;

        /// <summary>
        ///     Gets the graph points for the actual burn down 
        /// </summary>
        public ObservableCollection<GraphPoint> ActualBurnDown => this.mActualBurnDown;

        public ObservableCollection<ProjectTreeItemBase> ProjectItemsTreeData
        {
            get
            {
                return this.mProjectItemsTreeData;
            }
            private set
            {
                SetProperty(ref this.mProjectItemsTreeData, value);
            }
        }

        public Boolean IsSelectedTaskItemEditable
        {
            get
            {
                // Must have a current working day to edit the effort.
                if (this.ProjectModel.CurrentWorkDayID < 1) return false;

                // A task item must be selected in the tree
                if (this.SelectedTaskItem == null) return false;

                // Only leaf items are editable
                return (this.SelectedTaskItem as ProjectTreeItemBase).IsLeaf;
            }
        }

        public Boolean IsTimeEstimatesEditingAvailable
        {
            get
            {
                if (this.mIsEditingExistingItem)
                {
                    if (this.SelectedTaskItem == null) return false;
                    return (this.SelectedTaskItem as ProjectTreeItemBase).IsLeaf;
                }
                return true;
            }
        }

        public ProjectTreeBranchItem EditBoxSelectedTaskItem
        {
            get
            {
                return this.mEditBoxSelectedTaskItem ?? (this.mEditBoxSelectedTaskItem = new ProjectTreeBranchItem());
            }
            set
            {
                if (value == null)
                {
                    this.mEditBoxSelectedTaskItem = null;
                }
                else
                {
                    if (this.mEditBoxSelectedTaskItem == null)
                    {
                        this.mEditBoxSelectedTaskItem = new ProjectTreeBranchItem();
                    }
                    this.mEditBoxSelectedTaskItem.ItemDescription = value.ItemDescription;
                    this.mEditBoxSelectedTaskItem.MinimumTimeMinutes = value.MinimumTimeMinutes;
                    this.mEditBoxSelectedTaskItem.MaximumTimeMinutes = value.MaximumTimeMinutes;
                    this.mEditBoxSelectedTaskItem.EstimatedTimeMinutes = value.EstimatedTimeMinutes;
                }
            }
        }

        public Boolean IsMainWindowDisabled
        {
            get
            {
                return this.mIsMainWindowDisabled;
            }
            set
            {
                SetProperty(ref this.mIsMainWindowDisabled, value);
            }
        }

        public TimeMeasurementUnits SelectedTimeUnits
        {
            get
            {
                return this.mSelectedTimeUnits;
            }
            set
            {
                if (SetProperty(ref this.mSelectedTimeUnits, value))
                {
                    OnPropertyChanged(nameof(SelectedTimeUnitsLabel));
                }
            }
        }

        public DateTime EditWorkDayDate
        {
            get
            {
                return this.mEditWorkDayDate;
            }
            set
            {
                SetProperty(ref this.mEditWorkDayDate, value);
            }
        }

        public DateTime MinimumNextWorkDayDate
        {
            get 
            {
                return (this.ProjectModel.CurrentWorkDayID < 1) ? DateTime.MinValue : this.ProjectModel.CurrentWorkDayDate; 
            }
        }

        public Boolean CanCloseMainWindow 
        {
            get
            {
                return this.mCanCloseMainWindow;
            }
            set
            {
                this.mCanCloseMainWindow = value;
            }
        }

        public Object SelectedTaskItem
        {
            get
            {
                return this.mSelectedTaskItem;
            }
            set
            {
                if (SetProperty<ProjectTreeItemBase>(ref this.mSelectedTaskItem, value as ProjectTreeItemBase))
                {
                    (AddTaskCommand as DelegateCommand).RaiseCanExecuteChanged();
                    (EditTaskCommand as DelegateCommand).RaiseCanExecuteChanged();
                    (DeleteTaskCommand as DelegateCommand).RaiseCanExecuteChanged();
                    OnPropertyChanged(nameof(IsSelectedTaskItemEditable));
                    OnPropertyChanged(nameof(IsTimeEstimatesEditingAvailable));
                }
            }
        }

        /// <summary>
        ///     Copy of the number of minutes per working day for editing.
        /// </summary>
        public Int32 EditingWorkTimePerDayMinutes
        {
            get { return this.mEditingWorkTimePerDayMinutes; }
            set 
            {
                if (SetProperty(ref this.mEditingWorkTimePerDayMinutes, value))
                {
                    // TODO: Update burn down chart calculations
                }
            }
        }

        public PointCollection BurnDownChartIdealPoints
        {
            get { return this.mBurnDownChartIdealPoints; }
            private set
            {
                this.mBurnDownChartIdealPoints = value;
                OnPropertyChanged();
            }
        }

        #endregion Public properties


        #region Protected properties
        /// <summary>
        ///     Gets the model attached to the view model.
        /// </summary>
        protected ProjectEstimationTool.Model.ProjectModel ProjectModel => this.mProjectModel;
        #endregion Protected properties

        #region Private helper methods
        /// <summary>
        ///     Event handler for the <see cref="ProjectModelChangedEvent"/> event.
        /// </summary>
        private void OnProjectModelChanged(ProjectModelState projectState)
        {
            SelectedTaskItem = null;

            if (projectState == ProjectModelState.NoProject)
            {
                this.ProjectItemsTreeData.Clear();
                this.ActualBurnDown.Clear();
                this.IdealBurnDown.Clear();
            }
            else if (projectState == ProjectModelState.Open)
            {
                ProjectModelTreeBranchItem projectRoot = this.mProjectModel.ProjectTaskItemsRoot;
                if (projectRoot != null)
                {
                    ObservableCollection<ProjectTreeItemBase> replacementList = new ObservableCollection<ProjectTreeItemBase>();
                    ProjectTreeBranchItem rootItem = projectRoot.Clone() as ProjectTreeBranchItem;
                    replacementList.Add(rootItem);
                    this.ProjectItemsTreeData = replacementList;
                }
                else
                {
                    this.ProjectItemsTreeData.Clear();
                }

                RecalculateProjectDuration();
            }

            (this.AddWorkDayCommand as DelegateCommand).RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(IsSelectedTaskItemEditable));
            OnPropertyChanged(nameof(IsTimeEstimatesEditingAvailable));
            OnPropertyChanged(nameof(ProjectDay));

            (this.SaveCommand as DelegateCommand).RaiseCanExecuteChanged();
            (this.CloseCommand as DelegateCommand).RaiseCanExecuteChanged();
        }

        private void OnWorkDayCreated(Int32 workDayID)
        {
            OnPropertyChanged(nameof(IsSelectedTaskItemEditable));
            OnPropertyChanged(nameof(IsTimeEstimatesEditingAvailable));
            OnPropertyChanged(nameof(ProjectDay));
        }

        private void OnTaskItemChanged(Object sender, String propertyName)
        {
            this.ProjectModel.TaskItemChanged(sender as ProjectTreeItemBase);
            UpdateParentsOfTask(sender as ProjectTreeItemBase, propertyName);
            if (String.Compare(propertyName, nameof(ProjectTreeItemBase.EstimatedTimeMinutes), StringComparison.Ordinal) == 0)
            {
                RecalculateProjectDuration();
            }
        }

        private void UpdateParentsOfTask(ProjectTreeItemBase changedTask, String propertyName)
        {
            if (this.mProjectItemsTreeData != null)
            {
                foreach (ProjectTreeItemBase node in this.mProjectItemsTreeData)
                {
                    if (Object.ReferenceEquals(node, changedTask))
                    {
                        return;
                    }

                    if (UpdateParentsOfTask(node, changedTask, propertyName))
                    {
                        node.OnPropertyChanged(propertyName);
                        return;
                    }
                }
            }
        }

        private Boolean UpdateParentsOfTask(ProjectTreeItemBase parentNode, ProjectTreeItemBase changedTask, String propertyName)
        {
            if (parentNode.Children != null)
            {
                foreach (ProjectTreeItemBase node in parentNode.Children)
                {
                    if (Object.ReferenceEquals(node, changedTask))
                    {
                        return true;
                    }

                    if (UpdateParentsOfTask(node, changedTask, propertyName))
                    {
                        node.OnPropertyChanged(propertyName);
                        return true;
                    }
                }
            }

            return false;
        }

        private void AddEditTaskDialogErrorsCollectionChangedProc()
        {
            (AddEditTaskDialogOkButtonCommand as DelegateCommand).RaiseCanExecuteChanged();
        }

        private void EditWorkDayDateDialogErrorsCollectionChangedProc()
        {
            (EditWorkDayDateOkButtonCommand as DelegateCommand).RaiseCanExecuteChanged();
        }

        private void OnLoadCommand()
        {
            this.OnOpenDocument();
        }

        private void OnNewCommand()
        {
            this.OnNewDocument();
        }

        private void OnSaveCommand()
        {
            this.OnSaveDocument();
        }
        
        private Boolean IsSaveCommandEnabled()
        {
            return (this.ProjectModel.ModelChanged == true);
        }

        private void OnCloseCommand()
        {
            this.OnCloseDocument();
        }
        
        private Boolean IsCloseCommandEnabled()
        {
            return (this.ProjectModel.IsProjectModelActive == true);
        }

        private void OnProjectPropertiesCommand()
        {
            this.SetProjectProperties();
        }

        private Boolean IsProjectPropertiesCommandEnabled()
        {
            return (this.ProjectModel.IsProjectModelActive == true);
        }

        private void OnEditProjectPropertiesDialogOkButtonCommand()
        {
            ProjectModel.WorkDayLengthMinutes = EditingWorkTimePerDayMinutes;
            RecalculateProjectDuration();
        }

        private void OnAddTaskCommand()
        {
            mIsEditingExistingItem = false;
            this.EditBoxSelectedTaskItem.ItemDescription = Resources.DefaultNewTaksDescription;
            this.EditBoxSelectedTaskItem.MaximumTimeMinutes = 0.0;
            this.EditBoxSelectedTaskItem.MinimumTimeMinutes = 0.0;
            this.EditBoxSelectedTaskItem.EstimatedTimeMinutes = 0.0;
            this.EditBoxSelectedTaskItem.ParentProjectItemID = (SelectedTaskItem as ProjectTreeItemBase).ProjectItemID;
            this.EditBoxSelectedTaskItem.ProjectItemID = -1;
            Utility.EventAggregator.GetEvent<ShowAddItemEvent>().Publish();
        }

        private Boolean IsAddTaskCommandEnabled()
        {
            if (SelectedTaskItem == null) return false;
            return ((SelectedTaskItem as ProjectTreeItemBase).TreeLevel < 5);
        }

        private void OnEditTaskCommand()
        {
            mIsEditingExistingItem = true;
            ProjectTreeBranchItem src = SelectedTaskItem as ProjectTreeBranchItem;
            EditBoxSelectedTaskItem.ProjectItemID = src.ProjectItemID;
            EditBoxSelectedTaskItem.ParentProjectItemID = src.ParentProjectItemID;
            EditBoxSelectedTaskItem.ItemDescription = src.ItemDescription;
            EditBoxSelectedTaskItem.MaximumTimeMinutes = src.MaximumTimeMinutes;
            EditBoxSelectedTaskItem.MinimumTimeMinutes = src.MinimumTimeMinutes;
            EditBoxSelectedTaskItem.EstimatedTimeMinutes = src.EstimatedTimeMinutes;
            EditBoxSelectedTaskItem.TimeSpentMinutes = src.TimeSpentMinutes;
            EditBoxSelectedTaskItem.PercentageComplete = src.PercentageComplete;
            Utility.EventAggregator.GetEvent<ShowEditItemEvent>().Publish();
        }
        
        private Boolean IsEditTaskCommandEnabled()
        {
            return (this.SelectedTaskItem != null);
        }

        private void OnDeleteTaskCommand()
        {
            this.ProjectModel.DeleteTaskItem((SelectedTaskItem as ProjectTreeItemBase).ProjectItemID);
            if (this.mProjectItemsTreeData != null)
            {
                for (Int32 childIndex = 0; childIndex < this.mProjectItemsTreeData.Count; ++childIndex)
                {
                    ProjectTreeItemBase childItem = this.mProjectItemsTreeData[childIndex];
                    if (RemoveFromTree((SelectedTaskItem as ProjectTreeItemBase).ProjectItemID, this.mProjectItemsTreeData, childIndex))
                        break;
                }
            }
            SelectedTaskItem = null;
        }
        
        private Boolean RemoveFromTree(Int32 itemID, IList<ProjectTreeItemBase> branch, Int32 index)
        {
            if (branch[index].ProjectItemID == itemID)
            {
                branch.RemoveAt(index);
                return true;
            }

            if (branch[index].Children != null)
            {
                for (Int32 childIndex = 0; childIndex < branch[index].Children.Count; ++childIndex)
                {
                    ProjectTreeItemBase childItem = branch[index].Children[childIndex];
                    if (RemoveFromTree((SelectedTaskItem as ProjectTreeItemBase).ProjectItemID, branch[index].Children, childIndex))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private Boolean IsDeleteTaskCommandEnabled()
        {
            // Can only delete leaf items
            ProjectTreeItemBase selectedItem = SelectedTaskItem as ProjectTreeItemBase;
            if (selectedItem == null) return false;

            return (selectedItem.IsLeaf && (selectedItem.TreeLevel > 0));
        }

        private Boolean IsAddEditTaskDialogOkButtonCommandEnabled()
        {
            if (SelectedTaskItem == null) return false;
            if (!(SelectedTaskItem as ProjectTreeItemBase).IsLeaf)
            {
                this.mAddEditTaskDialogErrorsCollection.ClearError(nameof(ProjectTreeItemBase.MinimumTimeMinutes));
                this.mAddEditTaskDialogErrorsCollection.ClearError(nameof(ProjectTreeItemBase.MaximumTimeMinutes));
                this.mAddEditTaskDialogErrorsCollection.ClearError(nameof(ProjectTreeItemBase.EstimatedTimeMinutes));
            }
            return !this.mAddEditTaskDialogErrorsCollection.HasErrors;
        }

        private void OnAddWorkDayCommand()
        {
            EditWorkDayDate = this.ProjectModel.CurrentWorkDayDate;
            Utility.EventAggregator.GetEvent<ShowWorkDayDialogEvent>().Publish();
        }

        private Boolean IsAddWorkDayCommandEnabled()
        {
            return this.ProjectModel.IsProjectModelActive;
        }

        /// <summary>
        ///     Called when the user clicks the OK button on the dialog box where a work day is added.
        /// </summary>
        private void OnEditWorkDayDateOkButtonCommand()
        {
            this.ProjectModel.AddWorkDay(EditWorkDayDate);
        }

        private Boolean IsEditWorkDayDateOkButtonCommandEnabled()
        {
            return this.EditWorkDayDateDialogErrorsCollection.HasErrors ? false : true;
        }

        /// <summary>
        ///     Called when the user clicks the OK button on the Add/Edit task dialog box.
        /// </summary>
        private void OnAddEditTaskDialogOkButtonCommand()
        {
            // The ProjectItemID for new items being added is set to -1
            if (mIsEditingExistingItem) // Editing an item
            {
                ProjectTreeItemBase selectedItem = SelectedTaskItem as ProjectTreeItemBase;
                if (selectedItem != null)
                {
                    selectedItem.ItemDescription = EditBoxSelectedTaskItem.ItemDescription;
                    selectedItem.MinimumTimeMinutes = EditBoxSelectedTaskItem.MinimumTimeMinutes;
                    selectedItem.MaximumTimeMinutes = EditBoxSelectedTaskItem.MaximumTimeMinutes;
                    selectedItem.EstimatedTimeMinutes = EditBoxSelectedTaskItem.EstimatedTimeMinutes;

                    UpdateParentsOfTask(selectedItem, nameof(selectedItem.MinimumTimeMinutes));
                    UpdateParentsOfTask(selectedItem, nameof(selectedItem.MaximumTimeMinutes));
                    UpdateParentsOfTask(selectedItem, nameof(selectedItem.EstimatedTimeMinutes));

                    this.ProjectModel.TaskItemChanged(selectedItem);
                }
            }
            else // Adding an item
            {
                ProjectTreeItemBase selectedItem = SelectedTaskItem as ProjectTreeItemBase;

                ProjectTreeBranchItem newChild = new ProjectTreeBranchItem()
                {
                    ProjectItemID = ProjectTreeBranchItem.GetNextProjectItemID(),
                    ItemDescription = EditBoxSelectedTaskItem.ItemDescription,
                    MinimumTimeMinutes = EditBoxSelectedTaskItem.MinimumTimeMinutes,
                    MaximumTimeMinutes = EditBoxSelectedTaskItem.MaximumTimeMinutes,
                    EstimatedTimeMinutes = EditBoxSelectedTaskItem.EstimatedTimeMinutes,
                    TimeSpentMinutes = 0.0,
                    PercentageComplete = 0,
                    TreeLevel = selectedItem.TreeLevel + 1,
                    ParentProjectItemID = selectedItem.ProjectItemID


                };
                this.mProjectModel.AddTaskItem(selectedItem.ProjectItemID, newChild);

                if (selectedItem.Children == null)
                {
                    selectedItem.Children = new ObservableCollection<ProjectTreeItemBase>();
                }
                selectedItem.Children.Add(newChild);

                UpdateParentsOfTask(newChild, nameof(newChild.MinimumTimeMinutes));
                UpdateParentsOfTask(newChild, nameof(newChild.MaximumTimeMinutes));
                UpdateParentsOfTask(newChild, nameof(newChild.EstimatedTimeMinutes));
                UpdateParentsOfTask(newChild, nameof(newChild.TimeSpentMinutes));
                UpdateParentsOfTask(newChild, nameof(newChild.PercentageComplete));
            }
        }

        private void OnOpenDocument()
        {
            ProjectModelProcessingStepBase steps = new DisableMainWindowStep(this, new CloseProjectModelStep(this.ProjectModel, new LoadProjectModelStep(this.ProjectModel)));
            steps.Execute();
        }


        private void OnCloseDocument()
        {
            ProjectModelProcessingStepBase steps = new DisableMainWindowStep(this, new CloseProjectModelStep(this.ProjectModel));
            steps.Execute();
        }

        private void OnSaveDocument()
        {
            ProjectModelProcessingStepBase steps = new DisableMainWindowStep(this, new SaveProjectModelStep(this.ProjectModel));
            steps.Execute();
        }

        private void SetProjectProperties()
        {
            
            Utility.EventAggregator.GetEvent<ShowEditProjectPropertiesEvent>().Publish();
        }

        /// <summary>
        /// Calculates the project duration based on the number of hours per work day and the estimated task duration of each item.
        /// </summary>
        private void RecalculateProjectDuration()
        {
            this.ActualBurnDown.Clear();
            this.IdealBurnDown.Clear();
            this.BurnDownChartIdealPoints = new PointCollection();

            if (this.ProjectItemsTreeData.Count < 1) 
            {
                return;
            }

            List<GraphPoint> idealBurnDownPoints = new List<GraphPoint>();
            Int32 estimatedWorkDays = (Convert.ToInt32(this.ProjectItemsTreeData[0].EstimatedTimeMinutes) + (this.ProjectModel.WorkDayLengthMinutes - 1)) / this.ProjectModel.WorkDayLengthMinutes;
            Double idealEffortLeft = 0.0;
            Double idealDailyEffort = 100.0 / (Convert.ToDouble(estimatedWorkDays));
            for (Int32 dayNumber = 0; dayNumber < estimatedWorkDays; ++dayNumber)
            {
                idealBurnDownPoints.Add(new GraphPoint(dayNumber, Convert.ToInt32(idealEffortLeft)));
                idealEffortLeft += idealDailyEffort;
            }
            //idealBurnDownPoints.Add(new GraphPoint(estimatedWorkDays, 0));
            IdealBurnDown.Clear();
            IdealBurnDown.AddRange(idealBurnDownPoints);

            PointCollection graphPoints = new PointCollection(estimatedWorkDays);
            foreach (GraphPoint graphPoint in IdealBurnDown)
            {
                graphPoints.Add(new Point(graphPoint.X, graphPoint.Y));
            }
            BurnDownChartIdealPoints = graphPoints;
        }
        #endregion Private helper methods
    } // class MainWindowViewModel
} // namespace ProjectEstimationTool.ViewModels
