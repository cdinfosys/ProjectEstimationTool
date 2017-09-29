using System;
using System.Collections.Generic;
using ProjectEstimationTool.Classes;
using ProjectEstimationTool.Events;
using ProjectEstimationTool.Interfaces;
using ProjectEstimationTool.Properties;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool.Model
{
    public class ProjectModel
    {
        #region Private data members
        /// <summary>
        ///     Flag to indicate that the data in the model was modified.
        /// </summary>
        private Boolean mModelChanged = false;

        /// <summary>
        ///     Object for communicating with the data store.
        /// </summary>
        private IDataAccess mDataAccess = null;

        /// <summary>
        ///     Root item of the project tree.
        /// </summary>
        private ProjectModelTreeBranchItem mProjectItemsRoot = new ProjectModelTreeBranchItem();

        #endregion // Private data members


        #region Construction
        public ProjectModel()
        {
            // Setup a new, empty model.
            NewModel();
        }
        #endregion // Construction

        #region Public properties
        public Boolean ModelChanged
        {
            get
            {
                return this.mModelChanged;
            }
            set
            {
                this.mModelChanged = value;
            }
        }

        /// <summary>
        ///     Gets the root item for the project.
        /// </summary>
        public ProjectModelTreeBranchItem ProjectTaskItemsRoot => this.mProjectItemsRoot;

        /// <summary>
        ///     Gets a flag 
        /// </summary>
        public Boolean ProjectPathSet
        {
            get
            {
                return this.mDataAccess != null;
            }
        }
        #endregion Public properties

        #region Public accessor methods
        /// <summary>
        ///     Discard the current model.
        /// </summary>
        public void DiscardModel()
        {
            if (this.mDataAccess != null)
            {
                this.mDataAccess.Dispose();
                this.mDataAccess = null;
            }
            mProjectItemsRoot = null;
            ModelChanged = false;
        }

        public void CloseModel()
        {
            DiscardModel();
            Utility.EventAggregator.GetEvent<ProjectModelChangedEvent>().Publish();
        }

        public void ForAllTreeItems(Action<ProjectTreeItemBase> action)
        {
            ForAllTreeItems(this.mProjectItemsRoot, action);
        }

        protected void ForAllTreeItems(ProjectTreeItemBase childItem, Action<ProjectTreeItemBase> action)
        {
            if (childItem != null)
            {
                action(childItem);

                if (childItem.Children != null)
                {
                    foreach (ProjectTreeItemBase child in childItem.Children)
                    {
                        ForAllTreeItems(child, action);
                    }
                }
            }
        }

        public void NewModel()
        {
            mProjectItemsRoot = new ProjectModelTreeBranchItem()
            {
                ProjectItemID = 1,
                ItemDescription = Resources.DefaultRootTaksDescription
            };

            mProjectItemsRoot =  new ProjectModelTreeBranchItem()
            {
                ProjectItemID = 1,
                ItemDescription = "Root!!",
                Children = new List<ProjectTreeItemBase>()
                {
                    new ProjectModelTreeBranchItem()
                    {
                        ProjectItemID = 2,
                        ItemDescription = "Root",
                        Children = new List<ProjectTreeItemBase>()
                        {
                            new ProjectModelTreeBranchItem()
                            {
                                ProjectItemID = 3,
                                ItemDescription = "Root",
                                Children = new List<ProjectTreeItemBase>()
                                {
                                    new ProjectModelTreeBranchItem()
                                    {
                                        ProjectItemID = 4,
                                        ItemDescription = "Root",
                                        Children = new List<ProjectTreeItemBase>()
                                        {
                                            new ProjectModelTreeBranchItem()
                                            {
                                                ProjectItemID = 5,
                                                ItemDescription = "Child 1",
                                                EstimatedTimeMinutes = 60,
                                                MinimumTimeMinutes = 30,
                                                MaximumTimeMinutes = 80,
                                                PercentageComplete = 50,
                                                TimeSpentMinutes = 40
                                            },

                                            new ProjectModelTreeBranchItem()
                                            {
                                                ProjectItemID = 6,
                                                ItemDescription = "Child 2",
                                                EstimatedTimeMinutes = 90,
                                                MinimumTimeMinutes = 60,
                                                MaximumTimeMinutes = 120,
                                                PercentageComplete = 20,
                                                TimeSpentMinutes = 60
                                            },
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            ForAllTreeItems(u => (u as ProjectModelTreeBranchItem).TrackingFlags = ProjectModelTreeBranchItem.ChangeTrackingFlags.Unchanged);

            ModelChanged = false;
            Utility.EventAggregator.GetEvent<ProjectModelChangedEvent>().Publish();
        }

        public void SaveData()
        {
            this.ModelChanged = false;
        }

        public void SaveDataAs(String pathToOutputFile)
        {
            IDataAccess dataAccess = this.DataAccessObject;
            if (dataAccess is SQLiteDataAccess)
            {
                (dataAccess as SQLiteDataAccess).DatabaseFilePath = pathToOutputFile;
            }
            dataAccess.CreateNew();
        }

        public void LoadData(String pathToOutputFile)
        {
            Utility.EventAggregator.GetEvent<ProjectModelChangedEvent>().Publish();
        }

        public void TaskItemChanged(ProjectTreeItemBase changedItem)
        {
            if (this.mProjectItemsRoot != null)
            {
                if (this.mProjectItemsRoot.ProjectItemID == changedItem.ProjectItemID)
                {
                    this.mProjectItemsRoot.UpdateFrom(changedItem);
                    this.ModelChanged = true;
                    return;
                }

                if (this.mProjectItemsRoot.Children != null)
                {
                    foreach (ProjectTreeItemBase child in this.mProjectItemsRoot.Children)
                    {
                        if (UpdateTaskItem(child, changedItem))
                        {
                            this.ModelChanged = true;
                            return;
                        }
                    }
                }
            }
        }

        #endregion // Public accessor methods

        #region Protected properties
        protected IDataAccess DataAccessObject
        {
            get
            {
                return this.mDataAccess ?? (mDataAccess = new SQLiteDataAccess());
            }
        }
        #endregion Protected properties

        #region Private helper methods
        private Boolean UpdateTaskItem(ProjectTreeItemBase projectModelItem, ProjectTreeItemBase updatedItem)
        {
            if (projectModelItem.ProjectItemID == updatedItem.ProjectItemID)
            {
                projectModelItem.UpdateFrom(updatedItem);
                return true;
            }

            if (projectModelItem.Children != null)
            {
                foreach (ProjectTreeItemBase child in projectModelItem.Children)
                {
                    if (UpdateTaskItem(child, updatedItem))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion // Private helper methods
    } // class ProjectModel
} // namespace ProjectEstimationTool.Model
