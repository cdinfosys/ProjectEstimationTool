﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectEstimationTool.Classes
{
    public abstract class ProjectModelProcessingStepBase
    {
        private ProjectModelProcessingStepBase mNextStep;

        public ProjectModelProcessingStepBase()
        {
        }

        public ProjectModelProcessingStepBase(ProjectModelProcessingStepBase nextStep)
        {
            this.mNextStep = nextStep;
        }

        public void Execute()
        {
            try
            {
                if (PerformAction())
                {
                    this.mNextStep?.Execute();
                }
            }
            finally
            {
                PerformCleanup();
            }
        }

        protected ProjectModelProcessingStepBase NextStep => mNextStep;

        /// <summary>
        ///     This method is called by the UI after the user has made a selection from a messagebox.
        /// </summary>
        /// <param name="userInput">
        ///     Result from the message box.
        /// </param>
        /// <param name="additionalData">
        ///     Additional data for processing.
        /// </param>
        public virtual ProjectModelProcessingStepBase ContinueWithUserInput(MessageBoxResult userInput, Object additionalData)
        {
            return null;
        }

        /// <summary>
        ///     Implement this method to perform the work of the step.
        /// </summary>
        /// <returns>
        ///     Returns <c>true</c> if the action was performed successfully of <c>false</c> if the action was not performed
        /// </returns>
        protected abstract Boolean PerformAction();

        /// <summary>
        ///     Called after all child steps have been executed.
        /// </summary>
        protected virtual void PerformCleanup()
        {
        }
    } // class ProjectModelProcessingStepBase
} // ProjectEstimationTool.Classes
