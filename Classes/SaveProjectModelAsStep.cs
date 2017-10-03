using System;
using System.Windows;
using ProjectEstimationTool.Events;
using ProjectEstimationTool.Model;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool.Classes
{
    public class SaveProjectModelAsStep : ProjectModelProcessingStepBase
    {
        private ProjectModel mProjectModel;

        public SaveProjectModelAsStep(ProjectModel projectModel, ProjectModelProcessingStepBase nextStep)
            :   base(nextStep)
        {
            this.mProjectModel = projectModel;
        }

        public SaveProjectModelAsStep(ProjectModel projectModel)
        {
            this.mProjectModel = projectModel;
        }

        protected override Boolean PerformAction()
        {
            Utility.EventAggregator.GetEvent<ProjectModelFilePathRequiredEvent>().Publish
            (
                new ProjectModelFilePathRequiredEventPayload(this)
                {
                    ShowOpenDialog = false
                }
            );
            return false;
        }

        public override ProjectModelProcessingStepBase ContinueWithUserInput(MessageBoxResult userInput, Object additionalData)
        {
            switch (userInput)
            {
                case MessageBoxResult.OK:
                    this.mProjectModel.SaveDataAs(additionalData as String);
                    return this.NextStep;

                default:
                    return null;
            }
        }
    }
}
