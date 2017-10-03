using System;
using System.ComponentModel;
using ProjectEstimationTool.Properties;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool.Classes
{
    public class ProjectTreeBranchItem : ProjectTreeItemBase//, IDataErrorInfo
    {
        private Int32 mProjectItemID;
        private Double mMinimumTimeMinutes;
        private Double mMaximumTimeMinutes;
        private Double mEstimatedTimeMinutes;
        private Double mTimeSpentMinutes;
        private Int32 mPercentageComplete;
        private Int32 mParentProjectItemID;

        public override Int32 ProjectItemID 
        { 
            get { return this.mProjectItemID; }
            set 
            {
                if (SetProperty(ref this.mProjectItemID, value))
                {
                    base.ProjectItemID = value;
                    FieldUpdated();
                }
            }
        }

        public Int32 ParentProjectItemID
        {
            get { return this.mParentProjectItemID; }
            set { this.mParentProjectItemID = value; }
        }

        public override Double MinimumTimeMinutes
        { 
            get
            {
                if (this.IsLeaf)
                {
                    return this.mMinimumTimeMinutes;
                }

                Int32 result = 0;
                foreach (ProjectTreeItemBase child in this.Children)
                {
                    result += (Int32)child.MinimumTimeMinutes;
                }
                return result; 
            }
            set
            {
                if (SetProperty(ref this.mMinimumTimeMinutes, value))
                {
                    FieldUpdated();
                    //OnPropertyChanged(nameof(MaximumTimeMinutes));
                    //OnPropertyChanged(nameof(EstimatedTimeMinutes));
                    Utility.NotifyTaskItemChanged(this);
                }
            }
        }

        public override Double MaximumTimeMinutes
        { 
            get 
            { 
                if (this.IsLeaf)
                {
                    return this.mMaximumTimeMinutes;
                }

                Double result = 0.0;
                foreach (ProjectTreeItemBase child in this.Children)
                {
                    result += child.MaximumTimeMinutes;
                }
                return result; 
            }
            set 
            {
                if (SetProperty(ref this.mMaximumTimeMinutes, value))
                {
                    FieldUpdated();
                    //OnPropertyChanged(nameof(MinimumTimeMinutes));
                    //OnPropertyChanged(nameof(EstimatedTimeMinutes));
                    Utility.NotifyTaskItemChanged(this);
                }
            }
        }

        public override Double EstimatedTimeMinutes
        { 
            get 
            { 
                if (this.IsLeaf)
                {
                    return this.mEstimatedTimeMinutes;
                }

                Double result = 0.0;
                foreach (ProjectTreeItemBase child in this.Children)
                {
                    result += child.EstimatedTimeMinutes;
                }
                return result; 
            }
            set 
            {
                if (SetProperty(ref this.mEstimatedTimeMinutes, value))
                {
                    FieldUpdated();
                    //OnPropertyChanged(nameof(MinimumTimeMinutes));
                    //OnPropertyChanged(nameof(MaximumTimeMinutes));
                    Utility.NotifyTaskItemChanged(this);
                }
            }
        }

        public override Double TimeSpentMinutes
        { 
            get 
            { 
                if (this.IsLeaf)
                {
                    return this.mTimeSpentMinutes;
                }

                Double result = 0.0;
                foreach (ProjectTreeItemBase child in this.Children)
                {
                    result += child.TimeSpentMinutes;
                }
                return result; 
            }
            set 
            {
                if (SetProperty(ref this.mTimeSpentMinutes, value))
                {
                    FieldUpdated();
                    Utility.NotifyTaskItemChanged(this);
                }
            }
        }

        protected Double ChildrenTotalTimeSpentMinutes
        {
            get
            {
                if (this.IsLeaf)
                {
                    return this.mTimeSpentMinutes;
                }

                Double result = 0.0;
                foreach (ProjectTreeItemBase child in this.Children)
                {
                    result += child.TimeSpentMinutes;
                }

                return result;
            }
        }

        public override Int32 PercentageComplete
        { 
            get 
            { 
                if (this.IsLeaf)
                {
                    return this.mPercentageComplete;
                }

                Double branchEstimatedTimeMinutes = Convert.ToDouble(EstimatedTimeMinutes);

                Int32 result = 0;
                if (branchEstimatedTimeMinutes > 0.0)
                {
                    foreach (ProjectTreeItemBase child in this.Children)
                    {
                        Double ratioOfTotal = child.EstimatedTimeMinutes / branchEstimatedTimeMinutes;
                        Double childPercentageOfTotal = Convert.ToDouble(child.PercentageComplete) * ratioOfTotal;
                        result += Convert.ToInt32(childPercentageOfTotal);
                    }
                }

                return  result; 
            }

            set 
            { 
                if (SetProperty(ref this.mPercentageComplete, value))
                {
                    FieldUpdated();
                    Utility.NotifyTaskItemChanged(this);
                }
            }
        }

        /// <summary>
        ///     Clone the object.
        /// </summary>
        /// <returns>
        ///     Returns a new instance of the class with data copied from this instance.
        /// </returns>
        public override Object Clone()
        {
            ProjectTreeBranchItem result = new ProjectTreeBranchItem();
            PopulateCloneFields(result);
            return result;
        }

        public override void UpdateFrom(ProjectTreeItemBase other)
        {
            if (!Object.ReferenceEquals(other, this))
            {
                base.UpdateFrom(other);
                this.ProjectItemID = other.ProjectItemID;
                this.MinimumTimeMinutes = other.MinimumTimeMinutes;
                this.MaximumTimeMinutes = other.MaximumTimeMinutes;
                this.EstimatedTimeMinutes = other.EstimatedTimeMinutes;
                this.TimeSpentMinutes = other.TimeSpentMinutes;
                this.PercentageComplete = other.PercentageComplete;
            }
        }

        protected override void PopulateCloneFields(ProjectTreeItemBase clone)
        {
            base.PopulateCloneFields(clone);
            if (clone is ProjectTreeBranchItem)
            {
                (clone as ProjectTreeBranchItem).mProjectItemID = this.mProjectItemID;
                (clone as ProjectTreeBranchItem).mMinimumTimeMinutes = this.mMinimumTimeMinutes;
                (clone as ProjectTreeBranchItem).mMaximumTimeMinutes = this.mMaximumTimeMinutes;
                (clone as ProjectTreeBranchItem).mEstimatedTimeMinutes = this.EstimatedTimeMinutes;
                (clone as ProjectTreeBranchItem).mTimeSpentMinutes = this.mTimeSpentMinutes;
                (clone as ProjectTreeBranchItem).mPercentageComplete = this.mPercentageComplete;
            }
        }
    } // class ProjectTreeBranchItem
} // namespace ProjectEstimationTool.Classes
