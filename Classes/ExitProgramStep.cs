using System;
using ProjectEstimationTool.Events;
using ProjectEstimationTool.Model;
using ProjectEstimationTool.Utilities;
using ProjectEstimationTool.ViewModels;

namespace ProjectEstimationTool.Classes
{
    public class ExitProgramStep : ProjectModelProcessingStepBase
    {
        private MainWindowViewModel mMainViewModel;

        public ExitProgramStep(MainWindowViewModel mainViewModel, ProjectModelProcessingStepBase nextStep)
            :   base(nextStep)
        {
            this.mMainViewModel = mainViewModel;
        }

        public ExitProgramStep(MainWindowViewModel mainViewModel)
        {
            this.mMainViewModel = mainViewModel;
        }

        protected override Boolean PerformAction()
        {
            //Utility.EventAggregator.GetEvent<ExitProgramEvent>().Publish(0);
            this.mMainViewModel.CanCloseMainWindow = true;
            return true;
        }
    } // class ExitProgramStep
} // namespace ProjectEstimationTool.Classes
