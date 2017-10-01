using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEstimationTool.Classes;

namespace ProjectEstimationTool.Events
{
    public class ProjectModelFilePathRequiredEventPayload
    {
        private ProjectModelProcessingStepBase mCurrentStep;
        private Boolean mShowOpenDialog = false;

        public ProjectModelFilePathRequiredEventPayload(ProjectModelProcessingStepBase currentStep)
        {
            this.mCurrentStep = currentStep;
        }

        public ProjectModelProcessingStepBase CurrentStep => this.mCurrentStep;

        public Boolean ShowOpenDialog
        {
            get { return this.mShowOpenDialog; }
            set { this.mShowOpenDialog = value; }
        }
    }

    public class ProjectModelFilePathRequiredEvent : Prism.Events.PubSubEvent<ProjectModelFilePathRequiredEventPayload>
    {
    } // class UnsavedModelChangesEvent
} // namespace ProjectEstimationTool.Events
