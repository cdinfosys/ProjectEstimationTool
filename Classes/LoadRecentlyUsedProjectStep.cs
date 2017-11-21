using System;
using ProjectEstimationTool.Model;

namespace ProjectEstimationTool.Classes
{
    sealed class LoadRecentlyUsedProjectStep : ProjectModelProcessingStepBase
    {
        private ProjectModel mProjectModel;
        private String mFilePath;

        public LoadRecentlyUsedProjectStep(ProjectModel projectModel, String filePath, ProjectModelProcessingStepBase nextStep)
            :   base(nextStep)
        {
            this.mProjectModel = projectModel;
            this.mFilePath = filePath;
        }

        public LoadRecentlyUsedProjectStep(ProjectModel projectModel, String filePath)
        {
            this.mProjectModel = projectModel;
            this.mFilePath = filePath;
        }

        protected override Boolean PerformAction()
        {
            this.mProjectModel.LoadData(this.mFilePath);
            return true;
        }
    } // class LoadProjectModelStep
} // namespace ProjectEstimationTool.Classes
