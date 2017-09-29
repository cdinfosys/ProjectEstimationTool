using System;
using System.Collections.Generic;
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

        #region Private data members
        /// <summary>
        ///     Next level of project items
        /// </summary>
        private List<ProjectTreeItemBase> mChildren;

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

        #region Public Properties
        public virtual IList<ProjectTreeItemBase> Children
        {
            get { return this.mChildren; }
            set 
            {
                if (!Object.Equals(this.mChildren, value))
                {
                    if (SetProperty<List<ProjectTreeItemBase>>(ref this.mChildren, value.ToList()))
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
            protected set { this.mTreeLevel = value; }
        }

        public Boolean IsLeaf 
        {
            get 
            {
                return ((this.mChildren == null) || (this.mChildren.Count < 1)) ; 
            }
        }

        public abstract Int32 ProjectItemID { get; set; }
        public abstract Int32 MinimumTimeMinutes { get; set; }
        public abstract Int32 MaximumTimeMinutes { get; set; }
        public abstract Int32 EstimatedTimeMinutes { get; set; }
        public abstract Int32 TimeSpentMinutes { get; set; }
        public abstract Int32 PercentageComplete { get; set; }
        #endregion Public Properties

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
                clone.mChildren = new List<ProjectTreeItemBase>();
                foreach (ProjectTreeItemBase child in this.mChildren)
                {
                    clone.mChildren.Add(child.Clone() as ProjectTreeItemBase);
                }
            }
        }
        #endregion Protected helpers
    } // class ProjectTreeItemBase
} // namespace ProjectEstimationTool.Classes
