using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEstimationTool.Classes
{
    public abstract class ProjectTreeItemBase : NotifyPropertyChangedBase, INotifyCollectionChanged, ICloneable
    {
        /// <summary>
        ///     This event is raised when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #region Class members
        private static Int32 mHighestProjectItemID = Int32.MinValue;
        #endregion // Class members

        #region Private data members
        /// <summary>
        ///     Next level of project items
        /// </summary>
        private ObservableCollection<ProjectTreeItemBase> mChildren;

        /// <summary>
        ///     Level in the tree where the item appears
        /// </summary>
        private Int32 mTreeLevel = 0;

        /// <summary>
        ///     Storage for the description of the task.
        /// </summary>
        private String mItemDescription;
        #endregion // Private data members

        #region Overridables
        public abstract Object Clone();

        public virtual void UpdateFrom(ProjectTreeItemBase other)
        {
            if (!Object.ReferenceEquals(other, this))
            {
                TreeLevel = other.mTreeLevel;
                ItemDescription = other.mItemDescription;
            }
        }
        #endregion Overridables

        #region Class methods
        public static void SetHighestProjectItemID(Int32 projectItemID)
        {
            mHighestProjectItemID = projectItemID;
        }

        /// <summary>
        ///     Gets the ProjectItemID for a new item to be added.
        /// </summary>
        /// <returns></returns>
        public static Int32 GetNextProjectItemID()
        {
            return ++mHighestProjectItemID;
        }
        #endregion Class methods

        #region Public Properties
        public virtual ObservableCollection<ProjectTreeItemBase> Children
        {
            get { return this.mChildren; }
            set 
            {
                if (!Object.Equals(this.mChildren, value))
                {
                    if (SetProperty<ObservableCollection<ProjectTreeItemBase>>(ref this.mChildren, value))
                    {
                        SetTreeLevel();
                    } 
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        public String ItemDescription
        { 
            get { return this.mItemDescription; }
            set { SetProperty(ref this.mItemDescription , value); }
        }

        public Int32 TreeLevel
        {
            get { return this.mTreeLevel; }
            set { this.mTreeLevel = value; }
        }

        public Boolean IsLeaf 
        {
            get 
            {
                return ((this.mChildren == null) || (this.mChildren.Count < 1)) ; 
            }
        }

        public virtual Int32 ProjectItemID
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                mHighestProjectItemID = Math.Max(mHighestProjectItemID, value);
            }
        }

        public abstract Double MinimumTimeMinutes { get; set; }
        public abstract Double MaximumTimeMinutes { get; set; }
        public abstract Double EstimatedTimeMinutes { get; set; }
        public abstract Double TimeSpentMinutes { get; set; }
        public abstract Int32 PercentageComplete { get; set; }
        #endregion Public Properties

        public void RaiseCollectionChanged()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
        }


        #region Protected helpers
        protected virtual void FieldUpdated()
        {
        }

        protected void SetTreeLevel()
        {
            if (this.mChildren != null)
            {
                foreach (ProjectTreeItemBase childItem in this.mChildren)
                {
                    childItem.SetTreeLevel(mTreeLevel);
                }
            }
        }

        protected void SetTreeLevel(Int32 parentLevel)
        {
            this.TreeLevel = parentLevel + 1;
            if (this.mChildren != null)
            {
                foreach (ProjectTreeItemBase childItem in this.mChildren)
                {
                    childItem.SetTreeLevel(parentLevel + 1);
                }
            }
        }

        protected virtual void PopulateCloneFields(ProjectTreeItemBase clone)
        {
            clone.mTreeLevel = this.mTreeLevel;
            clone.mItemDescription = this.mItemDescription;

            if ((this.mChildren != null) && (this.mChildren.Count > 0))
            {
                clone.mChildren = new ObservableCollection<ProjectTreeItemBase>();
                foreach (ProjectTreeItemBase child in this.mChildren)
                {
                    clone.mChildren.Add(child.Clone() as ProjectTreeItemBase);
                }
            }
        }
        #endregion Protected helpers
    } // class ProjectTreeItemBase
} // namespace ProjectEstimationTool.Classes
