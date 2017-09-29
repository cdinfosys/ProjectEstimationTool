using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ProjectEstimationTool.Classes;
using ProjectEstimationTool.Events;
using ProjectEstimationTool.Interfaces;
using ProjectEstimationTool.Properties;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel, IDataErrorInfo
    {
        private List<ProjectTreeItemBase> mProjectItemsTreeData = new List<ProjectTreeItemBase>();
        private ProjectTreeItemBase mSelectedTaskItem = null;
        private TimeMeasurementUnits mSelectedTimeUnits = TimeMeasurementUnits.Unknown;
        private ProjectEstimationTool.Model.ProjectModel mProjectModel = new ProjectEstimationTool.Model.ProjectModel();

        /// <summary>
        ///     Default constructor
        /// </summary>
        public MainWindowViewModel()
        {
            SelectedTimeUnits = TimeMeasurementUnits.Minutes;

            Utilities.Utility.TaskItemChanged += OnTaskItemChanged;
            Utilities.Utility.EventAggregator.GetEvent<ProjectModelChangedEvent>().Subscribe(OnProjectModelChanged);
        }

        public void OnOpenDocument()
        {
        }

        public void OnCloseDocument()
        {
            ProjectModelProcessingStepBase steps = new CloseProjectModelStep(this.ProjectModel);
            steps.Execute();
        }

        public void OnNewDocument()
        {
            ProjectModelProcessingStepBase steps = new CloseProjectModelStep(this.ProjectModel, new NewProjectModelStep(this.ProjectModel));
            steps.Execute();
        }

        public void OnSaveDocument()
        {
            ProjectModelProcessingStepBase steps = new SaveProjectModelStep(this.ProjectModel);
            steps.Execute();
        }

        public void OnSaveDocumentAs()
        {
            ProjectModelProcessingStepBase steps = new SaveProjectModelAsStep(this.ProjectModel);
            steps.Execute();
        }

        /// <summary>
        ///     Event handler for the <see cref="ProjectModelChangedEvent"/> event.
        /// </summary>
        private void OnProjectModelChanged()
        {
            ProjectModelTreeBranchItem projectRoot = this.mProjectModel.ProjectTaskItemsRoot;
            if (projectRoot != null)
            {
                List<ProjectTreeItemBase> replacementList = new List<ProjectTreeItemBase>();
                ProjectTreeBranchItem rootItem = projectRoot.Clone() as ProjectTreeBranchItem;
                replacementList.Add(rootItem);
                this.ProjectItemsTreeData = replacementList;
            }
            else
            {
                this.ProjectItemsTreeData.Clear();
            }
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

        public List<ProjectTreeItemBase> ProjectItemsTreeData
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
                return (this.SelectedTaskItem == null) ? false : (SelectedTaskItem as ProjectTreeItemBase).IsLeaf;
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
                    OnPropertyChanged(nameof(IsSelectedTaskItemEditable));
                }
            }
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

        public String Error
        {
            get
            {
                return String.Empty;
            }
        }

        public String this[String columnName]
        {
            get
            {
                return String.Empty;
            }
        }

        #region Protected properties
        protected ProjectEstimationTool.Model.ProjectModel ProjectModel => this.mProjectModel;
        #endregion Protected properties
    } // class MainWindowViewModel
} // namespace ProjectEstimationTool.ViewModels
