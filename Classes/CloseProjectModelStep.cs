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
    public class CloseProjectModelStep : ProjectModelProcessingStepBase
    {
        private ProjectModel mProjectModel;

        public CloseProjectModelStep(ProjectModel projectModel, ProjectModelProcessingStepBase nextStep)
            :   base(nextStep)
        {
            this.mProjectModel = projectModel;
        }

        public CloseProjectModelStep(ProjectModel projectModel)
        {
            this.mProjectModel = projectModel;
        }

        protected override Boolean PerformAction()
        {
            if (this.mProjectModel.ModelChanged)
            {
                Utility.EventAggregator.GetEvent<ProjectModelUserInputRequiredEvent>().Publish
                (
                    new ProjectModelUserInputRequiredEventPayload
                    (
                        this,
                        Resources.QuestionSaveChanges,
                        Resources.CaptionUnsavedChanges,
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question,
                        MessageBoxResult.Yes
                    )
                );
                return false;
            }

            this.mProjectModel.CloseModel();
            return true;
        }

        public override ProjectModelProcessingStepBase ContinueWithUserInput(MessageBoxResult userInput, Object additionalData)
        {
            switch(userInput)
            {
                case MessageBoxResult.Yes:
                    // Save data
                    return new SaveProjectModelStep(this.mProjectModel, this);

                case MessageBoxResult.No:
                    this.mProjectModel.DiscardModel();
                    return this;

                default:
                    return null;
            }
        }
    } // class CloseProjectModelStep
} // namespace ProjectEstimationTool.Classes
