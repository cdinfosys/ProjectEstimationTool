﻿using System;
using System.Windows;
using ProjectEstimationTool.Events;
using ProjectEstimationTool.Model;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool.Classes
{
    class NewProjectModelStep  : ProjectModelProcessingStepBase
    {
        private ProjectModel mProjectModel;

        public NewProjectModelStep(ProjectModel projectModel, ProjectModelProcessingStepBase nextStep)
            :   base(nextStep)
        {
            this.mProjectModel = projectModel;
        }

        public NewProjectModelStep(ProjectModel projectModel)
        {
            this.mProjectModel = projectModel;
        }

        protected override Boolean PerformAction()
        {
            this.mProjectModel.NewModel();
            return true;
        }

        public override ProjectModelProcessingStepBase ContinueWithUserInput(MessageBoxResult userInput, Object additionalData)
        {
            return null;
        }
    } // class NewProjectModelStep
} // namespace ProjectEstimationTool.Classes
