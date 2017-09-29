using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEstimationTool.Classes
{
    /// <summary>
    ///     Extension of the <see cref="ProjectTreeBranchItem"/> class used in the model.
    /// </summary>
    public class ProjectModelTreeBranchItem : ProjectTreeBranchItem, ICloneable
    {
        [Flags]
        public enum ChangeTrackingFlags
        {
            Unchanged       = 0,
            Added           = 0x01,
            Modified        = 0x02,
            Deleted         = 0x04
        } // enum ChangeTrackingFlags

        #region Private data members
        /// <summary>
        ///     Flags to track changes to the object.
        /// </summary>
        private ChangeTrackingFlags mChangeTrackingFlags = ChangeTrackingFlags.Unchanged;
        #endregion Private data members

        public ChangeTrackingFlags TrackingFlags
        {
            get { return this. mChangeTrackingFlags; }
            set { this.mChangeTrackingFlags = value; }
        }

        public Boolean IsUnchanged
        {
            get { return this.mChangeTrackingFlags == ChangeTrackingFlags.Unchanged; }
        }

        public Boolean IsAdded
        {
            get { return ChangeTrackingFlags.Added == (this.mChangeTrackingFlags & ChangeTrackingFlags.Added); }
            set
            {
                if (value == true)
                {
                    this.mChangeTrackingFlags |= ChangeTrackingFlags.Added;
                }
                else
                {
                    this.mChangeTrackingFlags &= ~ChangeTrackingFlags.Added;
                }
            }
        }

        public Boolean IsModified
        {
            get { return ChangeTrackingFlags.Modified == (this.mChangeTrackingFlags & ChangeTrackingFlags.Modified); }
            set
            {
                if (value == true)
                {
                    this.mChangeTrackingFlags |= ChangeTrackingFlags.Modified;
                }
                else
                {
                    this.mChangeTrackingFlags &= ~ChangeTrackingFlags.Modified;
                }
            }
        }

        public Boolean IsDeleted
        {
            get { return ChangeTrackingFlags.Deleted == (this.mChangeTrackingFlags & ChangeTrackingFlags.Deleted); }
            set
            {
                if (value == true)
                {
                    this.mChangeTrackingFlags |= ChangeTrackingFlags.Deleted;
                }
                else
                {
                    this.mChangeTrackingFlags &= ~ChangeTrackingFlags.Deleted;
                }
            }
        }

        public override Object Clone()
        {
            ProjectModelTreeBranchItem result = new ProjectModelTreeBranchItem();
            PopulateCloneFields(result);
            return result;
        }

        protected override void PopulateCloneFields(ProjectTreeItemBase clone)
        {
            base.PopulateCloneFields(clone);
            if (clone is ProjectModelTreeBranchItem)
            {
                (clone as ProjectModelTreeBranchItem).mChangeTrackingFlags = this.mChangeTrackingFlags;
            }
        }

        protected override void FieldUpdated()
        {
            this.IsModified = true;
        }
    } // class ProjectModelTreeBranchItem
} // namespace ProjectEstimationTool.Classes
