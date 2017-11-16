using System;

namespace ProjectEstimationTool.Classes
{
    class ProjectVersionDTO : ICloneable
    {
        private readonly Int32 mProjectVersionID;
        private readonly DateTime mVersionDate;

        public ProjectVersionDTO(Int32 projectVersionID, DateTime versionDate)
        {
            this.mProjectVersionID = projectVersionID;
            this.mVersionDate = versionDate;
        }

        public Int32 ProjectVersionID => mProjectVersionID;
        public DateTime VersionDate => mVersionDate;

        public Object Clone()
        {
            return new ProjectVersionDTO(this.mProjectVersionID, this.mVersionDate);
        }
    } // class ProjectVersionDTO
} // namespace ProjectEstimationTool.Classes
