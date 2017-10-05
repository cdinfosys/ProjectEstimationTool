using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
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
        private ICommand mAddEditTaskDialogOkButtonCommand;
        private ICommand mAddTaskCommand;
        private ICommand mEditTaskCommand;
        private ICommand mDeleteTaskCommand;
        private ICommand mAddWorkDay;
        private Boolean mIsEditingExistingItem;
        private Boolean mIsMainWindowDisabled = false;

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

            Utilities.Utility.TaskItemChanged += OnTaskItemChanged;
            Utilities.Utility.EventAggregator.GetEvent<ProjectModelChangedEvent>().Subscribe(OnProjectModelChanged);
        }
        #endregion Construction

        #region Public accessor methods
        public void OnOpenDocument()
        {
            ProjectModelProcessingStepBase steps = new DisableMainWindowStep(this, new CloseProjectModelStep(this.ProjectModel, new LoadProjectModelStep(this.ProjectModel)));
            steps.Execute();
        }

        public void OnCloseDocument()
        {
            ProjectModelProcessingStepBase steps = new DisableMainWindowStep(this, new CloseProjectModelStep(this.ProjectModel));
            steps.Execute();
        }

        public void OnNewDocument()
        {
            ProjectModelProcessingStepBase steps = new DisableMainWindowStep(this, new CloseProjectModelStep(this.ProjectModel, new NewProjectModelStep(this.ProjectModel)));
            steps.Execute();
        }

        public void OnSaveDocument()
        {
            ProjectModelProcessingStepBase steps = new DisableMainWindowStep(this, new SaveProjectModelStep(this.ProjectModel));
            steps.Execute();
        }

        public void OnSaveDocumentAs()
        {
            ProjectModelProcessingStepBase steps = new DisableMainWindowStep(this, new SaveProjectModelAsStep(this.ProjectModel));
            steps.Execute();
        }

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

        public ICommand AddWorkDayCommand => this.mAddWorkDay ?? (mAddWorkDay = new DelegateCommand(OnAddWorkDayCommand, IsAddWorkDayCommandEnabled));

        public ICommand AddEditTaskDialogOkButtonCommand => this.mAddEditTaskDialogOkButtonCommand ?? (mAddEditTaskDialogOkButtonCommand = new DelegateCommand(OnAddEditTaskDialogOkButtonCommand, IsAddEditTaskDialogOkButtonCommandEnabled));

        public ICommand AddTaskCommand => this.mAddTaskCommand ?? (mAddTaskCommand = new DelegateCommand(OnAddTaskCommand, IsAddTaskCommandEnabled));

        public ICommand EditTaskCommand => this.mEditTaskCommand ?? (mEditTaskCommand = new DelegateCommand(OnEditTaskCommand, IsEditTaskCommandEnabled));

        public ICommand DeleteTaskCommand => this.mDeleteTaskCommand ?? (mDeleteTaskCommand = new DelegateCommand(OnDeleteTaskCommand, IsDeleteTaskCommandEnabled));

        public Int32 ProjectDay => (this.ProjectModel != null) ? this.ProjectModel.CurrentWorkDayID : 0;

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
                    EditBoxSelectedTaskItem = value as ProjectTreeBranchItem;
                    (AddTaskCommand as DelegateCommand).RaiseCanExecuteChanged();
                    (EditTaskCommand as DelegateCommand).RaiseCanExecuteChanged();
                    (DeleteTaskCommand as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }
        #endregion Public properties


        #region Protected properties
        protected ProjectEstimationTool.Model.ProjectModel ProjectModel => this.mProjectModel;
        #endregion Protected properties

        #region Private helper methods
        /// <summary>
        ///     Event handler for the <see cref="ProjectModelChangedEvent"/> event.
        /// </summary>
        private void OnProjectModelChanged()
        {
            SelectedTaskItem = null;
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

            (this.AddWorkDayCommand as DelegateCommand).RaiseCanExecuteChanged();
        }

        private void OnTaskItemChanged(Object sender, String propertyName)
        {
            this.ProjectModel.TaskItemChanged(sender as ProjectTreeItemBase);
            UpdateParentsOfTask(sender as ProjectTreeItemBase, propertyName);
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
            
        }
        
        private Boolean IsAddWorkDayCommandEnabled()
        {
            return this.ProjectModel.IsProjectModelActive;
        }

        private void OnAddEditTaskDialogOkButtonCommand()
        {
            // The ProjectItemID for new items being added is set to -1
            if (EditBoxSelectedTaskItem.ProjectItemID >= 0) // Editing an item
            {
                ProjectTreeItemBase selectedItem = SelectedTaskItem as ProjectTreeItemBase;
                if (selectedItem != null)
                {
                    selectedItem.ItemDescription = EditBoxSelectedTaskItem.ItemDescription;
                    selectedItem.MinimumTimeMinutes = EditBoxSelectedTaskItem.MinimumTimeMinutes;
                    selectedItem.MaximumTimeMinutes = EditBoxSelectedTaskItem.MaximumTimeMinutes;
                    selectedItem.EstimatedTimeMinutes = EditBoxSelectedTaskItem.EstimatedTimeMinutes;

                    this.ProjectModel.TaskItemChanged(selectedItem);
                }
            }
            else
            {
                ProjectTreeItemBase selectedItem = SelectedTaskItem as ProjectTreeItemBase;

                // Adding an item
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
                SelectedTaskItem = newChild;
            }
        }
        #endregion Private helper methods
    } // class MainWindowViewModel
} // namespace ProjectEstimationTool.ViewModels
