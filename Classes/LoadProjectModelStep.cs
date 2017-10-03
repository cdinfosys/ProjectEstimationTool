using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ProjectEstimationTool.Events;
using ProjectEstimationTool.Model;
using ProjectEstimationTool.Properties;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool.Classes
{
    public class LoadProjectModelStep : ProjectModelProcessingStepBase
    {
        private ProjectModel mProjectModel;

        public LoadProjectModelStep(ProjectModel projectModel, ProjectModelProcessingStepBase nextStep)
            :   base(nextStep)
        {
            this.mProjectModel = projectModel;
        }

        public LoadProjectModelStep(ProjectModel projectModel)
        {
            this.mProjectModel = projectModel;
        }

        protected override Boolean PerformAction()
        {
            Utility.EventAggregator.GetEvent<ProjectModelFilePathRequiredEvent>().Publish
            (
                new ProjectModelFilePathRequiredEventPayload(this)
                {
                    ShowOpenDialog = true
                }
            );
            return true;
        }

        public override ProjectModelProcessingStepBase ContinueWithUserInput(MessageBoxResult userInput, Object additionalData)
        {
            switch (userInput)
            {
                case MessageBoxResult.OK:
                    this.mProjectModel.LoadData(additionalData as String);
                    return this.NextStep;

                default:
                    return null;
            }
        }

    } // class LoadProjectModelStep
} // namespace ProjectEstimationTool.Classes
