
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEstimationTool.Properties;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool.Classes
{
    public class ProjectTreeBranchItem : ProjectTreeItemBase
    {
        private Int32 mProjectItemID;
        private Int32 mMinimumTimeMinutes;
        private Int32 mMaximumTimeMinutes;
        private Int32 mEstimatedTimeMinutes;
        private Int32 mTimeSpentMinutes;
        private Int32 mPercentageComplete;
        private Int32 mParentProjectItemID;

        public override Int32 ProjectItemID 
        { 
            get { return this.mProjectItemID; }
            set 
            {
                if (SetProperty(ref this.mProjectItemID, value))
                {
                    FieldUpdated();
                }
            }
        }

        public Int32 ParentProjectItemID
        {
            get { return this.mParentProjectItemID; }
            set { this.mParentProjectItemID = value; }
        }

        public override Int32 MinimumTimeMinutes
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
                    result += child.MinimumTimeMinutes;
                }
                return result; 
            }
            set
            {
                if (SetProperty(ref this.mMinimumTimeMinutes, value))
                {
                    FieldUpdated();
                    Utility.NotifyTaskItemChanged(this);
                }
            }
        }

        public override Int32 MaximumTimeMinutes
        { 
            get 
            { 
                if (this.IsLeaf)
                {
                    return this.mMaximumTimeMinutes;
                }

                Int32 result = 0;
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
                    Utility.NotifyTaskItemChanged(this);
                }
            }
        }

        public override Int32 EstimatedTimeMinutes
        { 
            get 
            { 
                if (this.IsLeaf)
                {
                    return this.mEstimatedTimeMinutes;
                }

                Int32 result = 0;
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
                    Utility.NotifyTaskItemChanged(this);
                }
            }
        }

        public override Int32 TimeSpentMinutes
        { 
            get 
            { 
                if (this.IsLeaf)
                {
                    return this.mTimeSpentMinutes;
                }

                Int32 result = 0;
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

        protected Int32 ChildrenTotalTimeSpentMinutes
        {
            get
            {
                if (this.IsLeaf)
                {
                    return this.mTimeSpentMinutes;
                }

                Int32 result = 0;
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
