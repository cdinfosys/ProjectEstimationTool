using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NLog;
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
        ///     Backing data member for the <see cref="PathToModelFile"/> property
        /// </summary>
        private String mPathToModelFile;

        /// <summary>
        ///     Object for communicating with the data store.
        /// </summary>
        private IDataAccess mDataAccess = null;

        /// <summary>
        ///     Root item of the project tree.
        /// </summary>
        private ProjectModelTreeBranchItem mProjectItemsRoot = new ProjectModelTreeBranchItem();

        #endregion // Private data members

        /// <summary>
        ///     Permutations of Added/Modified/Deleted flags for determining the type of action to perform when
        ///     writing a task item to the store.
        /// </summary>
        private static Tuple<ProjectModelTreeBranchItem.ChangeTrackingFlags, ProjectModelTreeBranchItem.ChangeTrackingFlags>[] SaveActionTable = 
        {
            // A | M | D
            // 0 | 0 | 0
            new Tuple<ProjectModelTreeBranchItem.ChangeTrackingFlags, ProjectModelTreeBranchItem.ChangeTrackingFlags>
            ( 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Unchanged, 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Unchanged
            ), 

            // 0 | 0 | 1
            new Tuple<ProjectModelTreeBranchItem.ChangeTrackingFlags, ProjectModelTreeBranchItem.ChangeTrackingFlags>
            ( 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Deleted, 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Deleted
            ), 

            // 0 | 1 | 0
            new Tuple<ProjectModelTreeBranchItem.ChangeTrackingFlags, ProjectModelTreeBranchItem.ChangeTrackingFlags>
            ( 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Modified, 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Modified
            ), 

            // 0 | 1 | 1
            new Tuple<ProjectModelTreeBranchItem.ChangeTrackingFlags, ProjectModelTreeBranchItem.ChangeTrackingFlags>
            ( 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Modified | ProjectModelTreeBranchItem.ChangeTrackingFlags.Deleted, 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Deleted
            ), 

            // 1 | 0 | 0
            new Tuple<ProjectModelTreeBranchItem.ChangeTrackingFlags, ProjectModelTreeBranchItem.ChangeTrackingFlags>
            ( 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Added, 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Added
            ), 

            // 1 | 0 | 1
            new Tuple<ProjectModelTreeBranchItem.ChangeTrackingFlags, ProjectModelTreeBranchItem.ChangeTrackingFlags>
            ( 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Added | ProjectModelTreeBranchItem.ChangeTrackingFlags.Deleted, 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Unchanged
            ), 

            // 1 | 1 | 0
            new Tuple<ProjectModelTreeBranchItem.ChangeTrackingFlags, ProjectModelTreeBranchItem.ChangeTrackingFlags>
            ( 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Added | ProjectModelTreeBranchItem.ChangeTrackingFlags.Modified, 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Added
            ), 

            // 1 | 1 | 1
            new Tuple<ProjectModelTreeBranchItem.ChangeTrackingFlags, ProjectModelTreeBranchItem.ChangeTrackingFlags>
            ( 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Added | ProjectModelTreeBranchItem.ChangeTrackingFlags.Modified | ProjectModelTreeBranchItem.ChangeTrackingFlags.Deleted, 
                ProjectModelTreeBranchItem.ChangeTrackingFlags.Deleted
            ), 
        };

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
        public ProjectModelTreeBranchItem ProjectTaskItemsRoot 
        {
            get { return this.mProjectItemsRoot; }
            private set { this.mProjectItemsRoot = value; }
        }

        /// <summary>
        ///     Gets or sets the path to the model database file.
        /// </summary>
        public String PathToModelFile
        {
            get { return this.mPathToModelFile; }
            set { this.mPathToModelFile = value; }
        }

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
            ProjectTreeItemBase.SetHighestProjectItemID(1);

            /*
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
            */
            ForAllTreeItems(u => (u as ProjectModelTreeBranchItem).TrackingFlags = ProjectModelTreeBranchItem.ChangeTrackingFlags.Added);

            ModelChanged = false;
            Utility.EventAggregator.GetEvent<ProjectModelChangedEvent>().Publish();
        }

        public void LoadData(String filePath)
        {
            this.PathToModelFile = filePath;

            IDataAccess dataAccess = this.DataAccessObject;
            PathToModelFile = filePath;
            if (dataAccess is SQLiteDataAccess)
            {
                (dataAccess as SQLiteDataAccess).DatabaseFilePath = filePath;
            }
            dataAccess.Open();
            ProjectTreeItemBase.SetHighestProjectItemID(dataAccess.GetHighestTaskItemID());
            ExpandIntoTree(dataAccess.GetTaskItems(true));
            ForAllTreeItems(u => (u as ProjectModelTreeBranchItem).TrackingFlags = ProjectModelTreeBranchItem.ChangeTrackingFlags.Unchanged);
            this.ModelChanged = false;

            Utility.EventAggregator.GetEvent<ProjectModelChangedEvent>().Publish();
        }

        public void SaveData()
        {
            List<ProjectModelTreeBranchItem> updateList = OrganiseTaskItemsForStoring();
            if (updateList.Count > 0)
            {
                IDataAccess dataAccess = this.DataAccessObject;
                dataAccess.CreateProjectVersion();

                foreach (ProjectModelTreeBranchItem taskItem in updateList)
                {
                    WriteTaskItem(taskItem);
                }

                dataAccess.SetLastUpdateTime();
            }
            this.ModelChanged = false;
        }

        public void SaveDataAs(String pathToOutputFile)
        {
            IDataAccess dataAccess = this.DataAccessObject;
            PathToModelFile = pathToOutputFile;
            if (dataAccess is SQLiteDataAccess)
            {
                (dataAccess as SQLiteDataAccess).DatabaseFilePath = pathToOutputFile;
            }
            dataAccess.CreateNew();
            SaveData();
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

        /// <summary>
        ///     Mark an item as deleted
        /// </summary>
        /// <param name="taskItemID">
        /// </param>
        public void DeleteTaskItem(Int32 projectItemID)
        {
            ForAllTreeItems
            (
                u =>
                {
                    if (u.ProjectItemID == projectItemID)
                    {
                        (u as ProjectModelTreeBranchItem).IsDeleted = true;
                        this.ModelChanged = true;
                    }
                }
            );
        }

        public void AddTaskItem(Int32 parentProjectItemID, ProjectTreeItemBase newItem)
        {
            ForAllTreeItems
            (
                u =>
                {
                    if (u.ProjectItemID == parentProjectItemID)
                    {
                        ProjectModelTreeBranchItem newChild = new ProjectModelTreeBranchItem
                        {
                            ProjectItemID = newItem.ProjectItemID,
                            ItemDescription = newItem.ItemDescription,
                            ParentProjectItemID = parentProjectItemID,
                            TreeLevel = newItem.TreeLevel,
                            MinimumTimeMinutes = newItem.MinimumTimeMinutes,
                            MaximumTimeMinutes = newItem.MaximumTimeMinutes,
                            EstimatedTimeMinutes = newItem.EstimatedTimeMinutes,
                            PercentageComplete = newItem.PercentageComplete,
                            TimeSpentMinutes = newItem.TimeSpentMinutes,
                            IsAdded = true
                        };

                        if (u.Children == null)
                        {
                            u.Children = new ObservableCollection<ProjectTreeItemBase>();
                        }
                        u.Children.Add(newChild);
                        this.ModelChanged = true;
                    }
                }
            );
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
        private void ExpandIntoTree(IEnumerable<ProjectTreeItemBase> taskItems)
        {
            Dictionary<Int32, ProjectModelTreeBranchItem> treeBuilder = new Dictionary<int, ProjectModelTreeBranchItem>();

            ProjectModelTreeBranchItem rootItem = null;
            foreach (ProjectTreeItemBase taskItem in taskItems)
            {
                ProjectTreeBranchItem treeItem = taskItem as ProjectTreeBranchItem;
                if (treeItem == null) continue;

                treeBuilder.Add(treeItem.ProjectItemID, CloneProjectTask(treeItem));
                if (treeItem.ParentProjectItemID == 0)
                {
                    rootItem = treeBuilder[treeItem.ProjectItemID];
                }
            }

            foreach (ProjectModelTreeBranchItem item in treeBuilder.Values)
            {
                if (item.ParentProjectItemID == 0) continue;

                ProjectModelTreeBranchItem parentItem = treeBuilder[item.ParentProjectItemID];
                if (parentItem.Children == null)
                {
                    parentItem.Children = new ObservableCollection<ProjectTreeItemBase>();
                }
                parentItem.Children.Add(item);
            }

            SetTreeLevels(rootItem, 0);
            this.ProjectTaskItemsRoot = rootItem;
        }

        private void SetTreeLevels(ProjectTreeItemBase treeItem, Int32 level)
        {
            treeItem.TreeLevel = level;
            if (treeItem.Children != null)
            {
                foreach (ProjectTreeItemBase child in treeItem.Children)
                {
                    SetTreeLevels(child, level + 1);
                }
            }
        }

        private ProjectModelTreeBranchItem CloneProjectTask(ProjectTreeBranchItem taskItem)
        {
            ProjectModelTreeBranchItem result = new ProjectModelTreeBranchItem()
            {
                ProjectItemID = taskItem.ProjectItemID,
                ParentProjectItemID = taskItem.ParentProjectItemID,
                ItemDescription = taskItem.ItemDescription,
                EstimatedTimeMinutes = taskItem.EstimatedTimeMinutes,
                MinimumTimeMinutes = taskItem.MinimumTimeMinutes,
                MaximumTimeMinutes = taskItem.MaximumTimeMinutes,
                TimeSpentMinutes = taskItem.TimeSpentMinutes,
                PercentageComplete = taskItem.PercentageComplete,
                TrackingFlags = ProjectModelTreeBranchItem.ChangeTrackingFlags.Unchanged
            };

            return result;
        }

        private List<ProjectModelTreeBranchItem> OrganiseTaskItemsForStoring()
        {
            List<ProjectModelTreeBranchItem> result = new List<ProjectModelTreeBranchItem>();

            OrganiseTaskItemsForStoring(this.mProjectItemsRoot, null, result);

            return result;
        }

        private void OrganiseTaskItemsForStoring
        (
            ProjectModelTreeBranchItem taskItem, 
            ProjectModelTreeBranchItem parentTaskItem, 
            List<ProjectModelTreeBranchItem> resultList
        )
        {
            Boolean permutationFound = false;
            foreach (Tuple<ProjectModelTreeBranchItem.ChangeTrackingFlags, ProjectModelTreeBranchItem.ChangeTrackingFlags> flagsPermutation in SaveActionTable)
            {
                if (taskItem.TrackingFlags == flagsPermutation.Item1)
                {
                    if (flagsPermutation.Item2 != ProjectModelTreeBranchItem.ChangeTrackingFlags.Unchanged)
                    {
                        taskItem.ParentProjectItemID = (parentTaskItem == null) ? 0 : parentTaskItem.ProjectItemID;
                        taskItem.TrackingFlags = flagsPermutation.Item2;
                        resultList.Add(taskItem);
                    }
                    permutationFound = true;
                    break;
                }
            }
            if (!permutationFound)
            {
                Utility.Logger.Log(LogLevel.Warn, "{0}: Permutation of tracking flags not found: {1}", nameof(OrganiseTaskItemsForStoring), taskItem.TrackingFlags.ToString());
            }

            if (taskItem.Children != null)
            {
                foreach (ProjectModelTreeBranchItem child in taskItem.Children)
                {
                    OrganiseTaskItemsForStoring(child, taskItem, resultList);
                }
            }
        }

        private void WriteTaskItem(ProjectModelTreeBranchItem item)
        {
            if (item.IsAdded)
            {
                DataAccessObject.InsertOrUpdateTaskItem
                (
                    item.ProjectItemID,
                    item.ParentProjectItemID,
                    item.ItemDescription,
                    (Int32)item.EstimatedTimeMinutes,
                    (Int32)item.MinimumTimeMinutes,
                    (Int32)item.MaximumTimeMinutes,
                    item.PercentageComplete,
                    (Int32)item.TimeSpentMinutes
                );
                item.TrackingFlags = ProjectModelTreeBranchItem.ChangeTrackingFlags.Unchanged;
            }
            else if (item.IsModified)
            {
                DataAccessObject.ArchiveTaskItem(item.ProjectItemID);
                DataAccessObject.InsertOrUpdateTaskItem
                (
                    item.ProjectItemID,
                    item.ParentProjectItemID,
                    item.ItemDescription,
                    (Int32)item.EstimatedTimeMinutes,
                    (Int32)item.MinimumTimeMinutes,
                    (Int32)item.MaximumTimeMinutes,
                    item.PercentageComplete,
                    (Int32)item.TimeSpentMinutes
                );
                item.TrackingFlags = ProjectModelTreeBranchItem.ChangeTrackingFlags.Unchanged;
            }
            else if (item.IsDeleted)
            {
                DataAccessObject.DeleteTaskItem(item.ProjectItemID, true);
            }
        }

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
