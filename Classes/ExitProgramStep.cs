using System;
using ProjectEstimationTool.Events;
using ProjectEstimationTool.Model;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool.Classes
{
    public class ExitProgramStep : ProjectModelProcessingStepBase
    {
        private ProjectModel mProjectModel;

        public ExitProgramStep(ProjectModel projectModel, ProjectModelProcessingStepBase nextStep)
            :   base(nextStep)
        {
            this.mProjectModel = projectModel;
        }

        public ExitProgramStep(ProjectModel projectModel)
        {
            this.mProjectModel = projectModel;
        }

        protected override Boolean PerformAction()
        {
            Utility.EventAggregator.GetEvent<ExitProgramEvent>().Publish(0);
            return true;
        }
    } // class ExitProgramStep
} // namespace ProjectEstimationTool.Classes
