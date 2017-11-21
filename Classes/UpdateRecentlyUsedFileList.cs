using ProjectEstimationTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEstimationTool.Classes
{
    class UpdateRecentlyUsedFileList : ProjectModelProcessingStepBase
    {
        private ProjectModel mProjectModel;

        public UpdateRecentlyUsedFileList(ProjectModel projectModel, ProjectModelProcessingStepBase nextStep)
            :   base(nextStep)
        {
            this.mProjectModel = projectModel;
        }

        public UpdateRecentlyUsedFileList(ProjectModel projectModel)
        {
            this.mProjectModel = projectModel;
        }

        protected override bool PerformAction()
        {
            mProjectModel.UpdateRecentlyUsedFilesList();
            return true;
        }
    }
}
