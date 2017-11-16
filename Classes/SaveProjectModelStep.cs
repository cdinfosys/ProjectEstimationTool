using System;
using System.Windows;
using ProjectEstimationTool.Events;
using ProjectEstimationTool.Model;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool.Classes
{
    class SaveProjectModelStep : ProjectModelProcessingStepBase
    {
        private ProjectModel mProjectModel;

        public SaveProjectModelStep(ProjectModel projectModel, ProjectModelProcessingStepBase nextStep)
            :   base(nextStep)
        {
            this.mProjectModel = projectModel;
        }

        public SaveProjectModelStep(ProjectModel projectModel)
        {
            this.mProjectModel = projectModel;
        }

        protected override Boolean PerformAction()
        {
            if (!this.mProjectModel.ProjectPathSet)
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

            this.mProjectModel.SaveData();
            return true;
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
